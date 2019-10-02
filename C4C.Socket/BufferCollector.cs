using System;
using System.Linq;

namespace C4C.Sockets
{
    internal class BufferCollector
    {
        public byte[] Data { get; private set; } = new byte[0];
        private object Locker = new object();

        public void Append(byte[] value, int size)
        {
            Array.Resize(ref value, size);
            lock (Locker) Data = Data.Concat(value).ToArray();
        }

        public void Clear()
        {
            lock(Locker) Data = new byte[0];
        }

        public static byte[] Resize(byte[] value, int size)
        {
            byte[] tmp = new byte[size];
            Array.Copy(value, tmp, size);
            return tmp;
        }
    }
}
