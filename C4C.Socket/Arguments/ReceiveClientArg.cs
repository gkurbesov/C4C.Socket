using System;
using System.Text;

namespace C4C.Sockets.Arguments
{
    public class ReceiveClientArgs: EventArgs
    {
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
        internal ReceiveClientArgs() { }
        internal ReceiveClientArgs(byte[] value, Encoding encoding)
        {
            MessageBytes = value;
            StringEcncoding = encoding;
        }
    }
}
