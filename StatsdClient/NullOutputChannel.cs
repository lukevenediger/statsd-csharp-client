using System.Threading.Tasks;

namespace StatsdClient
{
    internal sealed class NullOutputChannel : IOutputChannel
    {
        public async Task SendAsync(string line)
        {
        }
    }
}