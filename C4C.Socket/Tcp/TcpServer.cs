using C4C.Sockets.Value;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using C4C.Sockets.Arguments;

namespace C4C.Sockets.Tcp
{
    public class TcpServer
    {

        /// <summary>
        /// Событие об изменении статуса сервера
        /// </summary>
        public event ServerStatusHandler ListenerStatus;
        /// <summary>
        /// События с отчетом об ошибках
        /// </summary>
        public event EventHandler<ErrorServerArgs> ServerErrors;
        /// <summary>
        /// Событие о получение новых данных
        /// </summary>
        public event EventHandler<ReceiveServerArgs> ReceiveMessage;
        /// <summary>
        /// Отчет об отправки данных
        /// </summary>
        public event EventHandler<SendServerArgs> SendMessage;
        /// <summary>
        /// Событие о подключении нового клиента
        /// </summary>
        public event EventHandler<ClientConnectionArgs> ClientConnect;
        /// <summary>
        /// Событие об отключении клиента
        /// </summary>
        public event EventHandler<ClientConnectionArgs> ClientDisconnect;

        /// <summary>
        /// Прослушивающий сокет
        /// </summary>
        private Socket ServerSocket { get; set; }
        /// <summary>
        /// Порт прослушивания
        /// </summary>
        private int Port { get; set; } = 0;
        /// <summary>
        /// Метка экземпляра класса
        /// </summary>
        public object Tag { get; set; } = null;
        /// <summary>
        /// Размер принимающего буфера
        /// </summary>
        public int SizeBuffer { get; private set; } = 1024;
        /// <summary>
        /// Таймаут приема (мс)
        /// </summary>
        public int TimeoutReceive { get; private set; } = 500;
        /// <summary>
        /// Таймаут отправки (мс)
        /// </summary>
        public int TimeoutSend { get; private set; } = 500;
        /// <summary>
        /// Кодировка для текстовых данных
        /// </summary>
        public Encoding StringEcncoding { get; private set; } = Encoding.UTF8;
        private List<ConnectionValue> Сonnections = new List<ConnectionValue>();
        /// <summary>
        /// Список подключенных клиентов
        /// </summary>
        public List<SocketValue> ConnectionList
        {
            get
            {
                List<SocketValue> tmp = new List<SocketValue>();
                lock (Сonnections) Сonnections.ForEach(o => tmp.Add(o));
                return tmp;
            }
        }
        /// <summary>
        /// Количество подключенных клиентов
        /// </summary>
        public int ClientCount { get { return Сonnections.Count; } }
        private volatile bool IsListeningStatus = false;
        /// <summary>
        /// Статус прослушивания порта
        /// </summary>
        public bool IsListen { get { return IsListeningStatus; } }

        #region конструкторы
        /// <summary>
        /// Конструктор класса сервера
        /// </summary>
        public TcpServer() { }
        /// <summary>
        /// Конструктор класса сервера
        /// </summary>
        /// <param name="encoding">кодировка текстовых сообщений</param>
        /// <param name="buffer_size">размер буфера приема</param>
        /// <param name="receive_timeout">таймаут приема</param>
        /// <param name="send_timeout">таймаут отправки</param>
        public TcpServer(Encoding encoding, int buffer_size = 1024, int receive_timeout = 500, int send_timeout = 500)
        {
            StringEcncoding = encoding;
            SizeBuffer = buffer_size;
            TimeoutReceive = receive_timeout;
            TimeoutSend = send_timeout;
        }
        #endregion

