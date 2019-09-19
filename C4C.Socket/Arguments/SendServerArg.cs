using System;
using System.Net;

namespace C4C.Sockets.Arguments
{
    /// <summary>
    /// Аргумент статуса отправки сообщения от сервера
    /// </summary>
    public class SendServerArg
    {
        /// <summary>
        /// Целевой клиент
        /// </summary>
        public IntPtr ClientID { get; } = IntPtr.Zero;
        /// <summary>
        /// Удаленная точка подключения клиента (используется в UDP)
        /// </summary>
        public EndPoint ClientEndPoint { get; } = null;
        /// <summary>
        /// Размер отправленной информации
        /// </summary>
        public int TotalBytes { get; } = -1;
        internal SendServerArg(IntPtr id, int size)
        {
            ClientID = id;
            TotalBytes = size;
        }

        internal SendServerArg(EndPoint point, int size)
        {
            ClientEndPoint = point;
            TotalBytes = size;
        }
    }
}
