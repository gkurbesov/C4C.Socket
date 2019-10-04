using System;

namespace C4C.Sockets.Value
{
    /// <summary>
    /// Контейнер данных о сокете
    /// </summary>
    public class SocketValue
    {
        /// <summary>
        /// Указатель на сокет
        /// </summary>
        public IntPtr SocketID { get; internal set; } = IntPtr.Zero;
        /// <summary>
        /// IP адрес удаленной точки
        /// </summary>
        public string RemoteIP { get; internal set; } = string.Empty;
        /// <summary>
        /// Порт соединения исходящей точки
        /// </summary>
        public int RemotePort { get; internal set; } = -1;
        internal SocketValue() { }
    }
}
