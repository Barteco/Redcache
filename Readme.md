# Redfish

Redfish is a strongly typed wrapper for Redis key-value store and pub-sub service, based on [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) library

## 1. Installation

### 1.1. Add packages
Install main package in your startup project:

`dotnet add package Redfish`

Add one of the following serialization packages (see "Choosing serialization method" section for more details):

`dotnet add package Redfish.Serialization.Protobuf`

`dotnet add package Redfish.Serialization.SystemTextJson`

`dotnet add package Redfish.Serialization.NewtonsoftJson`

Optionally, add:

`dotnet add package Redfish.Logging`

In multi-project solutions you can also add following package, which contains only required interfaces and dependencies:

`dotnet add package Redfish.Abstractions`

### 1.2. Configuration
*appsettings.json*
```
"Redfish": {
  "Redis": {
    "ConnectionString": "127.0.0.1:6379",
    "DefaultDatabase": 0
  },
  "Logging": { // optional section, only when using logging 
    "DefaultLevel": "Information"
  }
}
```

### 1.3. Registration
*Startup.cs*
```
using Redfish;

// ...

public void ConfigureServices(IServiceCollection services)
{
    // ...

    services.AddRedfish(Configuration.GetSection("Redfish:Redis"))
        .AddProtobufSerializer() // or any other chosen provider
        .AddLogging(Configuration.GetSection("Redfish:Logging")); // optional, only when using logging 
      
    // ...
}
```

## 2. Usage

### 2.1. Cache service

Getting value from Redis with fallback
```
public class HomeController : Controller
{
    private readonly IRedcache _redcache;

    public HomeController(IRedcache redcache)
    {
        _redcache = redcache;
    }

    [HttpGet("date")]
    public async Task<DateTime> GetDate()
    {
        return await _redcache.GetOrSet("key", () => DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }
}
```

### 2.2. Pub-sub service

Publishing messages to specified channel:
```
public class HomeController : Controller
{
    private readonly IRedqueue _redqueue;

    public HomeController(IRedqueue redqueue)
    {
        _redqueue = redqueue;
    }

    [HttpGet("publish")]
    public async Task<OkResult> Publish()
    {
        await _redqueue.Publish("channel", "message");

        return Ok();
    }
}
```

Subscribing channels:
```
public class ChannelSubscriber : BackgroundService
{
    private readonly IRedqueue _redqueue;

    public ChannelSubscriber(IRedqueue redqueue)
    {
        _redqueue = redqueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _redqueue.Subscribe<string>("channel", message =>
        {
            // handle your message
        });
    }
}
```

## 3. Choosing serialization method

For now, Redfish supports three methods of value serialization in Redis.

### 3.1. Protobuf

This method uses Mark Gravell's [protobuf-net](https://github.com/protobuf-net/protobuf-net) as a contract based binary serializer, which writes data in "protocol buffers" format. To use it, you must decorate all object persisted in Redis with `ProtoContract` and `ProtoMember(n)` attributes as follows:

```
[ProtoContract]
public class Person 
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; }
}
```

This format offers significant storage size reduction and faster serialization, but lags behind in readability when browsing Redis with external tools, like Redis Desktop Manager

#### 3.1.1. Installation

`dotnet add package Redfish.Serialization.Protobuf`

#### 3.1.2. Registration

```
services.AddRedfish(...)
    .AddProtobufSerializer();
```



### 3.2. System.Text.Json

This method uses `System.Text.Json`, default Microsoft's Json serializer for ASP.NET Core

#### 3.2.1. Installation

`dotnet add package Redfish.Serialization.SystemTextJson`

#### 3.2.2. Registration

```
services.AddRedfish(...)
    .AddSystemTextJsonSerializer();
```

### 3.3. Newtonsoft.Json

This method uses highly popular [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) as Json serializer

#### 3.3.1. Installation

`dotnet add package Redfish.Serialization.NewtonsoftJson`

#### 3.3.2. Registration

```
services.AddRedfish(...)
    .AddNewtonsoftJsonSerializer();
```

## 4. Logging

## 5. Acknowledgements

* [Icon](https://creativenerds.co.uk/freebies/80-free-wildlife-icons-the-best-ever-animal-icon-set/)