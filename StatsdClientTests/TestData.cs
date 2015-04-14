using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsdClientTests
{
  public class TestData
  {
    private static string[] WORDS = "Lorem ipsum dolor sit amet consectetur adipisicing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua Ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur Excepteur sint occaecat cupidatat non proident sunt in culpa qui officia deserunt mollit anim id est laborum".Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    private Random _random;
    private int _lastInteger;
    
    public TestData()
    {
      _random = new Random();
    }

    public int NextInteger
    {
      get { return (_lastInteger = _random.Next(0, Int32.MaxValue)); }
    }

    public int LastInteger
    {
      get { return _lastInteger; }
    }

    public string NextStatName
    {
      get
      {
        var length = _random.Next(1, 5);
        var stat = new string[length];
        for (int i = 0; i < length; i++)
        {
          stat[i] = WORDS[_random.Next(0, WORDS.Length - 1)];
        }
        return String.Join(".", stat);
      }
    }

    public string NextWord
    {
      get
      {
        return WORDS[_random.Next(0, WORDS.Length - 1)];
      }
    }
  }
}
