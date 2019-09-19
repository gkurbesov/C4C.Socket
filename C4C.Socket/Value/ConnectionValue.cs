using System.Net.Sockets;

namespace C4C.Sockets.Value
{
    internal class ConnectionValue : SocketValue
    {
        public Socket Socket { get; set; }
        public byte[] Buffer { get; set; } = new byte[0];
        public BufferCollector BufferBuilder { get; set; } = new BufferCollector();

        public ConnectionValue() { }
    }
}
