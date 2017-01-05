using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StatsdClient;

namespace StatsdClientTests {
    [TestClass]
    public class StatsdExtensionsTests {
        private Mock<IOutputChannel> _outputChannel;
        private Statsd _statsd;

        [TestInitialize]
        public void Initialise() {
            _outputChannel = new Mock<IOutputChannel>();
            _statsd = new Statsd("localhost", 12000, outputChannel: _outputChannel.Object);
        }

        [TestMethod]
        public async Task count_SendToStatsd_Success() {
            _outputChannel.Setup(p => p.SendAsync("foo.bar:1|c")).Returns(Task.FromResult(false)).Verifiable();
            _statsd.count().foo.bar += 1;
            _outputChannel.VerifyAll();
        }

        [TestMethod]
        public void gauge_SendToStatsd_Success() {
            _outputChannel.Setup(p => p.SendAsync("foo.bar:1|g")).Returns(Task.FromResult(false)).Verifiable();
            _statsd.gauge().foo.bar += 1;
            _outputChannel.VerifyAll();
        }

        [TestMethod]
        public void gauge_SendFloatToStatsd_Success() {
            _outputChannel.Setup(p => p.SendAsync("foo.bar:1.5|g")).Returns(Task.FromResult(false)).Verifiable();
            _statsd.gauge().foo.bar += 1.5f;
            _outputChannel.VerifyAll();
        }

        [TestMethod]
        public void gauge_SendDoubleToStatsd_Success() {
            _outputChannel.Setup(p => p.SendAsync("foo.bar:2.5|g")).Returns(Task.FromResult(false)).Verifiable();
            _statsd.gauge().foo.bar += 2.5d;
            _outputChannel.VerifyAll();
        }

        [TestMethod]
        public void gauge_SendDecimalToStatsd_Success() {
            _outputChannel.Setup(p => p.SendAsync("foo.bar:2.5|g")).Returns(Task.FromResult(false)).Verifiable();
            _statsd.gauge().foo.bar += 2.5M;
            _outputChannel.VerifyAll();
        }

        [TestMethod]
        public void timing_SendToStatsd_Success() {
            _outputChannel.Setup(p => p.SendAsync("foo.bar:1|ms")).Returns(Task.FromResult(false)).Verifiable();
            _statsd.timing().foo.bar += 1;
            _outputChannel.VerifyAll();
        }

        [TestMethod]
        public void count_AddNamePartAsString_Success() {
            _outputChannel.Setup(p => p.SendAsync("foo.bar:1|ms")).Returns(Task.FromResult(false)).Verifiable();
            _statsd.timing().foo._("bar")._ += 1;
            _outputChannel.VerifyAll();
        }
    }
}