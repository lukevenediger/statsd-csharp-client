using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace StatsdClient
{
  /// <summary>
  /// A set of extensions for building up metrics using dynamic objects.
  /// </summary>
  public static class StatsdExtensions
  {
    /// <summary>
    /// Start logging a count
    /// </summary>
    public static dynamic count(this IStatsd statsd)
    {
      return new StatsBuilderInternal(statsd, MetricType.COUNT);
    }

    /// <summary>
    /// Start logging a timing / latency
    /// </summary>
    public static dynamic timing(this IStatsd statsd)
    {
      return new StatsBuilderInternal(statsd, MetricType.TIMING);
    }

    /// <summary>
    /// Start logging a gauge
    /// </summary>
    public static dynamic gauge(this IStatsd statsd)
    {
      return new StatsBuilderInternal(statsd, MetricType.GAUGE);
    }

    /// <summary>
    /// Start logging a set
    /// </summary>
    public static dynamic set(this IStatsd statsd)
    {
      return new StatsBuilderInternal(statsd, MetricType.SET);
    }

    private class StatsBuilderInternal : DynamicObject
    {
      private IStatsd _statsd;
      private List<string> _parts;
      private string _metricType;

      public StatsBuilderInternal(IStatsd statsd, string metricType)
      {
        _statsd = statsd;
        _parts = new List<string>();
        _metricType = metricType;
      }

      public override bool TryGetMember(GetMemberBinder binder, out object result)
      {
        if (binder.Name != "_")
        {
          _parts.Add(binder.Name);
        }
        result = this;
        return true;
      }

      public override bool TrySetMember(SetMemberBinder binder, object value)
      {
        return true;
      }

      public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
      {
        if (binder.Name == "_" && args.Length == 1)
        {
          _parts.Add(args[0].ToString());
          result = this;
          return true;
        }
        result = null;
        return false;
      }

      public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
      {
        if (binder.Operation == ExpressionType.AddAssign)
        {
          var quantity = (int)arg;
          var name = String.Join(".", _parts);
          switch (_metricType)
          {
            case MetricType.COUNT:
              _statsd.LogCount(name, quantity);
              break;
            case MetricType.GAUGE:
              _statsd.LogGauge(name, quantity);
              break;
            case MetricType.TIMING:
              _statsd.LogTiming(name, quantity);
              break;
            case MetricType.SET:
              _statsd.LogSet(name, quantity);
              break;
          }
          result = null;
          return true;
        }
        result = null;
        return false;
      }
    }
  }
}
