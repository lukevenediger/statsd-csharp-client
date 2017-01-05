using System.Threading.Tasks;

namespace StatsdClient
{
    internal sealed class NullOutputChannel : IOutputChannel
    {
        public Task SendAsync(string line)
        {
            return Task.FromResult(0);
        }
    }
}