using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StatsdClient
{
    internal sealed class TcpOutputChannel : IOutputChannel
    {
        private readonly TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly string _host;
        private readonly int _port;
        private readonly bool _reconnectEnabled;
        private readonly int _retryAttempts;
        private readonly AsyncLock _asyncLock;

        public TcpOutputChannel(string host, int port, bool reconnectEnabled = true, int retryAttempts = 3)
        {
            _host = host;
            _port = port;
            _reconnectEnabled = reconnectEnabled;
            _retryAttempts = retryAttempts;
            _tcpClient = new TcpClient();
            _asyncLock = new AsyncLock();
        }

        public async Task SendAsync(string line)
        {
            await SendWithRetryAsync(line, _reconnectEnabled ? _retryAttempts - 1 : 0);
        }

        private async Task SendWithRetryAsync(string line, int attemptsLeft)
        {
            string errorMessage = null;
            try
            {
                if (!_tcpClient.Connected)
                {
                    await RestoreConnectionAsync();
                }

                var bytesToSend = Encoding.UTF8.GetBytes(line + Environment.NewLine);
                await _stream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
            }
            catch (IOException ex)
            {
                errorMessage = string.Format("Sending metrics via TCP failed with an IOException: {0}", ex.Message);
            }
            catch (SocketException ex)
            {
                // No more attempts left, so log it and continue
                errorMessage = string.Format("Sending metrics via TCP failed with a SocketException: {0}, code: {1}", ex.Message, ex.SocketErrorCode);
            }

            if (errorMessage != null)
            {
                if (attemptsLeft > 0)
                {
                    await SendWithRetryAsync(line, --attemptsLeft);
                }
                else
                {
                    // No more attempts left, so log it and continue
                    Trace.TraceWarning(errorMessage);
                }
            }
        }

        private async Task RestoreConnectionAsync()
        {
            if (!_tcpClient.Connected)
            {
                using (await _asyncLock.LockAsync())
                {
                    if (!_tcpClient.Connected)
                    {
                        await _tcpClient.ConnectAsync(_host, _port);
                        _stream = _tcpClient.GetStream();
                    }
                }
            }
        }
    }
}