using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Greentube.Monitoring.Threading;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Greentube.Monitoring.Tests
{
    public sealed class ResourceMonitorTests
    {
        private sealed class Fixture
        {
            private string ResourceName { get; } = "Resource Monitor Test";
            public IHealthCheckStrategy HealthCheckStrategy { get; set; } = Substitute.For<IHealthCheckStrategy>();
            public ResourceMonitorConfiguration ResourceMonitorConfiguration { private get; set; } = new ResourceMonitorConfiguration(
                true,
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromMilliseconds(0),
                TimeSpan.FromSeconds(2));
            public ILogger<ResourceMonitor> Logger { get; set; } = Substitute.For<ILogger<ResourceMonitor>>();
            public  bool IsCritical { get; set;  } = true;
            public ITimerFactory TimerFactory { get; set; } = Substitute.For<ITimerFactory>();
            public ITimer Timer { get; } = Substitute.For<ITimer>();

            public Fixture()
            {
                TimerFactory.Create(Arg.Any<TimerCallback>()).Returns(Timer);
            }

            public ResourceMonitor GetSut()
            {
                return new ResourceMonitor(
                    ResourceName,
                    HealthCheckStrategy,
                    ResourceMonitorConfiguration,
                    Logger,
                    IsCritical,
                    TimerFactory);
            }
        }

        private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(2);
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void Verify_StrategyThrowsException_RaisesEventResourceDown()
        {
            var expected = new ArithmeticException();
            _fixture.HealthCheckStrategy.Check(Arg.Any<CancellationToken>()).Throws(expected);

            var target = _fixture.GetSut();

            var evt = new ManualResetEventSlim();
            target.MonitoringEvent += (sender, args) =>
            {
                Assert.False(args.IsUp);
                var actual = Assert.IsType<ArithmeticException>(args.Exception);
                Assert.Same(expected, actual);
                evt.Set();
            };

            target.Verify();

            Assert.True(evt.Wait(TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public void Verify_BlockingVerification_ThrowsTimeoutException()
        {
            // Verification will take longer than Timeout
            _fixture.ResourceMonitorConfiguration = new ResourceMonitorConfiguration(true, timeout: TimeSpan.Zero);
            var nonStartedTask = new Task<bool>(() => false);
            _fixture.HealthCheckStrategy.Check(Arg.Any<CancellationToken>()).Returns(nonStartedTask);

            var target = _fixture.GetSut();

            var evt = new ManualResetEventSlim();
            target.MonitoringEvent += (sender, args) =>
            {
                Assert.False(args.IsUp); // Has Timed out
                Assert.IsType<TimeoutException>(args.Exception);
                evt.Set();
            };

            target.Verify();

            Assert.True(evt.Wait(TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public void Verify_SuccessCheck_RaisesEvent()
        {
            _fixture.HealthCheckStrategy.Check(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            var evt = new ManualResetEventSlim();

            var sut = _fixture.GetSut();
            var sw = Stopwatch.StartNew();
            var testStartTimeUtc = DateTime.UtcNow;

            sut.MonitoringEvent += (s, a) =>
            {
                Assert.True(a.IsUp);
                Assert.Null(a.Exception);

                sw.Stop();
                AssertEventTime(a, sw, testStartTimeUtc);

                evt.Set();
            };

            sut.Verify();

            _fixture.Logger
                .DidNotReceive()
                .Log(LogLevel.Critical, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception>(), Arg.Any<Func<object, Exception, string>>());

            Assert.True(evt.Wait(WaitTimeout), "Callback not called or didn't not each event.Set()");
        }

        [Fact]
        public void Verify_FailedCheck_RaisesEvent()
        {
            _fixture.HealthCheckStrategy.Check(Arg.Any<CancellationToken>()).Throws<ArithmeticException>();

            var evt = new ManualResetEventSlim();

            var sut = _fixture.GetSut();
            var sw = Stopwatch.StartNew();
            var testStartTimeUtc = DateTime.UtcNow;

            sut.MonitoringEvent += (s, a) =>
            {
                Assert.False(a.IsUp);
                Assert.NotNull(a.Exception);
                Assert.IsType<ArithmeticException>(a.Exception);

                sw.Stop();
                AssertEventTime(a, sw, testStartTimeUtc);

                evt.Set();
            };

            sut.Verify();

            Assert.True(evt.Wait(WaitTimeout), "Callback not called or didn't not each event.Set()");
        }

        [Fact]
        public void Start_TimerIsStarted()
        {
            var upInterval = TimeSpan.FromDays(10);
            var downInterval = TimeSpan.FromDays(20);

            _fixture.ResourceMonitorConfiguration = new ResourceMonitorConfiguration(true, downInterval, upInterval);
            _fixture.HealthCheckStrategy.Check(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));

            using (var sut = _fixture.GetSut())
                sut.Start();

            _fixture.Timer.Received(1).Change(upInterval, upInterval);
        }

        [Fact]
        public void Stop_TimerIsStopped()
        {
            using (var sut = _fixture.GetSut())
            {
                sut.Start();
                sut.Stop();
            }

            _fixture.Timer.Received(1).Change(Timeout.Infinite, Timeout.Infinite);
        }

        [Fact]
        public void Verify_EventHandlerThrows_CriticalLogEntry()
        {
            using (var sut = _fixture.GetSut())
            {
                sut.MonitoringEvent += (s, a) =>
                {
                    throw new ArithmeticException();
                };

                Assert.Throws<ArithmeticException>(() => sut.Verify());

                _fixture.Logger
                    .Received(1)
                    .Log(LogLevel.Critical, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<ArithmeticException>(), Arg.Any<Func<object, Exception, string>>());
            }
        }

        [Theory]
        [InlineData(true, LogLevel.Error)]
        [InlineData(false, LogLevel.Warning)]
        public void Verify_ResourceStateDown_LogEntryLevelBasedOnCritical(bool isCritical, LogLevel expectedLogLevel)
        {
            var expectedException = new ArithmeticException();
            _fixture.IsCritical = isCritical;
            _fixture.HealthCheckStrategy.Check(Arg.Any<CancellationToken>()).Throws(expectedException);
            using (var sut = _fixture.GetSut())
            {
                sut.Verify();
                _fixture.Logger
                    .Received(1)
                    .Log(expectedLogLevel, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<ArithmeticException>(), Arg.Any<Func<object, Exception, string>>());
            }
        }

        [Fact]
        public void Verify_ResourceStateUp_LogEntryLevelExpected()
        {
            _fixture.IsCritical = false;
            _fixture.HealthCheckStrategy.Check(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            using (var sut = _fixture.GetSut())
            {
                sut.Verify();
                _fixture.Logger
                    .Received(1)
                    .Log(LogLevel.Trace, Arg.Any<EventId>(), Arg.Any<object>(), null, Arg.Any<Func<object, Exception, string>>());
            }
        }

        [Fact]
        public void Verify_FailCheck_IntervalChanges()
        {
            var upInterval = TimeSpan.FromDays(10);
            var downInterval = TimeSpan.FromDays(20);

            _fixture.ResourceMonitorConfiguration = new ResourceMonitorConfiguration(true, downInterval, upInterval);

            _fixture.HealthCheckStrategy
               .Check(Arg.Any<CancellationToken>())
               .Throws<ArithmeticException>();

            using (var sut = _fixture.GetSut())
                sut.Verify();

            _fixture.Timer.Received(1).Change(downInterval, downInterval);
        }

        [Fact]
        public void Verify_TimeoutCheck_ReportsDown()
        {
            _fixture.ResourceMonitorConfiguration = new ResourceMonitorConfiguration(true, timeout: TimeSpan.FromMilliseconds(2));

            _fixture.HealthCheckStrategy
               .Check(Arg.Any<CancellationToken>())
               .Returns(new Task<bool>(() => true, new CancellationToken(true)));

            var evt = new ManualResetEventSlim();
            using (var sut = _fixture.GetSut())
            {
                sut.MonitoringEvent += (s, a) =>
                {
                    Assert.False(a.IsUp);
                    evt.Set();
                };
                sut.Verify();
            }

            Assert.True(evt.Wait(WaitTimeout), "Callback not called or didn't not each event.Set()");
        }

        [Fact]
        public void Dispose_TimerIsDisposed()
        {
            _fixture.GetSut().Dispose();
            _fixture.Timer.Received(1).Dispose();
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsNullArgument()
        {
            _fixture.Logger = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }

        [Fact]
        public void Constructor_NullHeathCheckStrategy_ThrowsNullArgument()
        {
            _fixture.HealthCheckStrategy = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }

        [Fact]
        public void Constructor_NullTimerFactory_ThrowsNullArgument()
        {
            _fixture.TimerFactory = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }

        [Fact]
        public void Constructor_NullTimerFactoryReturn_ThrowsInvalidOperation()
        {
            _fixture.TimerFactory.Create(Arg.Any<TimerCallback>()).ReturnsNull();
            Assert.Throws<InvalidOperationException>(() => _fixture.GetSut());
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void AssertEventTime(ResourceMonitorEventArgs evt, Stopwatch sw, DateTime testStartTimeUtc)
        {
            Assert.True(evt.Latency < sw.Elapsed); // The check must have taken less then the wrapping code calling it
            Assert.True(evt.VerificationTimeUtc >= testStartTimeUtc,
                "Verification can't have started before the test that invoked it.");
            Assert.True(evt.VerificationTimeUtc <= DateTime.UtcNow, "The Verification can't have started after this assertion.");
        }
    }
}
