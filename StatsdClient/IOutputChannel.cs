using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsdClient
{
  /// <summary>
  /// Contract for sending raw statds lines to the server
  /// </summary>
  public interface IOutputChannel
  {
    /// <summary>
    /// Sends a line of stats data to the server.
    /// </summary>
    void Send(string line);
  }
}
