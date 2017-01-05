using System;

namespace StatsdClient
{
    /// <summary>
    /// A set of extensions for use with the StatsdClient library.
    /// </summary>
    public static class StatsdClientExtensions
    {
        /// <summary>
        /// Log a counter.
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The metric name.</param>
        /// <param name="count">The counter value (defaults to 1).</param>
        public static void LogCount(this IStatsd client, string name, int count = 1)
        {
            client.LogCountAsync(name, count).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Log a timing / latency
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The metric name.</param>
        /// <param name="milliseconds">The duration, in milliseconds, for this metric.</param>
        public static void LogTiming(this IStatsd client, string name, long milliseconds)
        {
            client.LogTimingAsync(name, milliseconds).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Log a gauge.
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The metric name</param>
        /// <param name="value">The value for this gauge</param>
        public static void LogGauge(this IStatsd client, string name, int value)
        {
            client.LogGaugeAsync(name, value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Log a gauge.
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The metric name</param>
        /// <param name="value">The value for this gauge</param>
        public static void LogGauge(this IStatsd client, string name, double value) {
            client.LogGaugeAsync(name, value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Log a gauge.
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The metric name</param>
        /// <param name="value">The value for this gauge</param>
        public static void LogGauge(this IStatsd client, string name, decimal value) {
            client.LogGaugeAsync(name, value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Log to a set
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The value to log.</param>
        /// <remarks>
        /// Logging to a set is about counting the number of occurrences of each event.
        /// </remarks>
        public static void LogSet(this IStatsd client, string name, int value)
        {
            client.LogSetAsync(name, value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Log a raw metric that will not get aggregated on the server.
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The metric value.</param>
        /// <param name="epoch">(optional) The epoch timestamp. Leave this blank to have the server assign an epoch for you.</param>
        public static void LogRaw(this IStatsd client, string name, int value, long? epoch = null)
        {
            client.LogRawAsync(name, value, epoch).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Log a calendargram metric
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The metric namespace</param>
        /// <param name="value">The unique value to be counted in the time period</param>
        /// <param name="period">The time period, can be one of h,d,dow,w,m</param>
        public static void LogCalendargram(this IStatsd client, string name, string value, string period)
        {
            client.LogCalendargramAsync(name, value, period).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Log a calendargram metric
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The metric namespace</param>
        /// <param name="value">The unique value to be counted in the time period</param>
        /// <param name="period">The time period, can be one of h,d,dow,w,m</param>
        public static void LogCalendargram(this IStatsd client, string name, long value, string period)
        {
            client.LogCalendargramAsync(name, value, period).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Log a timing metric
        /// </summary>
        /// <param name="client">The statsd client instance.</param>
        /// <param name="name">The namespace of the timing metric.</param>
        /// <param name="duration">The duration to log (will be converted into milliseconds)</param>
        public static void LogTiming(this IStatsd client, string name, TimeSpan duration)
        {
            client.LogTiming(name, (long)duration.TotalMilliseconds);
        }

        /// <summary>
        /// Starts a timing metric that will be logged when the TimingToken is disposed.
        /// </summary>
        /// <param name="client">The statsd clien instance.</param>
        /// <param name="name">The namespace of the timing metric.</param>
        /// <returns>A timing token that has been initialised with a start datetime.</returns>
        /// <remarks>
        /// Wrap the code you want to measure in a using() {} block. The TimingToken instance will log the duration when it is disposed.
        /// </remarks>
        public static TimingToken LogTiming(this IStatsd client, string name)
        {
            return new TimingToken(client, name);
        }
    }
}