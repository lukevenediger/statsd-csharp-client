using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsdClient
{
  /// <summary>
  /// A set of extensions for use with the StatsdClient library.
  /// </summary>
  public static class StatsdClientExtensions
  {
    /// <summary>
    /// Log a timing metric
    /// </summary>
    /// <param name="client">The statsd client instance.</param>
    /// <param name="name">The namespace of the timing metric.</param>
    /// <param name="duration">The duration to log (will be converted into milliseconds)</param>
    public static void LogTiming(this IStatsd client, string name, TimeSpan duration)
    {
      client.LogTiming(name, (int)duration.TotalMilliseconds);
    }

    /// <summary>
    /// Starts a timing metric that will be logged when the TimingToken is disposed.
    /// </summary>
    /// <param name="client">The statsd clien instance.</param>
    /// <param name="name">The namespace of the timing metric.</param>
    /// <returns>A timing token that has been initialised with a start datetime.</returns>
    /// <remarks>Wrap the code you want to measure in a using() {} block. The 
    /// TimingToken instance will log the duration when it is disposed.</remarks>
    public static TimingToken LogTiming(this IStatsd client, string name)
    {
      return new TimingToken(client, name);
    }
  }
}
