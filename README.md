# Serilog.Sinks.LogDNA #

Serilog Sink that sends log events to LogDNA https://logdna.com

**Package** - [Serilog.Sinks.LogDNA](http://nuget.org/packages/serilog.sinks.logdna) | **Platforms** - .NET 4.5, netstandard2.0

Example:
```csharp
var log = new LoggerConfiguration()
    .WriteTo.LogDNA(apiKey: "<API_KEY>", appName: "myapp")
    .CreateLogger();

var position = new { Latitude = 25, Longitude = 134 };
var elapsedMs = 34;
log.Information("Processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);
```

Prints to LogDNA console:
```
Oct 10 16:09:13 desktop-r9hnrih myapp Information Information Processed { Latitude: 25, Longitude: 134 } in 034 ms.
```

[![Nuget](https://img.shields.io/nuget/v/serilog.sinks.logdna.svg)](https://www.nuget.org/packages/Serilog.Sinks.LogDNA/)