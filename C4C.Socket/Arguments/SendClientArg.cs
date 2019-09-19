namespace C4C.Sockets.Arguments
{
    /// <summary>
    /// Аргумент статуса отправки сообщения от клиента к серверу
    /// </summary>
    public class SendClientArg
    {
        /// <summary>
        /// Размер отправленной информации
        /// </summary>
        public int TotalBytes { get; }
        public SendClientArg(int size)
        {
            TotalBytes = size;
        }
    }
}