        #region методы для вызова событий
        /// <summary>
        /// Метод для передачи статусов ошибок
        /// </summary>
        /// <param name="type">тип ошибки</param>
        /// <param name="message">сообщение об ошибке</param>
        private void CallErrorServer(ServerErrorType type, string message = "")
        {
            Task.Factory.StartNew(() => { ServerErrors?.Invoke(this, new Arguments.ErrorServerArgs(type, message)); });
        }
        /// <summary>
        /// Метод для передачи статус сервера
        /// </summary>
        /// <param name="status">статус сервера</param>
        private void CallStatus(ServerStatus status)
        {
            Task.Factory.StartNew(() => { ListenerStatus?.Invoke(this, status); });
        }
        /// <summary>
        /// Вызов события о новом подключении
        /// </summary>
        /// <param name="id">ID сокета</param>
        /// <param name="ip">IP адрес подклоючения</param>
        /// <param name="port">Удаленный порт</param>
        private void CallConnected(ConnectionValue client)
        {
            Task.Factory.StartNew(() => { ClientConnect?.Invoke(this, new Arguments.ClientConnectionArgs(client.SocketID, client.RemoteIP, client.RemotePort)); });
        }
        /// <summary>
        /// Вызов события об отключении
        /// </summary>
        /// <param name="id">ID сокета</param>
        /// <param name="ip">IP адрес подклоючения</param>
        /// <param name="port">Удаленный порт</param>
        private void CallDisconnected(ConnectionValue client)
        {
            Task.Factory.StartNew(() => { ClientDisconnect?.Invoke(this, new Arguments.ClientConnectionArgs(client.SocketID, client.RemoteIP, client.RemotePort)); });
        }
        /// <summary>
        /// Метод вызова события о приеме нового сообщения
        /// </summary>
        /// <param name="client_id">ID сокета клиента</param>
        /// <param name="value">Принятые данные</param>
        private void CallReceive(IntPtr client_id, byte[] value)
        {
            Task.Factory.StartNew(() => { ReceiveMessage?.Invoke(this, new Arguments.ReceiveServerArgs(client_id, value, StringEcncoding)); });
        }
        /// <summary>
        /// Метод вызова события о статусе доставки данных
        /// </summary>
        /// <param name="client_id">ID сокета клиента</param>
        /// <param name="size">количество переданных данных</param>
        private void CallSendResult(IntPtr client_id, int size)
        {
            Task.Factory.StartNew(() => { SendMessage?.Invoke(this, new Arguments.SendServerArgs(client_id, size)); });
        }
        #endregion

