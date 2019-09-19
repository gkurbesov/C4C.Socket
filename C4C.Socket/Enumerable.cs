namespace C4C.Sockets
{
    /// <summary>
    /// Типы ошибок сервера
    /// </summary>
    public enum ServerErrorType
    {
        /// <summary>
        /// Прочий тип ошибок
        /// </summary>
        Other,
        /// <summary>
        /// Ошибка, сервер уже был запущен
        /// </summary>
        WasListening,
        /// <summary>
        /// Нет разрешения на работу с сокетом
        /// </summary>
        SecurityError,
        /// <summary>
        /// Ошибка инициализации сокета
        /// </summary>
        InitServerError,
        /// <summary>
        /// Ошибка запуска сервера
        /// </summary>
        StartListenError,
        /// <summary>
        /// Ошибка закрытия соединения с клиентом
        /// </summary>
        CloseConnection,
        /// <summary>
        /// Ошибка при отпраке сообщения
        /// </summary>
        SendDataError,
        /// <summary>
        /// Ошибка при попытке приема сообщения
        /// </summary>
        ReceiveDataError,
        /// <summary>
        /// Клиента не существует
        /// </summary>
        EmptyClientError,
        /// <summary>
        /// Ошибка принятие входящего подключения
        /// </summary>
        AcceptError
    }

    public enum ClientErrorType
    {
        Other,
        SocketIsConnected,
        ServerIsNotAvailable,
        InitSocketError,
        SoccketIsNotConnected,
        ConnectSocketError,
        CloseConnection,
        SendDataError,
        ReceiveDataError
    }

    public enum ServerStatus
    {
        Start,
        Stop
    }
}
