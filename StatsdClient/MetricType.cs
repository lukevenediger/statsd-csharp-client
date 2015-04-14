using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsdClient
{
  /// <summary>
  /// A list of metric types that statsd.net supports
  /// </summary>
  public static class MetricType
  {
    /// <summary>
    /// The number of times something happened.
    /// </summary>
    public const string COUNT = "c";
    /// <summary>
    /// The time it took for something to happen.
    /// </summary>
    public const string TIMING = "ms";
    /// <summary>
    /// The value of some measurement at this very moment.
    /// </summary>
    public const string GAUGE = "g";
    /// <summary>
    /// The number of times each event has been seen.
    /// </summary>
    public const string SET = "s";
    /// <summary>
    /// A raw metric that won't be aggregated on the server.
    /// </summary>
    public const string RAW = "r";
    /// <summary>
    /// A metric that calculates unique hits per hour, day, day-of-week, week or month
    /// </summary>
    public const string CALENDARGRAM = "cg";
  }
}
