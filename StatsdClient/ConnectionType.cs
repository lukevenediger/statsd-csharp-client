using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsdClient
{
  /// <summary>
  /// The network connection type
  /// </summary>
  public enum ConnectionType
  {
    /// <summary>
    /// Udp (recommended)
    /// </summary>
    Udp,
    /// <summary>
    /// Tcp
    /// </summary>
    Tcp
  }
}
