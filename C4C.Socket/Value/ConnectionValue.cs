using System.Net.Sockets;

namespace C4C.Sockets.Value
{
    /// <summary>
    /// Контейнер для сокета подключившегося клиента
    /// </summary>
    internal class ConnectionValue : SocketValue
    {
        /// <summary>
        /// Сокет клиента
        /// </summary>
        public Socket Socket { get; set; }
        /// <summary>
        /// Промежуточный буфер данных
        /// </summary>
        public byte[] Buffer { get; set; } = new byte[0];
        /// <summary>
        /// Буфер данных
        /// </summary>
        public BufferCollector BufferBuilder { get; set; } = new BufferCollector();
        public ConnectionValue() { }
    }
}
