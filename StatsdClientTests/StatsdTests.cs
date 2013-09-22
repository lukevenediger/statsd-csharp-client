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

    [TestMethod]
    public void LogCount_NullPrefix_DoesNotStartNameWithPeriod()
    {
      var statsd = new Statsd("localhost", 12000, prefix : null, outputChannel : _outputChannel.Object);
      var inputStat = "some.stat:1|c";
      _outputChannel.Setup(p => p.Send(It.Is<string>(q => q == inputStat)))
        .Verifiable();
      statsd.LogCount("some.stat");
      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void LogCount_EmptyStringPrefix_DoesNotStartNameWithPeriod()
    {
      var statsd = new Statsd("localhost", 12000, prefix : "", outputChannel : _outputChannel.Object);
      var inputStat = "some.stat:1|c";
      _outputChannel.Setup(p => p.Send(It.Is<string>(q => q == inputStat)))
        .Verifiable();
      statsd.LogCount("some.stat");
      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void LogRaw_WithoutEpoch_Valid()
    {
      var statsd = new Statsd("localhost", 12000, prefix : "", outputChannel : _outputChannel.Object);
      var inputStat = "my.raw.stat:12934|r";
      _outputChannel.Setup(p => p.Send(It.Is<String>(q => q == inputStat)))
        .Verifiable();
      statsd.LogRaw("my.raw.stat", 12934);
      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void LogRaw_WithEpoch_Valid()
    {
      var statsd = new Statsd("localhost", 12000, prefix : "", outputChannel : _outputChannel.Object);
      var almostAnEpoch = DateTime.Now.Ticks;
      var inputStat = "my.raw.stat:12934|r|" + almostAnEpoch;
      _outputChannel.Setup(p => p.Send(It.Is<String>(q => q == inputStat)))
        .Verifiable();
      statsd.LogRaw("my.raw.stat", 12934, almostAnEpoch);
      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void CreateClient_WithInvalidHostName_DoesNotError()
    {
      var statsd = new Statsd("nowhere.here.or.anywhere", 12000);
      statsd.LogCount("test.stat");
    }
  }
}
