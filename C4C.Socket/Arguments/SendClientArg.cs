using System;

namespace C4C.Sockets.Arguments
{
    /// <summary>
    /// Аргумент статуса отправки сообщения от клиента к серверу
    /// </summary>
    public class SendClientArgs : EventArgs
    {
        /// <summary>
        /// Размер отправленной информации
        /// </summary>
        public int TotalBytes { get; }
        public SendClientArgs(int size)
        {
            TotalBytes = size;
        }
    }
}
