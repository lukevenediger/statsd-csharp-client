# statsd-csharp-client Changelog

## v1.4.0.0
* Added async support

## v1.3.0.0
* Added support for Calendargrams

## v1.2.1.0
* Fixed a bug in the tcp output channel's retry logic
* Skip DNS resolution on the UDP client if already an IP Address
* Fall back to the Null Output Channel if the client is created with an empty host name.

## v1.2.0.0
* Support the Raw metric format
* A few more unit tests
* Fixed a bug where you couldn't start up if the host could not be resolved

## v1.1.0.0
* Added a TCP output channel
* Added a simpler default constructor for connecting via UDP

## v1.0.1.0
* Added support for .net 3.5 and .net 4.0 users

## v1.0.0.0
* First upload of the client library
* Supports counts, gauges and timers
* Can output to any statsd-compatible service