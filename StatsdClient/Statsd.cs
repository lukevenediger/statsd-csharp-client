using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StatsdClient
{
    /// <summary>
    ///     The statsd client library.
    /// </summary>
    public class Statsd : IStatsd
    {
        private string _prefix;
        private IOutputChannel _outputChannel;

        /// <summary>
        ///     Creates a new instance of the Statsd client.
        /// </summary>
        /// <param name="host">The statsd or statsd.net server.</param>
        /// <param name="port"></param>
        public Statsd(string host, int port)
        {
            if (String.IsNullOrEmpty(host))
            {
                Trace.TraceWarning("Statsd client initialised with empty host address. Dropping back to NullOutputChannel.");
                InitialiseInternal(() => new NullOutputChannel(), "", false);
            }
            else
            {
                InitialiseInternal(() => new UdpOutputChannel(host, port), "", false);
            }
        }

        /// <summary>
        ///     Creates a new instance of the Statsd client.
        /// </summary>
        /// <param name="host">The statsd or statsd.net server.</param>
        /// <param name="port"></param>
        /// <param name="prefix">A string prefix to prepend to every metric.</param>
        /// <param name="rethrowOnError">If True, rethrows any exceptions caught due to bad configuration.</param>
        /// <param name="connectionType">Choose between a UDP (recommended) or TCP connection.</param>
        /// <param name="retryOnDisconnect">Retry the connection if it fails (TCP only).</param>
        /// <param name="retryAttempts">Number of times to retry before giving up (TCP only).</param>
        public Statsd(string host,
            int port,
            ConnectionType connectionType = ConnectionType.Udp,
            string prefix = null,
            bool rethrowOnError = false,
            bool retryOnDisconnect = true,
            int retryAttempts = 3)
        {
            InitialiseInternal(() =>
            {
                return connectionType == ConnectionType.Tcp
                    ? (IOutputChannel)new TcpOutputChannel(host, port, retryOnDisconnect, retryAttempts)
                    : (IOutputChannel)new UdpOutputChannel(host, port);
            },
                prefix,
                rethrowOnError);
        }

        /// <summary>
        ///     Creates a new instance of the Statsd client.
        /// </summary>
        /// <param name="host">The statsd or statsd.net server.</param>
        /// <param name="port"></param>
        /// <param name="prefix">A string prefix to prepend to every metric.</param>
        /// <param name="rethrowOnError">If True, rethrows any exceptions caught due to bad configuration.</param>
        /// <param name="outputChannel">Optional output channel (useful for mocking / testing).</param>
        public Statsd(string host, int port, string prefix = null, bool rethrowOnError = false, IOutputChannel outputChannel = null)
        {
            if (outputChannel == null)
            {
                InitialiseInternal(() => new UdpOutputChannel(host, port), prefix, rethrowOnError);
            }
            else
            {
                InitialiseInternal(() => outputChannel, prefix, rethrowOnError);
            }
        }

        private void InitialiseInternal(Func<IOutputChannel> createOutputChannel, string prefix, bool rethrowOnError)
        {
            _prefix = prefix;
            if (_prefix != null && _prefix.EndsWith("."))
            {
                _prefix = _prefix.Substring(0, _prefix.Length - 1);
            }
            try
            {
                _outputChannel = createOutputChannel();
            }
            catch (Exception ex)
            {
                if (rethrowOnError)
                {
                    throw;
                }
                Trace.TraceError("Could not initialise the Statsd client: {0} - falling back to NullOutputChannel.", ex.Message);
                _outputChannel = new NullOutputChannel();
            }
        }

        /// <summary>
        ///     Log a counter.
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="count">The counter value (defaults to 1).</param>
        public async Task LogCountAsync(string name, long count = 1)
        {
            await SendMetricAsync(MetricType.COUNT, name, _prefix, count);
        }

        /// <summary>
        ///     Log a timing / latency
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="milliseconds">The duration, in milliseconds, for this metric.</param>
        public async Task LogTimingAsync(string name, long milliseconds)
        {
            await SendMetricAsync(MetricType.TIMING, name, _prefix, milliseconds);
        }

        /// <summary>
        ///     Log a gauge.
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The value for this gauge</param>
        public async Task LogGaugeAsync(string name, long value)
        {
            await SendMetricAsync(MetricType.GAUGE, name, _prefix, value);
        }

        /// <summary>
        ///     Log a gauge.
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The value for this gauge</param>
        public async Task LogGaugeAsync(string name, double value) {
            await SendMetricAsync(MetricType.GAUGE, name, _prefix, value);
        }

        /// <summary>
        ///     Log a gauge.
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The value for this gauge</param>
        public async Task LogGaugeAsync(string name, decimal value) {
            await SendMetricAsync(MetricType.GAUGE, name, _prefix, value);
        }

        /// <summary>
        ///     Log to a set
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The value to log.</param>
        /// <remarks>
        ///     Logging to a set is about counting the number
        ///     of occurrences of each event.
        /// </remarks>
        public async Task LogSetAsync(string name, long value)
        {
            await SendMetricAsync(MetricType.SET, name, _prefix, value);
        }

        /// <summary>
        ///     Log a raw metric that will not get aggregated on the server.
        /// </summary>
        /// <param name="name">The metric name.</param>
        /// <param name="value">The metric value.</param>
        /// <param name="epoch">(optional) The epoch timestamp. Leave this blank to have the server assign an epoch for you.</param>
        public async Task LogRawAsync(string name, long value, long? epoch = null)
        {
            await SendMetricAsync(MetricType.RAW, name, String.Empty, value, epoch.HasValue ? epoch.ToString() : null);
        }

        /// <summary>
        ///     Log a calendargram metric
        /// </summary>
        /// <param name="name">The metric namespace</param>
        /// <param name="value">The unique value to be counted in the time period</param>
        /// <param name="period">The time period, can be one of h,d,dow,w,m</param>
        public async Task LogCalendargramAsync(string name, string value, string period)
        {
            await SendMetricAsync(MetricType.CALENDARGRAM, name, _prefix, value, period);
        }

        /// <summary>
        ///     Log a calendargram metric
        /// </summary>
        /// <param name="name">The metric namespace</param>
        /// <param name="value">The unique value to be counted in the time period</param>
        /// <param name="period">The time period, can be one of h,d,dow,w,m</param>
        public async Task LogCalendargramAsync(string name, long value, string period)
        {
            await SendMetricAsync(MetricType.CALENDARGRAM, name, _prefix, value, period);
        }

        private async Task SendMetricAsync(string metricType, string name, string prefix, long value, string postFix = null)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", name, value);
                return;
            }

            await SendMetricAsync(metricType, name, prefix, value.ToString(), postFix);
        }

        private async Task SendMetricAsync(string metricType, string name, string prefix, double value, string postFix = null)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", name, value);
                return;
            }

            await SendMetricAsync(metricType, name, prefix, value.ToString(), postFix);
        }

        private async Task SendMetricAsync(string metricType, string name, string prefix, decimal value, string postFix = null)
        {
            if (value < 0)
            {
                Trace.TraceWarning("Metric value for {0} was less than zero: {1}. Not sending.", name, value);
                return;
            }

            await SendMetricAsync(metricType, name, prefix, value.ToString(), postFix);
        }

        private async Task SendMetricAsync(string metricType, string name, string prefix, string value, string postFix = null)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            await _outputChannel.SendAsync(PrepareMetric(metricType, name, prefix, value, postFix));
        }

        /// <summary>
        ///     Prepare a metric prior to sending it off ot the Graphite server.
        /// </summary>
        /// <param name="metricType"></param>
        /// <param name="name"></param>
        /// <param name="prefix"></param>
        /// <param name="value"></param>
        /// <param name="postFix">A value to append to the end of the line.</param>
        /// <returns>The formatted metric</returns>
        protected virtual string PrepareMetric(string metricType, string name, string prefix, string value, string postFix = null)
        {
            return (String.IsNullOrEmpty(prefix) ? name : (prefix + "." + name))
                   + ":" + value
                   + "|" + metricType
                   + (postFix == null ? String.Empty : "|" + postFix);
        }
    }
}