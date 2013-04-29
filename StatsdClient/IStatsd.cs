using System;
namespace StatsdClient
{
  public interface IStatsd
  {
    /// <summary>
    /// Log a count for a metric
    /// </summary>
    void LogCount(string name, int count = 1);
    /// <summary>
    /// Log a gauge value
    /// </summary>
    void LogGauge(string name, int value);
    /// <summary>
    /// Log a latency / Timing
    /// </summary>
    void LogTiming(string name, int milliseconds);
    /// <summary>
    /// Log the number of unique occurrances of something
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void LogSet(string name, int value);
  }
}
