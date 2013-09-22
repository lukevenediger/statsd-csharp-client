using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StatsdClient
{
  /// <summary>
  /// The statsd client library.
  /// </summary>
  [DebuggerDisplay("{host}:{port}")]
  public class Statsd : IStatsd
  {
    private string _prefix;
    private IOutputChannel _outputChannel;

    /// <summary>
    /// Creates a new instance of the Statsd client.
    /// </summary>
    /// <param name="host">The statsd or statsd.net server.</param>
    /// <param name="port"></param>
    public Statsd(string host, int port)
    {
      InitialiseInternal(() => new UdpOutputChannel(host, port), "", false);
    }

    /// <summary>
    /// Creates a new instance of the Statsd client.
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
    /// Creates a new instance of the Statsd client.
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
    /// Log a counter.
    /// </summary>
    /// <param name="name">The metric name.</param>
    /// <param name="count">The counter value (defaults to 1).</param>
    public void LogCount(string name, int count = 1)
    {
      SendMetric(MetricType.COUNT, name, _prefix, count);
    }

    /// <summary>
    /// Log a timing / latency
    /// </summary>
    /// <param name="name">The metric name.</param>
    /// <param name="milliseconds">The duration, in milliseconds, for this metric.</param>
    public void LogTiming(string name, int milliseconds)
    {
      SendMetric(MetricType.TIMING, name, _prefix, milliseconds);
    }

    /// <summary>
    /// Log a timing / latency
    /// </summary>
    /// <param name="name">The metric name.</param>
    /// <param name="milliseconds">The duration, in milliseconds, for this metric.</param>
    public void LogTiming(string name, long milliseconds)
    {
      LogTiming(name, (int)milliseconds);
    }

    /// <summary>
    /// Log a gauge.
    /// </summary>
    /// <param name="name">The metric name</param>
    /// <param name="value">The value for this gauge</param>
    public void LogGauge(string name, int value)
    {
      SendMetric(MetricType.GAUGE, name, _prefix, value);
    }

    /// <summary>
    /// Log to a set
    /// </summary>
    /// <param name="name">The metric name.</param>
    /// <param name="value">The value to log.</param>
    /// <remarks>Logging to a set is about counting the number
    /// of occurrences of each event.</remarks>
    public void LogSet(string name, int value)
    {
      SendMetric(MetricType.SET, name, _prefix, value);
    }

    /// <summary>
    /// Log a raw metric that will not get aggregated on the server.
    /// </summary>
    /// <param name="name">The metric name.</param>
    /// <param name="value">The metric value.</param>
    /// <param name="epoch">(optional) The epoch timestamp. Leave this blank to have the server assign an epoch for you.</param>
    public void LogRaw(string name, int value, long? epoch = null)
    {
      SendMetric(MetricType.RAW, name, String.Empty, value, epoch.HasValue ? epoch.ToString() : (string)null);
    }

    private void SendMetric(string metricType, string name, string prefix, int value, string postFix = null)
    {
      if (String.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException("name");
      }
      if (value < 0)
      {
        throw new ArgumentOutOfRangeException("value", value, "Cannot be less than zero.");
      }
      _outputChannel.Send(PrepareMetric(metricType, name, prefix, value, postFix));
    }

    /// <summary>
    /// Prepare a metric prior to sending it off ot the Graphite server.
    /// </summary>
    /// <param name="metricType"></param>
    /// <param name="name"></param>
    /// <param name="prefix"></param>
    /// <param name="value"></param>
    /// <param name="postFix">A value to append to the end of the line.</param>
    /// <returns>The formatted metric</returns>
    protected virtual string PrepareMetric(string metricType, string name, string prefix, int value, string postFix = null)
    {
      return (String.IsNullOrEmpty(prefix) ? name : (prefix + "." + name))
        + ":" + value
        + "|" + metricType
        + (postFix == null ? String.Empty : "|" + postFix);
    }
  }
}
