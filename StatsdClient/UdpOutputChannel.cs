using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StatsdClient
{
    internal sealed class UdpOutputChannel : IOutputChannel
    {
        private readonly UdpClient _udpClient;

        public Socket ClientSocket
        {
            get
            {
                return _udpClient.Client;
            }
        }

        public UdpOutputChannel(string hostOrIPAddress, int port)
        {
            IPAddress ipAddress;
            // Is this an IP address already?
            if (!IPAddress.TryParse(hostOrIPAddress, out ipAddress))
            {
                // Convert to ipv4 address
                ipAddress = Dns.GetHostAddresses(hostOrIPAddress).First(p => p.AddressFamily == AddressFamily.InterNetwork);
            }
            _udpClient = new UdpClient();
            _udpClient.Connect(ipAddress, port);
        }

        public async Task SendAsync(string line)
        {
            var payload = Encoding.UTF8.GetBytes(line);
            await _udpClient.SendAsync(payload, payload.Length);
        }
    }
}