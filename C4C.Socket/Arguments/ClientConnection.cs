﻿using System;

namespace C4C.Sockets.Arguments
{
    /// <summary>
    /// Информация о клиенте сервера
    /// </summary>
    public class ClientConnection
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
        internal ClientConnection() { }
        internal ClientConnection(IntPtr id)
        {
            ClientID = id;
        }
        internal ClientConnection(IntPtr id, string ip)
        {
            ClientID = id;
            ClientIP = ip;
        }
        internal ClientConnection(IntPtr id, string ip, int port)
        {
            ClientID = id;
            ClientIP = ip;
            ClientPort = port;
        }
    }
}