        #region методы управления сервером
        /// <summary>
        /// Запуск сервера
        /// </summary>
        /// <param name="port">порт сервера для прослушивания</param>
        public void Start(int port)
        {
            Port = port;
            try
            {
                if (SetupServerSocket())
                {
                    ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), ServerSocket);
                    IsListeningStatus = true;
                    CallStatus(ServerStatus.Start);
                }
                else
                {
                    CallErrorServer(ServerErrorType.StartListenError, "Failed to create socket");
                }
            }
            catch (Exception exс)
            {
                CallErrorServer(ServerErrorType.StartListenError, exс.Message);
            }
        }
        /// <summary>
        /// Метод для остановки сервера
        /// </summary>
        public void Stop()
        {
            if (ServerSocket != null)
            {
                lock (ServerSocket)
                {
                    try
                    {
                        ServerSocket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception ex)
                    {
                        CallErrorServer(ServerErrorType.CloseConnection, ex.Message);
                    }
                    if (ServerSocket != null)
                    {
                        ServerSocket.Close();
                        ServerSocket.Dispose();
                    }
                    ServerSocket = null;
                    foreach (var client in Сonnections.ToArray())
                    {
                        try
                        {
                            CloseConnection(client);
                        }
                        catch (Exception ex)
                        {
                            CallErrorServer(ServerErrorType.CloseConnection, ex.Message);
                        }
                    }
                }
            }
            lock (Сonnections) Сonnections.Clear();
            if (IsListeningStatus) CallStatus(ServerStatus.Stop);
            IsListeningStatus = false;
        }
        /// <summary>
        /// Отправка текста всем подключенным клиентам
        /// </summary>
        /// <param name="data">текст для отправки</param>
        public void SendToAll(string data)
        {
            SendToAll(StringEcncoding.GetBytes(data));
        }
        /// <summary>
        /// Отправка данных всем подключенным клиентам
        /// </summary>
        /// <param name="data">массив байт для отправки</param>
        public void SendToAll(byte[] data)
        {
            foreach (var client in Сonnections.ToArray())
            {
                if (client != null && client.Socket != null)
                {
                    try
                    {
                        client.Socket.BeginSend(data, 0, data.Length, SocketFlags.None,
                            new AsyncCallback(SendCallback), client);
                    }
                    catch (SocketException exc)
                    {
                        CallErrorServer(ServerErrorType.SendDataError, exc.Message);
                        CloseConnection(client);
                    }
                    catch (Exception exc)
                    {
                        CallErrorServer(ServerErrorType.SendDataError, exc.Message);
                        CloseConnection(client);
                    }
                }
                else
                {
                    CloseConnection(client);
                }
            }
        }
        /// <summary>
        /// Отправка текста
        /// </summary>
        /// <param name="client_id">ID клиента, полученный при событии подкулючения</param>
        /// <param name="data">текста в кодировке установленной для сервера (по умолчанию UTF8)</param>
        public void Send(IntPtr client_id, string data)
        {
            Send(client_id, StringEcncoding.GetBytes(data));
        }
        /// <summary>
        /// Отправка масива байт
        /// </summary>
        /// <param name="client_id">ID клиента, полученный при событии подкулючения</param>
        /// <param name="data">массив байт для отправки</param>
        public void Send(IntPtr client_id, byte[] data)
        {
            ConnectionValue client = null;
            lock (Сonnections) client = Сonnections.Find(o => o.SocketID.Equals(client_id));
            if (client != null)
            {
                try
                {
                    client.Socket.BeginSend(data, 0, data.Length, SocketFlags.None,
                        new AsyncCallback(SendCallback), client);
                }
                catch (SocketException exc)
                {
                    CallErrorServer(ServerErrorType.SendDataError, exc.Message);
                    CloseConnection(client);
                }
                catch (Exception exc)
                {
                    CallErrorServer(ServerErrorType.SendDataError, exc.Message);
                    CloseConnection(client);
                }
            }
            else
            {
                CallErrorServer(ServerErrorType.EmptyClientError, client_id.ToString() + " - not found");
            }
        }
        /// <summary>
        /// Отлючить клиента
        /// </summary>
        /// <param name="client_id">id сокета клиента</param>
        public void Kick(IntPtr client_id)
        {
            try
            {
                ConnectionValue client;
                lock (Сonnections) client = Сonnections.Find(o => o.SocketID.Equals(client_id));
                if (client != null)
                {
                    CloseConnection(client);
                }
                else
                {
                    CallErrorServer(ServerErrorType.EmptyClientError, client_id.ToString() + " - not found");
                }
            }
            catch (Exception ex)
            {
                CallErrorServer(ServerErrorType.EmptyClientError, ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Метод запускае прослушивание порт и возвращает сведение о запуске
        /// </summary>
        /// <returns>true - если сокет сервера создан и запущен</returns>
        private bool SetupServerSocket()
        {
            bool status = false;
            try
            {
                if (IsListeningStatus != true)
                {
                    if (ServerSocket != null)
                    {
                        ServerSocket.Dispose();
                        ServerSocket = null;
                    }
                    // создаем сокет
                    ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    {
                        ReceiveTimeout = TimeoutReceive,
                        SendTimeout = TimeoutSend,
                        ReceiveBufferSize = SizeBuffer,
                        SendBufferSize = SizeBuffer
                    };
                    IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
                    // Create a TCP/IP socket.  
                    ServerSocket.Bind(localEndPoint);
                    // начинаем слушать порт
                    ServerSocket.Listen((int)SocketOptionName.MaxConnections);
                    status = true;
                }
                else
                {
                    CallErrorServer(ServerErrorType.WasListening, "The server has already been started earlier");
                }
            }
            catch (SocketException ex)
            {
                CallErrorServer(ServerErrorType.InitServerError, ex.Message);
            }
            catch (SecurityException ex)
            {
                CallErrorServer(ServerErrorType.SecurityError, ex.Message);
            }
            catch (Exception ex)
            {
                CallErrorServer(ServerErrorType.Other, ex.Message);
            }
            return status;
        }
        /// <summary>
        /// Метод асинхронного принятия новых подключений
        /// </summary>
        /// <param name="result"></param>
        private void AcceptCallback(IAsyncResult result)
        {
            if (IsListeningStatus != false)
            {
                ConnectionValue connection = new ConnectionValue();
                try
                {
                    // Завершение операции Accept
                    connection.Socket = ServerSocket.EndAccept(result);
                    connection.SocketID = connection.Socket.Handle;
                    connection.Buffer = new byte[SizeBuffer];
                    connection.RemoteIP = ((IPEndPoint)connection.Socket.RemoteEndPoint).Address.ToString();
                    connection.RemotePort = ((IPEndPoint)connection.Socket.RemoteEndPoint).Port;
                    lock (Сonnections) Сonnections.Add(connection);
                    // Начало операции Receive и новой операции Accept
                    connection.Socket.BeginReceive(connection.Buffer,
                        0, connection.Buffer.Length, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback),
                        connection);
                    //Сообщаем о новом подключении                    
                    CallConnected(connection);
                }
                catch (SocketException exc)
                {
                    CallErrorServer(Sockets.ServerErrorType.AcceptError, exc.Message + exc.ToString());
                    CloseConnection(connection);
                }
                catch (Exception exc)
                {
                    CallErrorServer(Sockets.ServerErrorType.AcceptError, exc.Message + exc.ToString());
                    CloseConnection(connection);
                }
                finally
                {
                    if (ServerSocket != null)
                        ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                }
            }
        }
        /// <summary>
        ///  Метод асинхронного получения сообщений
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionValue connection = (ConnectionValue)result.AsyncState;
            try
            {
                if (connection.Socket.Connected)
                {
                    int read_size = connection.Socket.EndReceive(result);
                    if (read_size > 0)
                    {
                        connection.BufferBuilder.Append(connection.Buffer, read_size);
                        if (connection.Socket.Available <= 0)
                        {
                            //Вызываем событие по окнчанию чтения данных от сокета
                            CallReceive(connection.SocketID, connection.BufferBuilder.Data);
                            connection.BufferBuilder.Clear();
                        }
                        connection.Socket.BeginReceive(
                            connection.Buffer, 0,
                            connection.Buffer.Length, SocketFlags.None,
                            new AsyncCallback(ReceiveCallback),
                            connection);
                    }
                    else
                    {
                        CloseConnection(connection);
                    }
                }
                else
                {
                    CloseConnection(connection);
                }
            }
            catch (SocketException exc)
            {
                CallErrorServer(ServerErrorType.ReceiveDataError, exc.Message);
                CloseConnection(connection);
            }
            catch (Exception exc)
            {
                CloseConnection(connection);
                CallErrorServer(ServerErrorType.ReceiveDataError, exc.Message);
            }
        }
        /// <summary>
        ///  Метод отправки сообщения (калбэк)
        /// </summary>
        /// <param name="result"></param>
        private void SendCallback(IAsyncResult result)
        {
            ConnectionValue connection = (ConnectionValue)result.AsyncState;
            try
            {
                // Отправка сообщения завершена
                int send_size = connection.Socket.EndSend(result);
                CallSendResult(connection.SocketID, send_size);
            }
            catch (SocketException exc)
            {
                CallErrorServer(Sockets.ServerErrorType.SendDataError, exc.Message);
                CloseConnection(connection);
            }
            catch (Exception exc)
            {
                CallErrorServer(Sockets.ServerErrorType.SendDataError, exc.Message);
                CloseConnection(connection);
            }
        }
        /// <summary>
        /// Метод закрытия соединения с клиентом
        /// </summary>
        /// <param name="client">Экземпляр данных о клиенте</param>
        private void CloseConnection(ConnectionValue client)
        {
            if (client != null && client.Socket != null)
            {
                try
                {
                    if (client.Socket.Connected) client.Socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    CallErrorServer(ServerErrorType.CloseConnection, ex.Message);
                }
                client.Socket.Close();
                client.Socket.Dispose();
                lock (Сonnections) Сonnections.Remove(client);
                CallDisconnected(client);
            }
            else if (client != null && client.Socket == null)
            {
                lock (Сonnections) Сonnections.Remove(client);
            }

        }
    }
}
