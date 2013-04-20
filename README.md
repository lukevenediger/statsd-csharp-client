# statsd-csharp-client

A simple c# client library for [statsd.net](https://github.com/lukevenediger/statsd.net/) and [statsd](https://github.com/etsy/statsd/).

## Features
* Log counts, timings and gauges
* Has an additional API that uses dynamics to create and submit stats
* Fault-tolerant client that can be configured to fail silently (with a warning) if misconfigured
* IStatsdClient interface for easy mocking in unit tests
* Allows for customisation of every output stat to do things like screen metrics before sending
* Supports a user-defined prefix to prepend to every metric

## Coming soon
* batch-and-pump - collecting stats and sending them out in a batch at regular intervals
* Output to an HTTP endpoint

## Usage

Create an instance of StatsdClient to get started:
```csharp
var statsd = new StatsdClient(

You can specify your metrics as a string:
```csharp
statsd.LogCount( "site.hits" );
statsd.LogTiming( "site.pageLoad", 100 /* milliseconds */ );
statsd.LogGauge( "site.activeUsers", numActiveUsers );
```

You can also use the dynamic stats builder:

```csharp
_.count.site.hits + 1 > statsd;
_.timing.site.pageLoad + 100 > statsd;
_.gauge.site.activeUsers + numActiveUsers > statsd;
'''

[statsd]: https://github.com/etsy/statsd
[statsd.net]: https://github.com/lukevenediger/statsd.net