using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace C4C.Sockets.Udp
{
    public class UdpServer
    {

        /// <summary>
        /// Событие об изменении статуса сервера
        /// </summary>
        public event ServerStatusHandler ListenerStatus;
        /// <summary>
        /// События с отчетом об ошибках
        /// </summary>
        public event ErrorServerHandler ServerErrors;
        /// <summary>
        /// Событие о получение новых данных
        /// </summary>
        public event MessageServerHandler ReceiveMessage;
        /// <summary>
        /// Отчет об отправки данных
        /// </summary>
        public event SendDataServerHandler SendMessage;


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
        /// <summary>
        /// Статус прослушивания порта
        /// </summary>
        public bool IsListen { get; private set; } = false;
        private byte[] Buffer { get; set; } = new byte[0];

        #region конструкторы
        /// <summary>
        /// Конструктор класса сервера
        /// </summary>
        public UdpServer() { }
        /// <summary>
        /// Конструктор класса сервера
        /// </summary>
        /// <param name="encoding">кодировка текстовых сообщений</param>
        /// <param name="buffer_size">размер буфера приема</param>
        /// <param name="receive_timeout">таймаут приема</param>
        /// <param name="send_timeout">таймаут отправки</param>
        public UdpServer(Encoding encoding, int buffer_size = 1024, int receive_timeout = 500, int send_timeout = 500)
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
        /// Метод вызова события о приеме нового сообщения
        /// </summary>
        /// <param name="client_id">ID сокета клиента</param>
        /// <param name="value">Принятые данные</param>
        private void CallReceive(EndPoint point, byte[] value)
        {
            Task.Factory.StartNew(() => { ReceiveMessage?.Invoke(this, new Arguments.ReceiveServerArgs(point, value, StringEcncoding)); });
        }
        /// <summary>
        /// Метод вызова события о статусе доставки данных
        /// </summary>
        /// <param name="client_id">ID сокета клиента</param>
        /// <param name="size">количество переданных данных</param>
        private void CallSendResult(EndPoint point, int size)
        {
            Task.Factory.StartNew(() => { SendMessage?.Invoke(this, new Arguments.SendServerArgs(point, size)); });
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
                if (SetupServerSocket() != false)
                {
                    EndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
                    Buffer = new byte[SizeBuffer];
                    IsListen = true;
                    CallStatus(ServerStatus.Start);
                    ServerSocket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref localEndPoint, new AsyncCallback(ReceiveCallback), localEndPoint);
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
                }
            }
            if(ServerSocket != null)
            {
                ServerSocket.Close();
                ServerSocket.Dispose();
                ServerSocket = null;
            }
            if(IsListen) CallStatus(ServerStatus.Stop);
            IsListen = false;
        }
        /// <summary>
        /// Отправка текста
        /// </summary>
        /// <param name="client_point">ID клиента, полученный при событии подкулючения</param>
        /// <param name="data">текста в кодировке установленной для сервера (по умолчанию UTF8)</param>
        public void Send(EndPoint client_point, string data)
        {
            Send(client_point, StringEcncoding.GetBytes(data));
        }
        /// <summary>
        /// Отправка масива байт
        /// </summary>
        /// <param name="client_id">ID клиента, полученный при событии подкулючения</param>
        /// <param name="data">массив байт для отправки</param>
        public void Send(EndPoint client_point, byte[] data)
        {
            try
            {
                ServerSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, client_point,
                    new AsyncCallback(SendCallback), client_point);
            }
            catch (SocketException exc)
            {
                CallErrorServer(ServerErrorType.SendDataError, exc.Message);
            }
            catch (Exception exc)
            {
                CallErrorServer(ServerErrorType.SendDataError, exc.Message);
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
                if (IsListen != true)
                {
                    if (ServerSocket != null) ServerSocket.Dispose();
                    // создаем сокет
                    ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    ServerSocket.ReceiveTimeout = TimeoutReceive;
                    ServerSocket.SendTimeout = TimeoutSend;
                    ServerSocket.ReceiveBufferSize = SizeBuffer;
                    ServerSocket.SendBufferSize = SizeBuffer;
                    EndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
                    ServerSocket.Bind(localEndPoint);
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
        ///  Метод асинхронного получения сообщений
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveCallback(IAsyncResult result)
        {
            if (ServerSocket != null && IsListen)
            {
                try
                {
                    EndPoint point_remote = (EndPoint)result.AsyncState;
                    int read_size = ServerSocket.EndReceiveFrom(result, ref point_remote);
                    if (read_size > 0)
                    {
                        byte[] receive_data = BufferCollector.Resize(Buffer, read_size);
                        CallReceive(point_remote, receive_data);
                    }
                }
                catch (Exception ex)
                {
                    CallErrorServer(ServerErrorType.ReceiveDataError, ex.Message);
                }
                finally
                {
                    Buffer = new byte[SizeBuffer];
                    EndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
                    ServerSocket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref localEndPoint,
                        new AsyncCallback(ReceiveCallback), localEndPoint);
                }
            }
            else
            {
                Stop();
            }
        }
        /// <summary>
        ///  Метод отправки сообщения (калбэк)
        /// </summary>
        /// <param name="result"></param>
        private void SendCallback(IAsyncResult result)
        {
            EndPoint point_remote = (EndPoint)result.AsyncState;
            try
            {
                // Отправка сообщения завершена
                int send_size = ServerSocket.EndSend(result);
                CallSendResult(point_remote, send_size);
            }
            catch (Exception exc)
            {
                CallErrorServer(ServerErrorType.SendDataError, exc.Message);
            }
        }

    }
}
