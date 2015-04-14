using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StatsdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsdClientTests
{
  [TestClass]
  public class StatsdExtensionsTests
  {
    private Mock<IOutputChannel> _outputChannel;
    private Statsd _statsd;
    private TestData _testData;

    [TestInitialize]
    public void Initialise()
    {
      _outputChannel = new Mock<IOutputChannel>();
      _statsd = new Statsd("localhost", 12000, outputChannel : _outputChannel.Object);
      _testData = new TestData();
    }

    [TestMethod]
    public void count_SendToStatsd_Success()
    {
      _outputChannel.Setup(p => p.Send("foo.bar:1|c")).Verifiable();
      _statsd.count().foo.bar += 1;
      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void gauge_SendToStatsd_Success()
    {
      _outputChannel.Setup(p => p.Send("foo.bar:1|g")).Verifiable();
      _statsd.gauge().foo.bar += 1;
      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void timing_SendToStatsd_Success()
    {
      _outputChannel.Setup(p => p.Send("foo.bar:1|ms")).Verifiable();
      _statsd.timing().foo.bar += 1;
      _outputChannel.VerifyAll();
    }

    [TestMethod]
    public void count_AddNamePartAsString_Success()
    {
      _outputChannel.Setup(p => p.Send("foo.bar:1|ms")).Verifiable();
      _statsd.timing().foo._("bar")._ += 1;
      _outputChannel.VerifyAll();
    }
  }
  
}
