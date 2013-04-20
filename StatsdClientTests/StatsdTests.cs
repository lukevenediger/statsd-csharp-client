using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatsdClient;
using Moq;

namespace StatsdClientTests
{
  [TestClass]
  public class StatsdTests
  {
    private Mock<IOutputChannel> _outputChannel;
    private Statsd _statsd;
    private TestData _testData;

    public StatsdTests()
    {
      _testData = new TestData();
    }

    [TestInitialize]
    public void Initialise()
    {
      _outputChannel = new Mock<IOutputChannel>();
      _statsd = new Statsd("localhost", 12000, outputChannel : _outputChannel.Object);
    }

    #region Parameter Checks
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void LogCount_NameIsNull_ExpectArgumentNullException()
    {
      _statsd.LogCount(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void LogCount_ValueIsLessThanZero_ExpectArgumentOutOfRangeException()
    {
      _statsd.LogCount("foo", -1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void LogGauge_NameIsNull_ExpectArgumentNullException()
    {
      _statsd.LogGauge(null, _testData.NextInteger);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void LogGauge_ValueIsLessThanZero_ExpectArgumentOutOfRangeException()
    {
      _statsd.LogGauge("foo", -1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void LogTiming_NameIsNull_ExpectArgumentNullException()
    {
      _statsd.LogTiming(null, _testData.NextInteger);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void LogTiming_ValueIsLessThanZero_ExpectArgumentOutOfRangeException()
    {
      _statsd.LogTiming("foo", -1);
    }
    #endregion

    [TestMethod]
    public void LogCount_ValidInput_Success()
    {
      var stat = _testData.NextStatName;
      var count = _testData.NextInteger;
      _outputChannel.Setup(p => p.Send(stat + ":" + count.ToString() + "|c")).Verifiable();

      _statsd.LogCount(stat, count);

      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void LogTiming_ValidInput_Success()
    {
      var stat = _testData.NextStatName;
      var count = _testData.NextInteger;
      _outputChannel.Setup(p => p.Send(stat + ":" + count.ToString() + "|ms")).Verifiable();

      _statsd.LogTiming(stat, count);

      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void LogGauge_ValidInput_Success()
    {
      var stat = _testData.NextStatName;
      var count = _testData.NextInteger;
      _outputChannel.Setup(p => p.Send(stat + ":" + count.ToString() + "|g")).Verifiable();

      _statsd.LogGauge(stat, count);

      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void Constructor_PrefixEndsInPeriod_RemovePeriod()
    {
      var statsd = new Statsd("localhost", 12000, "foo.", outputChannel : _outputChannel.Object);
      var stat = _testData.NextStatName;
      var count = _testData.NextInteger;
      _outputChannel.Setup(p => p.Send("foo." + stat + ":" + count.ToString() + "|c")).Verifiable();

      statsd.LogCount(stat, count);

      _outputChannel.VerifyAll();
    }
  }
}
