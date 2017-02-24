using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Greentube.Monitoring.AspNetCore.Tests
{
    public class HealthCheckMiddlewareTests
    {
        private class Fixture
        {
            public IResourceStateCollector ResourceStateCollector { get; } = Substitute.For<IResourceStateCollector>();
            public IVersionService VersionService { get; set; } = Substitute.For<IVersionService>();
            public RequestDelegate RequestDelegate { get; } = Substitute.For<RequestDelegate>();
            public HttpContext HttpContext { get; } = Substitute.For<HttpContext>();
            public HttpRequest HttpRequest { get; } = Substitute.For<HttpRequest>();
            public HttpResponse HttpResponse { get; } = Substitute.For<HttpResponse>();
            private Stream Stream { get; } = new MemoryStream();

            public Fixture()
            {
                HttpContext.Request.Returns(HttpRequest);
                HttpContext.Response.Returns(HttpResponse);
                HttpRequest.Method.Returns("GET");
                HttpResponse.Body.Returns(Stream);
            }

            public HealthCheckMiddleware GetSut()
            {
                return new HealthCheckMiddleware(RequestDelegate, ResourceStateCollector, VersionService);
            }

            public T ReadResponseBody<T>(T type)
            {
                Stream.Position = 0;
                using (var reader = new StreamReader(Stream))
                {
                    return JsonConvert.DeserializeAnonymousType(reader.ReadToEnd(), type);
                }
            }

            public HealthCheckDetailedResponse ReadDetailedResponse()
            {
                Stream.Position = 0;
                using (var reader = new StreamReader(Stream))
                {
                    return JsonConvert.DeserializeObject<HealthCheckDetailedResponse>(reader.ReadToEnd());
                }
            }
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task Invoke_PostMethod_PassThruNextMiddleware()
        {
            _fixture.HttpRequest.Method.Returns("POST");
            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            await _fixture.RequestDelegate.Received(1).Invoke(_fixture.HttpContext);
            _fixture.ResourceStateCollector.DidNotReceive().GetStates();
        }

        [Fact]
        public async Task Invoke_NoQueryStringParameter_ReturnsNodeStatusAsProperty()
        {
            var state = Substitute.For<IResourceCurrentState>();
            state.IsUp.Returns(true);
            _fixture.ResourceStateCollector.GetStates().Returns(new[] { state });
            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            var responseBody = _fixture.ReadResponseBody(new { Up = false });
            Assert.True(responseBody.Up);
        }

        [Fact]
        public async Task Invoke_DetailedAsQueryStringParameter_ReturnsNodeStatusAsProperty()
        {
            var state = Substitute.For<IResourceCurrentState>();
            state.IsUp.Returns(true);
            _fixture.ResourceStateCollector.GetStates().Returns(new[] { state });
            _fixture.HttpRequest.Query.ContainsKey("detailed").Returns(true);

            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            var responseBody = _fixture.ReadResponseBody(new { Up = false });
            Assert.True(responseBody.Up);
        }

        [Fact]
        public async Task Invoke_CollectorHasNoStates_ReturnsNodeUp()
        {
            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            _fixture.HttpResponse.Received(1).StatusCode = 200;
        }


        [Fact]
        public async Task Invoke_CollectorHasCriticalResourceDown_ReturnsNodeDown()
        {
            var state = Substitute.For<IResourceCurrentState>();
            state.IsUp.Returns(false);
            state.ResourceMonitor.IsCritical.Returns(true);
            _fixture.ResourceStateCollector.GetStates().Returns(new[] { state });

            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            _fixture.HttpResponse.Received(1).StatusCode = 503;
        }

        [Fact]
        public async Task Invoke_CollectorHasNonCriticalResourceDown_ReturnsNodeUp()
        {
            var state = Substitute.For<IResourceCurrentState>();
            state.IsUp.Returns(true);
            state.ResourceMonitor.IsCritical.Returns(false);
            _fixture.ResourceStateCollector.GetStates().Returns(new[] { state });

            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            _fixture.HttpResponse.Received(1).StatusCode = 200;
        }

        [Fact]
        public async Task Invoke_CollectorHasCriticalResourceUp_ReturnsNodeUp()
        {
            var state = Substitute.For<IResourceCurrentState>();
            state.IsUp.Returns(true);
            state.ResourceMonitor.IsCritical.Returns(true);
            _fixture.ResourceStateCollector.GetStates().Returns(new[] { state });

            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            _fixture.HttpResponse.Received(1).StatusCode = 200;
        }

        [Fact]
        public async Task Invoke_CollectorHasNonCriticalResourceUp_ReturnsNodeUp()
        {
            var state = Substitute.For<IResourceCurrentState>();
            state.IsUp.Returns(true);
            state.ResourceMonitor.IsCritical.Returns(false);
            _fixture.ResourceStateCollector.GetStates().Returns(new[] { state });

            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            _fixture.HttpResponse.Received(1).StatusCode = 200;
        }

        [Fact]
        public async Task Invoke_NoVersionService_ReturnsNodeUp()
        {
            _fixture.VersionService = null;

            var state = Substitute.For<IResourceCurrentState>();
            state.IsUp.Returns(true);
            _fixture.ResourceStateCollector.GetStates().Returns(new[] { state });

            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            _fixture.HttpResponse.Received(1).StatusCode = 200;
        }

        [Fact]
        public async Task Invoke_DetailedVersionInformation_ReturnsVersion()
        {
            var expected = new VersionInformation(
                assemblyFileVersion: "1.1.1.1",
                assemblyInformationalVersion: "some arbitrary informational version",
                environmentName: "Production",
                runtimeFramework: ".NETCoreApp,Version=v1.0",
                startupTimeUtc: DateTimeOffset.UtcNow
            );
            _fixture.VersionService.GetVersionInformation().Returns(expected);

            var state = Substitute.For<IResourceCurrentState>();
            state.IsUp.Returns(true);
            _fixture.ResourceStateCollector.GetStates().Returns(new[] { state });
            _fixture.HttpRequest.Query.ContainsKey("detailed").Returns(true);

            var sut = _fixture.GetSut();

            await sut.Invoke(_fixture.HttpContext);

            var actual = _fixture.ReadDetailedResponse()?.VersionInformation;
            Assert.NotNull(actual);
            Assert.Equal(expected.AssemblyFileVersion, actual.AssemblyFileVersion);
            Assert.Equal(expected.AssemblyInformationalVersion, actual.AssemblyInformationalVersion);
            Assert.Equal(expected.EnvironmentName, actual.EnvironmentName);
            Assert.Equal(expected.MachineName, actual.MachineName);
            Assert.Equal(expected.RuntimeFramework, actual.RuntimeFramework);
            Assert.Equal(expected.StartupTimeUtc, actual.StartupTimeUtc);
        }
    }
}
