using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsdClient
{
  internal sealed class NullOutputChannel : IOutputChannel
  {
    public void Send(string line)
    {
      // noop
    }
  }
}
