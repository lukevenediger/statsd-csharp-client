using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace StatsdClient
{
  internal sealed class TcpOutputChannel : IOutputChannel
  {
    private TcpClient _tcpClient;
    private NetworkStream _stream;
    private object _reconnectLock;
    private string _host;
    private int _port;
    private bool _reconnectEnabled;
    private int _retryAttempts;

    public TcpOutputChannel(string host, int port, bool reconnectEnabled = true, int retryAttempts = 3)
    {
      _host = host;
      _port = port;
      _reconnectEnabled = reconnectEnabled;
      _retryAttempts = retryAttempts;
      _tcpClient = new TcpClient();
      _reconnectLock = new object();
    }

    public void Send(string line)
    {
      SendWithRetry(line, _reconnectEnabled ? _retryAttempts - 1 : 0);
    }

    private void SendWithRetry(string line, int attemptsLeft)
    {
      try
      {
        if ( !_tcpClient.Connected )
        {
          RestoreConnection();
        }
        var bytesToSend = Encoding.UTF8.GetBytes( line + Environment.NewLine );
        _stream.Write( bytesToSend, 0, bytesToSend.Length );
      }
      catch ( IOException ex )
      {
        if ( attemptsLeft > 0 )
        {
          SendWithRetry( line, --attemptsLeft );
        }
        else
        {
          // No more attempts left, so log it and continue
          Trace.TraceWarning( "Sending metrics via TCP failed with an IOException: {0}", ex.Message );
        }
      }
      catch ( SocketException ex )
      {
        if ( attemptsLeft > 0 )
        {
          SendWithRetry( line, --attemptsLeft );
        }
        else
        {
          // No more attempts left, so log it and continue
          Trace.TraceWarning( "Sending metrics via TCP failed with a SocketException: {0}, code: {1}", ex.Message, ex.SocketErrorCode.ToString() );
        }
      }
    }

    private void RestoreConnection()
    {
      lock (_reconnectLock)
      {
        if (!_tcpClient.Connected)
        {
          _tcpClient.Connect(_host, _port);
          _stream = _tcpClient.GetStream();
        }
      }
    }
  }
}
