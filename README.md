# Greentube.Monitoring 

Greentube.Monitoring is a library that simplifies monitoring dependencies that affect your application's availability.

It allows you to add one or more _resource monitors_  and verify with a single call if any _critical_ resource is down, which in turn makes reports your application as unavailable.

With a few lines of code you get a health check endpoint to reply to a load balancer or a monitoring tool that your application is ready to take work load or not.

Monitors can be simple delegates or one of the supported:

* Redis
* MongoDB
* HTTP
* SQL Server
* ActiveMQ

Example for monitoring both Redis and MongoDB:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMonitoring(options => {
        options.AddRedisMonitor();
        options.AddMongoDbMonitor();
    });
}
```
Or your own resource monitor:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMonitoring(provider => {
        var dep = provider.GetRequiredService<IMyServiceMonitor>();
        dep.StartMonitor();
    });
}
```
With the ASP.NET Core Health Check middleware, the state can be reported like:
```csharp
public void Configure(IApplicationBuilder app){
    app.UseHealthEndpoint()
}
```
Verifying the state of your service:
```console
$ curl -D - http://localhost:5000/health
HTTP/1.1 200 OK
Date: Mon, 12 Jun 2017 13:37:35 GMT
Content-Length: 11
Content-Type: application/json
Server: Kestrel

{"Up":true}
```
Or when a _critical_ resource is down:
```console
$ curl -D - http://localhost:5000/health
HTTP/1.1 503 Service Unavailable
Date: Mon, 12 Jun 2017 13:40:42 GMT
Content-Length: 12
Content-Type: application/json
Server: Kestrel

{"Up":false}
```

Each of those monitors is a separate NuGet package (pay for play). If you don't want use the ASP.NET Core middleware but want to use MVC to report the state in a different way:

```csharp
[Route("health")]
public class HealthController : Controller
{
    private readonly IResourceStateCollector _collector;

    public HealthController(IResourceStateCollector collector)
    {
        _collector = collector;
    }

    [HttpGet]
    public IEnumerable<object> AllResourceStates()
    {
        return from rm in _collector.GetStates()
               select new
               {
                   rm.ResourceMonitor.ResourceName,
                   rm.IsUp
               };
    }

    [HttpGet]
    [Route("non-critical/name")]
    public IEnumerable<string> NonCriticalResourcesName()
    {
        return from rm in _collector.GetStates()
               where !rm.ResourceMonitor.IsCritical
               select rm.ResourceMonitor.ResourceName;
    }
}
```

# License

Licensed under MIT