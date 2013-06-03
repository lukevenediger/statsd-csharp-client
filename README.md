# statsd-csharp-client

A simple c# client library for [statsd.net](https://github.com/lukevenediger/statsd.net/) and [statsd](https://github.com/etsy/statsd/).

# Features
* Log counts, timings and gauges
* Has an additional API that uses dynamics to create and submit stats
* Fault-tolerant client that can be configured to fail silently (with a warning) if misconfigured
* IStatsdClient interface for easy mocking in unit tests
* Allows for customisation of every output stat to do things like screen metrics before sending
* Supports a user-defined prefix to prepend to every metric

Coming soon:
* Support for sets and count sampling
* batch-and-pump - collecting stats and sending them out in a batch at regular intervals
* Output to an HTTP endpoint

# Installation via Nuget
Install the [StatsdCsharpClient](https://nuget.org/packages/StatsdCsharpClient/) via nuget:
```bash
PM> Install-Package StatsdCsharpClient
```

# Quickstart
Assuming your server is running on localhost and listening on port 12000:
```csharp
using StatsdClient;
...
var statsd = new Statsd("localhost", 12000);
// Log a count
statsd.LogCount( "site.hits" );
// Log a gauge
statsd.LogGauge( "site.activeUsers", numActiveUsers );
// Log a timing
statsd.LogTiming( "site.pageLoad", 100 /* milliseconds */ );
```

You can also wrap your code in a `using` block to measure the latency by using the LogTiming(string) extension method:
```csharp
using StatsdClient;
...
using (statsd.LogTiming( "site.db.fetchReport" ))
{
  // do some work
}
// At this point your latency has been sent to the server
```

## Dynamic Stats Builder
There's also a nifty set of extension methods that let you define your stats without using strings. Using the example provided above, but now using the builder:
```csharp
var statsd = new StatsdClient("localhost", 12000);
// Log a count
statsd.count.site.hits += 1;
// Log a gauge
statsd.gauge.site.activeUsers += numActiveUsers;
// Log a timing
statsd.site.pageLoad += 100; /* milliseconds */
```

# Project Information

## Target Runtimes
* .Net 3.5
* .Net 4.0
* .Net 4.5

## Authors
Luke Venediger - lukev@lukev.net

## See Also
* [statsd.net](https://github.com/lukevenediger/statsd.net/) 
* [statsd](https://github.com/etsy/statsd)
* [graphite](https://github.com/graphite-project)
* [StatsdCsharpClient on nuget.org](https://nuget.org/packages/StatsdCsharpClient/)
