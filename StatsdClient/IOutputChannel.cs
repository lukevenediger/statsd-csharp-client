using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsdClient
{
  public interface IOutputChannel
  {
    void Send(string line);
  }
}
