using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsdClient
{
  public interface IOutputChannel
  {
    void Send(string line);
  }
}
