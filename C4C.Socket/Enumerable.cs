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
    /// <summary>
    /// Типы ошибок клиентов
    /// </summary>
    public enum ClientErrorType
    {
        /// <summary>
        /// Неизвестная ошибка
        /// </summary>
        Other,
        /// <summary>
        /// Сокет уже подключен
        /// </summary>
        SocketIsConnected,
        /// <summary>
        /// Сервер не отвечает
        /// </summary>
        ServerIsNotAvailable,
        /// <summary>
        /// Ошибка создания сокета
        /// </summary>
        InitSocketError,
        /// <summary>
        /// Сокет не подключен
        /// </summary>
        SoccketIsNotConnected,
        /// <summary>
        /// Ошибка подключения
        /// </summary>
        ConnectSocketError,
        /// <summary>
        /// Сокет был закрыт
        /// </summary>
        CloseConnection,
        /// <summary>
        /// Ошибка во время отправки данных
        /// </summary>
        SendDataError,
        /// <summary>
        /// Ошибка во время приема данных
        /// </summary>
        ReceiveDataError
    }
    /// <summary>
    /// Статусы сервера
    /// </summary>
    public enum ServerStatus
    {
        /// <summary>
        /// Сервер запущен
        /// </summary>
        Start,
        /// <summary>
        /// Сервер остановлен
        /// </summary>
        Stop
    }
}
