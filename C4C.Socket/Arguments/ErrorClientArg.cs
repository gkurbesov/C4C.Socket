
using System;

namespace C4C.Sockets.Arguments
{
    /// <summary>
    /// Аргумент ошибок слиента
    /// </summary>
    public class ErrorClientArgs : EventArgs
    {
        /// <summary>
        /// Тип возникающей ошибки
        /// </summary>
        public ClientErrorType Type { get; } = ClientErrorType.Other;
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Message { get; } = string.Empty;

        internal ErrorClientArgs() { }
        internal ErrorClientArgs(ClientErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
