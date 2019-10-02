using System;

namespace C4C.Sockets.Arguments
{
    /// <summary>
    /// Информация о клиенте сервера
    /// </summary>
    public class ClientConnectionArgs : EventArgs
    {
        /// <summary>
        /// Целевой клиент
        /// </summary>
        public IntPtr ClientID { get; } = IntPtr.Zero;
        /// <summary>
        /// IP адрес клиента
        /// </summary>
        public string ClientIP { get; } = string.Empty;
        /// <summary>
        /// Удаленный порт сокета
        /// </summary>
        public int ClientPort { get; } = -1;
        internal ClientConnectionArgs() { }
        internal ClientConnectionArgs(IntPtr id)
        {
            ClientID = id;
        }
        internal ClientConnectionArgs(IntPtr id, string ip)
        {
            ClientID = id;
            ClientIP = ip;
        }
        internal ClientConnectionArgs(IntPtr id, string ip, int port)
        {
            ClientID = id;
            ClientIP = ip;
            ClientPort = port;
        }
    }
}
