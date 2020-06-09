using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Greentube.Monitoring.Tests
{
    public sealed class ResourceStateCollectorTests
    {
        private sealed class Fixture
        {
            public IResourceMonitor ResourceMonitorMock1 { get; } = Substitute.For<IResourceMonitor>();
            public IResourceMonitor ResourceMonitorMock2 { get; } = Substitute.For<IResourceMonitor>();
            public IEnumerable<IResourceMonitor> ResourceMonitorsMocks { get; set; }
            private ILogger<ResourceStateCollector> LoggerMock { get; } = Substitute.For<ILogger<ResourceStateCollector>>();
            public int MaxStatePerResource { private get; set; } = 100;

            public Fixture()
            {
                ResourceMonitorsMocks = new[] { ResourceMonitorMock1, ResourceMonitorMock2 };
            }

            public IResourceStateCollector GetSut()
            {
                return new ResourceStateCollector(ResourceMonitorsMocks, MaxStatePerResource, LoggerMock);
            }
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void AddMonitor_CollectorNotStarted_DoesNotStartNewMonitor()
        {
            var sut = _fixture.GetSut();
            var monitor = Substitute.For<IResourceMonitor>();
            // Act
            sut.AddMonitor(monitor);
            // Assert
            monitor.DidNotReceive().Start();
        }

        [Fact]
        public void AddMonitor_CollectorStarted_StartsNewMonitor()
        {
            var sut = _fixture.GetSut();
            var monitor = Substitute.For<IResourceMonitor>();
            // Act
            sut.Start();
            sut.AddMonitor(monitor);
            // Assert
            monitor.Received().Start();
        }

        [Fact]
        public void AddMonitor_CollectorNotStarted_GetStatesReturnsNewMonitor()
        {
            var sut = _fixture.GetSut();
            var monitor = Substitute.For<IResourceMonitor>();
            // Act
            sut.AddMonitor(monitor);
            // Assert
            var actual = sut.GetStates().FirstOrDefault(s => s.ResourceMonitor == monitor);
            Assert.NotNull(actual);
        }

        [Fact]
        public void AddMonitor_CollectorStarted_GetStatesReturnsNewMonitor()
        {
            var sut = _fixture.GetSut();
            var monitor = Substitute.For<IResourceMonitor>();
            // Act
            sut.Start();
            sut.AddMonitor(monitor);
            // Assert
            var actual = sut.GetStates().FirstOrDefault(s => s.ResourceMonitor == monitor);
            Assert.NotNull(actual);
        }

        [Fact]
        public void RemoveMonitor_CollectorStarted_StopsResourceMonitor()
        {
            var sut = _fixture.GetSut();
            var monitor = Substitute.For<IResourceMonitor>();
            // Act
            sut.Start();
            sut.AddMonitor(monitor);
            sut.RemoveMonitor(monitor);
            // Assert
            monitor.Received().Start();
            monitor.Received().Stop();
        }

        [Fact]
        public void RemoveMonitor_CollectorNotStarted_DoesNotStopResourceMonitor()
        {
            var sut = _fixture.GetSut();
            var monitor = Substitute.For<IResourceMonitor>();
            // Act
            sut.AddMonitor(monitor);
            sut.RemoveMonitor(monitor);
            // Assert
            monitor.DidNotReceive().Start();
            monitor.DidNotReceive().Stop();
        }

        [Fact]
        public void RemoveMonitor_CollectorNotStarted_GetStatesDoesNotReturnMonitor()
        {
            var sut = _fixture.GetSut();
            var monitor = Substitute.For<IResourceMonitor>();
            // Act
            sut.AddMonitor(monitor);
            sut.RemoveMonitor(monitor);
            // Assert
            var actual = sut.GetStates().FirstOrDefault(s => s.ResourceMonitor == monitor);
            Assert.Null(actual);
        }

        [Fact]
        public void RemoveMonitor_CollectorStarted_GetStatesDoesNotReturnMonitor()
        {
            var sut = _fixture.GetSut();
            sut.Start();
            var monitor = Substitute.For<IResourceMonitor>();
            // Act
            sut.AddMonitor(monitor);
            sut.RemoveMonitor(monitor);
            // Assert
            var actual = sut.GetStates().FirstOrDefault(s => s.ResourceMonitor == monitor);
            Assert.Null(actual);
        }

        [Fact]
        public void RemoveMonitor_UnknownMonitor_RemoveIsNoOp()
        {
            var sut = _fixture.GetSut();
            var monitor = Substitute.For<IResourceMonitor>();
            // Act
            sut.RemoveMonitor(monitor);
            // Assert
            var actual = sut.GetStates().FirstOrDefault(s => s.ResourceMonitor == monitor);
            Assert.Null(actual);
        }

        [Fact]
        public void Start_StartsAllResourceMonitors()
        {
            var sut = _fixture.GetSut();

            sut.Start();

            Assert.All(_fixture.ResourceMonitorsMocks, monitor => monitor.Received(1).Start());
        }

        [Fact]
        public void Start_SubscribesAllResourceMonitors()
        {
            var sut = _fixture.GetSut();
            sut.Start();

            // Act
            _fixture.ResourceMonitorMock1.MonitoringEvent += Raise.EventWith(new object(), new ResourceMonitorEventArgs());
            _fixture.ResourceMonitorMock2.MonitoringEvent += Raise.EventWith(new object(), new ResourceMonitorEventArgs());

            // Assert
            Assert.Equal(2, sut.GetStates().Count());
        }

        [Fact]
        public void Start_MarksAsRunning()
        {
            var sut = _fixture.GetSut();
            sut.Start();

            Assert.True(sut.IsRunning);
        }

        [Fact]
        public void Stop_MarksAsNotRunning()
        {
            var sut = _fixture.GetSut();
            sut.Start();
            sut.Stop();

            Assert.False(sut.IsRunning);
        }

        [Fact]
        public void Stop_StopsAllResourceMonitors()
        {
            var sut = _fixture.GetSut();

            sut.Start();
            sut.Stop();

            Assert.All(_fixture.ResourceMonitorsMocks, monitor => monitor.Received(1).Start());
            Assert.All(_fixture.ResourceMonitorsMocks, monitor => monitor.Received(1).Stop());
        }

        [Fact]
        public void Stop_UnsubscribesAllResourceMonitors()
        {
            var sut = _fixture.GetSut();
            sut.Start();
            sut.Stop();

            // Act
            _fixture.ResourceMonitorMock1.MonitoringEvent += Raise.EventWith(new object(), new ResourceMonitorEventArgs());
            _fixture.ResourceMonitorMock2.MonitoringEvent += Raise.EventWith(new object(), new ResourceMonitorEventArgs());

            // Assert
            Assert.Empty(sut.GetStates().SelectMany(s => s.MonitorEvents));
        }

        [Fact]
        public void GetStates_NoValidStates_ReturnsEmptyEnumerable()
        {
            var sut = _fixture.GetSut();

            Assert.Empty(sut.GetStates().SelectMany(s => s.MonitorEvents));
        }

        [Fact]
        public void GetStates_MoreThanMaxEvents_ReturnsOnlyMaxValue()
        {
            _fixture.MaxStatePerResource = 1;
            var sut = _fixture.GetSut();
            sut.Start();

            // Act
            _fixture.ResourceMonitorMock1.MonitoringEvent += Raise.EventWith(new object(), new ResourceMonitorEventArgs());
            _fixture.ResourceMonitorMock2.MonitoringEvent += Raise.EventWith(new object(), new ResourceMonitorEventArgs());

            // Assert single event per ResourceState
            Assert.Equal(sut.GetStates().Count(), sut.GetStates().SelectMany(s => s.MonitorEvents).Count());
        }

        [Fact]
        public void GetStates_DownEvent_ReturnsResourceDown()
        {
            // Use only the first Resource
            _fixture.ResourceMonitorsMocks = new[] { _fixture.ResourceMonitorMock1 };
            var sut = _fixture.GetSut();
            sut.Start();

            var down = new ResourceMonitorEventArgs { IsUp = false };
            _fixture.ResourceMonitorMock1.MonitoringEvent += Raise.EventWith(new object(), down);

            Assert.False(sut.GetStates().Single().IsUp);
        }

        [Fact]
        public void GetStates_UpEvent_ReturnsResourceUp()
        {
            // Use only the first Resource
            _fixture.ResourceMonitorsMocks = new[] { _fixture.ResourceMonitorMock1 };
            var sut = _fixture.GetSut();
            sut.Start();

            var down = new ResourceMonitorEventArgs { IsUp = true };
            _fixture.ResourceMonitorMock1.MonitoringEvent += Raise.EventWith(new object(), down);

            Assert.True(sut.GetStates().Single().IsUp);
        }

        [Fact]
        public void GetStates_DownAndUpEvent_ReturnsResourceUp()
        {
            _fixture.ResourceMonitorsMocks = new[] { _fixture.ResourceMonitorMock1 };
            var sut = _fixture.GetSut();
            sut.Start();

            var down = new ResourceMonitorEventArgs { IsUp = false };
            _fixture.ResourceMonitorMock1.MonitoringEvent += Raise.EventWith(new object(), down);

            var up = new ResourceMonitorEventArgs { IsUp = true };
            _fixture.ResourceMonitorMock1.MonitoringEvent += Raise.EventWith(new object(), up);

            Assert.True(sut.GetStates().Single().IsUp);
        }

        [Fact]
        public void GetStates_UpAndDownEvent_ReturnsResourceDown()
        {
            _fixture.ResourceMonitorsMocks = new[] { _fixture.ResourceMonitorMock1 };
            var sut = _fixture.GetSut();
            sut.Start();

            var up = new ResourceMonitorEventArgs { IsUp = true };
            _fixture.ResourceMonitorMock1.MonitoringEvent += Raise.EventWith(new object(), up);

            var down = new ResourceMonitorEventArgs { IsUp = false };
            _fixture.ResourceMonitorMock1.MonitoringEvent += Raise.EventWith(new object(), down);

            Assert.False(sut.GetStates().Single().IsUp);
        }

        [Fact]
        public void Constructor_NullResourceMonitor_ThrowsArgumentNull()
        {
            _fixture.ResourceMonitorsMocks = null;
            Assert.Throws<ArgumentNullException>(() => _fixture.GetSut());
        }
    }
}
