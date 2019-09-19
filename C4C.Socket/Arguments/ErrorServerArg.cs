namespace C4C.Sockets.Arguments
{
    /// <summary>
    /// Аргемент ошибок сервера
    /// </summary>
    public class ErrorServerArg
    {
        /// <summary>
        /// Тип возникающей ошибки
        /// </summary>
        public ServerErrorType Type { get; } = ServerErrorType.Other;
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Message { get; } = string.Empty;

        internal ErrorServerArg() { }
        internal ErrorServerArg(ServerErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
