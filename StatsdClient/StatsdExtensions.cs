﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

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
      private static Dictionary<Type, BinaryOperationHandler> _handlerMap =
                new Dictionary<Type, BinaryOperationHandler>();

      private delegate void BinaryOperationHandler(IStatsd client, string metricType, string name, object arg);

      static StatsBuilderInternal()
      {
        _handlerMap.Add(typeof(int), IntHandler);
        _handlerMap.Add(typeof(decimal), DecimalHandler);
        _handlerMap.Add(typeof(double), DoubleHandler);
        _handlerMap.Add(typeof(float), DoubleHandler);
      }

      private static void IntHandler(IStatsd client, string metricType, string name, object arg)
      {
          var value = Convert.ToInt32(arg);
          switch (metricType)
          {
            case MetricType.COUNT:
              client.LogCount(name, value);
              break;
            case MetricType.GAUGE:
              client.LogGauge(name, value);
              break;
            case MetricType.TIMING:
              client.LogTiming(name, value);
              break;
            case MetricType.SET:
              client.LogSet(name, value);
              break;
          }
      }

      private static void DoubleHandler(IStatsd client, string metricType, string name, object arg)
      {
          var value = Convert.ToDouble(arg);
          switch (metricType)
          {
            case MetricType.COUNT:
              throw new NotSupportedException();
            case MetricType.GAUGE:
              client.LogGauge(name, value);
              break;
            case MetricType.TIMING:
              throw new NotSupportedException();
            case MetricType.SET:
              throw new NotSupportedException();
          }
      }

      private static void DecimalHandler(IStatsd client, string metricType, string name, object arg)
      {
          var value = Convert.ToDecimal(arg);
          switch (metricType)
          {
            case MetricType.COUNT:
              throw new NotSupportedException();
            case MetricType.GAUGE:
              client.LogGauge(name, value);
              break;
            case MetricType.TIMING:
              throw new NotSupportedException();
            case MetricType.SET:
              throw new NotSupportedException();
          }
      }

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
          var argType = arg.GetType();
          var name = String.Join(".", _parts);

          if (!_handlerMap.ContainsKey(argType))
          {
            throw new ApplicationException("Handler not supported for type: " + argType);
          }

          var handler = _handlerMap[argType];
          handler(_statsd, _metricType, name, arg);

          result = null;
          return true;
        }
        result = null;
        return false;
      }
    }
  }
}