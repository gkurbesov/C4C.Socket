using System;
using System.Linq;

namespace C4C.Sockets
{
    /// <summary>
    /// Буфер данных сокета
    /// </summary>
    internal class BufferCollector
    {
        public byte[] Data { get; private set; } = new byte[0];
        private object Locker = new object();
        /// <summary>
        /// Добавить данные в буфер
        /// </summary>
        /// <param name="value">массиф данных</param>
        /// <param name="size">количество байт, необходимое для считывания</param>
        public void Append(byte[] value, int size)
        {
            Array.Resize(ref value, size);
            lock (Locker) Data = Data.Concat(value).ToArray();
        }
        /// <summary>
        /// очистить буфер данных
        /// </summary>
        public void Clear()
        {
            lock(Locker) Data = new byte[0];
        }
        /// <summary>
        /// Обрезка масива байт
        /// </summary>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] Resize(byte[] value, int size)
        {
            byte[] tmp = new byte[size];
            Array.Copy(value, tmp, size);
            return tmp;
        }
    }
}
