﻿using System;

namespace C4C.Sockets.Arguments
{
    /// <summary>
    /// Аргемент ошибок сервера
    /// </summary>
    public class ErrorServerArgs : EventArgs
    {
        /// <summary>
        /// Тип возникающей ошибки
        /// </summary>
        public ServerErrorType Type { get; } = ServerErrorType.Other;
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Message { get; } = string.Empty;
        internal ErrorServerArgs() { }
        internal ErrorServerArgs(ServerErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
