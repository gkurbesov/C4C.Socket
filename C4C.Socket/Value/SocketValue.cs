using System;

namespace C4C.Sockets.Value
{
    public class SocketValue
    {
        public IntPtr SocketId { get; internal set; } = IntPtr.Zero;
        public string RemoteIP { get; internal set; } = string.Empty;
        public int RemotePort { get; internal set; } = -1;
        internal SocketValue() { }
    }
}
