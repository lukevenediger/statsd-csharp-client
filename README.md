# statsd-csharp-client

A simple c# client library for [statsd.net](https://github.com/lukevenediger/statsd.net/) and [statsd](https://github.com/etsy/statsd/).

# Features
* Log counts, timings, gauges, sets and raw metrics
* Has an additional API that uses dynamics to create and submit stats
* Fault-tolerant client that can be configured to fail silently (with a warning) if misconfigured
* IStatsdClient interface for easy mocking in unit tests
* Allows for customisation of every output stat to do things like screen metrics before sending
* Supports a user-defined prefix to prepend to every metric
* Send metrics over a UDP or TCP connection

Coming soon:
* Support for count sampling and histograms
* batch-and-pump - collecting stats and sending them out in a batch at regular intervals
* Output to an HTTP endpoint

# Download and Install 
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
// Log a raw metric
statsd.LogRaw ("already.aggregated", 982, 1885837485 /* epoch timestamp */ );
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

## TCP Output Channel
Metrics can be delivered over a TCP connection by specifying ConnectionType.Tcp during construction:
```csharp
var statsd = new Statsd("localhost", 12001);
// Continue as normal
```

The connection will attempt to reconnect if something goes wrong, and will try three times before giving up. Use the retryOnDisconnect parameter to enable/disable this, and the retryAttempts parameter to specify the number of times to try the request again.

# Project Information

## Target Runtimes
* .Net 3.5
* .Net 4.0
* .Net 4.5

## Authors
Luke Venediger - lukev@lukev.net and [@lukevenediger](http://twitter.com/lukevenediger)

## See Also
* [statsd.net](https://github.com/lukevenediger/statsd.net/) 
* [statsd](https://github.com/etsy/statsd)
* [graphite](https://github.com/graphite-project)
* [StatsdCsharpClient on nuget.org](https://nuget.org/packages/StatsdCsharpClient/)
