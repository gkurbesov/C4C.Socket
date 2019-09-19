using System;
using System.Net;
using System.Text;

namespace C4C.Sockets.Arguments
{
    public class ReceiveServerArg
    {
        /// <summary>
        /// Клиент передавший информацию
        /// </summary>
        public IntPtr ClientID { get; } = IntPtr.Zero;
        /// <summary>
        /// Удаленныя точка подключения клиента (используется для UDP)
        /// </summary>
        public EndPoint ClientEndPoint { get; } = null;
        /// <summary>
        /// Текстовые данные
        /// </summary>
        public string MessageString { get { return StringEcncoding.GetString(MessageBytes); } }
        /// <summary>
        /// Массив байт полученной информации
        /// </summary>
        public byte[] MessageBytes { get; } = new byte[0];
        /// <summary>
        /// Кодировка для представления текста
        /// </summary>
        private Encoding StringEcncoding { get; set; } = Encoding.UTF8;

        internal ReceiveServerArg() { }
        internal ReceiveServerArg(IntPtr id, byte[] message_byte, Encoding encoding)
        {
            ClientID = id;
            MessageBytes = message_byte;
            StringEcncoding = encoding;
        }
        internal ReceiveServerArg(EndPoint point, byte[] message_byte, Encoding encoding)
        {
            ClientEndPoint = point;
            MessageBytes = message_byte;
            StringEcncoding = encoding;
        }
    }
}
