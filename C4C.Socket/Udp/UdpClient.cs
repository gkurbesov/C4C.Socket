using C4C.Sockets.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace C4C.Sockets.Udp
{
    public class UdpClient
    {
        /// <summary>
        /// Событие успешного подключения
        /// </summary>
        public event EventHandler Connected;
        /// <summary>
        /// Событие об отключении сокета
        /// </summary>
        public event EventHandler Disconnected;
        /// <summary>
        /// Событие ошибок сокета
        /// </summary>
        public event EventHandler<ErrorClientArgs> ClientErrors;
        /// <summary>
        /// Событие о получении новых данных
        /// </summary>
        public event EventHandler<ReceiveClientArgs> ReceiveMessage;
        /// <summary>
        /// Событие о статусе отправки данных
        /// </summary>
        public event EventHandler<SendClientArgs> SendMessage;

        /// <summary>
        /// Метка экземпляра класса
        /// </summary>
        public object Tag { get; set; } = null;
        /// <summary>
        /// Таймаут приема (мс)
        /// </summary>
        public int TimeoutReceive { get; internal set; } = 500;
        /// <summary>
        /// Таймаут отправки (мс)
        /// </summary>
        public int TimeoutSend { get; internal set; } = 500;
        /// <summary>
        /// Кодировка текстовых данных
        /// </summary>
        public Encoding StringEcncoding { get; internal set; } = Encoding.UTF8;
        /// <summary>
        /// Статус подключения
        /// </summary>
        public bool ConnectedStatus { get; internal set; } = false;
        /// <summary>
        /// Сокет клиента
        /// </summary>
        internal Socket ClientSocket { get; set; } = null;
        /// <summary>
        /// Размер принимающего буфера
        /// </summary>
        public int SizeBuffer { get; internal set; } = 1024;
        /// <summary>
        /// Второстепенный буфер приема данных
        /// </summary>
        internal byte[] Buffer { get; set; } = new byte[1024];
        /// <summary>
        /// Буфер полученных данных
        /// </summary>
        internal BufferCollector BufferBuilder { get; set; } = new BufferCollector();
        internal EndPoint ServerEndPoint = null;
        private object locker = new object();


        #region конструкторы
        public UdpClient() { }
        public UdpClient(Encoding encoding, int buffre_size = 1024, int receive_timeout = 500, int send_timeout = 500)
        {
            StringEcncoding = encoding;
            SizeBuffer = buffre_size;
            TimeoutReceive = receive_timeout;
            TimeoutSend = send_timeout;
        }
        #endregion


        #region методы для вызова событий
        /// <summary>
        /// Вызов событий об ошибках
        /// </summary>
        /// <param name="type">тип ошибки</param>
        /// <param name="message">сообщение об ошибке</param>
        internal void CallErrorClient(ClientErrorType type, string message)
        {
            Task.Factory.StartNew(() => ClientErrors?.Invoke(this, new ErrorClientArgs(type, message)));
        }
        /// <summary>
        /// Событие о получении новых данных
        /// </summary>
        /// <param name="value">полученные данные от сервера</param>
        internal void CallReceive(byte[] value)
        {
            Task.Factory.StartNew(() => { ReceiveMessage?.Invoke(this, new ReceiveClientArgs(value, StringEcncoding)); });
        }
        /// <summary>
        /// Вызов события об успешной отправки данных
        /// </summary>
        /// <param name="send_size"></param>
        internal void CallSend(int send_size)
        {
            Task.Factory.StartNew(() => { SendMessage?.Invoke(this, new SendClientArgs(send_size)); });
        }

        internal void CallConnected()
        {
            Task.Factory.StartNew(() => Connected?.Invoke(this, EventArgs.Empty));
        }

        internal void CallDisconnected()
        {
            Task.Factory.StartNew(() => Disconnected?.Invoke(this, EventArgs.Empty));
        }
        #endregion


        #region методы управления сокет клиентом
        /// <summary>
        /// Подключение к серверу
        /// </summary>
        /// <param name="server_host">адрес сервера</param>
        /// <param name="server_port">порт для подключения</param>
        public void Connect(string server_host, int server_port)
        {
            lock(locker)
            {
                if (!ConnectedStatus)
                {
                    try
                    {
                        if (ClientSocket != null)
                        {
                            ClientSocket.Dispose();
                            ClientSocket = null;
                        }
                        IPAddress[] server_host_list = Dns.GetHostAddresses(server_host);
                        IPAddress ip_adress = Array.Find(server_host_list, o => o.AddressFamily == AddressFamily.InterNetwork);
                        IPEndPoint remote_end_point = new IPEndPoint(ip_adress, server_port);
                        ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                        {
                            ReceiveTimeout = TimeoutReceive,
                            SendTimeout = TimeoutSend
                        };
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                ClientSocket.Connect(remote_end_point);
                                ServerEndPoint = ClientSocket.RemoteEndPoint;
                                Buffer = new byte[SizeBuffer];
                                ConnectedStatus = true;
                                CallConnected();
                                ClientSocket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None,
                                    ref ServerEndPoint, new AsyncCallback(ReceiveCallback), null);
                            }
                            catch (SocketException ex)
                            {
                                CallErrorClient(ClientErrorType.ServerIsNotAvailable, ex.Message + ex.StackTrace);                                
                                Disconnect();
                            }
                            catch (Exception ex)
                            {
                                CallErrorClient(ClientErrorType.ConnectSocketError, ex.Message);
                                Disconnect();
                            }
                        });
                    }
                    catch (SocketException ex)
                    {
                        CallErrorClient(ClientErrorType.ServerIsNotAvailable, ex.Message + ex.StackTrace);
                        Disconnect();
                    }
                    catch (Exception ex)
                    {
                        CallErrorClient(ClientErrorType.ConnectSocketError, ex.Message);
                        Disconnect();
                    }
                }
                else
                {
                    CallErrorClient(ClientErrorType.SocketIsConnected, "For a new connection, you must break the old");
                }

            }
        }
        /// <summary>
        /// Отправка данных серверу
        /// </summary>
        /// <param name="value">строковое представление данных</param>
        public void Send(string value)
        {
            Send(StringEcncoding.GetBytes(value));
        }
        /// <summary>
        /// Отправка данных серверу
        /// </summary>
        /// <param name="value">любой массив байт</param>
        public void Send(byte[] value)
        {
            if (ConnectedStatus)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ClientSocket.SendTo(value, ServerEndPoint);
                    }
                    catch (SocketException ex)
                    {
                        CallErrorClient(ClientErrorType.SendDataError, ex.Message);
                        Disconnect();
                    }
                    catch (Exception ex)
                    {
                        CallErrorClient(ClientErrorType.SendDataError, ex.Message);
                        Disconnect();
                    }
                });
            }
            else
            {
                CallErrorClient(ClientErrorType.SoccketIsNotConnected, "The socket is not init");
            }
        }
        /// <summary>
        /// Отключение сокета от сервера
        /// </summary>
        public void Disconnect()
        {
            if (ClientSocket != null)
            {
                lock (ClientSocket)
                {
                    try
                    {
                        ClientSocket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception ex)
                    {
                        CallErrorClient(ClientErrorType.CloseConnection, "Error in Shutdown - " + ex.Message);
                    }
                }
                if (ClientSocket != null)
                {
                    ClientSocket.Close();
                    ClientSocket.Dispose();
                }
                ClientSocket = null;
            }
            if (ConnectedStatus) CallDisconnected();
            ConnectedStatus = false;
        }
        #endregion

        /// <summary>
        /// Делегат вызова принятия данных  (калбэк)
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                if (ClientSocket != null && ClientSocket.Connected)
                {
                    int read_size = ClientSocket.EndReceiveFrom(result, ref ServerEndPoint);
                    if (read_size > 0)
                    {
                        BufferBuilder.Append(Buffer, read_size);
                        if (ClientSocket.Available <= 0)
                        {
                            //Вызываем событие по окнчанию чтения данных от сокета
                            CallReceive(BufferBuilder.Data);
                            BufferBuilder.Clear();
                        }
                        ClientSocket.BeginReceiveFrom(
                            Buffer, 0,
                            Buffer.Length, SocketFlags.None, ref ServerEndPoint,
                            new AsyncCallback(ReceiveCallback),
                            null);
                    }
                    else
                    {
                        Disconnect();
                    }
                }
                else
                {
                    Disconnect();
                }
            }
            catch (SocketException ex)
            {
                CallErrorClient(ClientErrorType.ReceiveDataError, ex.Message);
                Disconnect();
            }
            catch (Exception ex)
            {
                CallErrorClient(ClientErrorType.ReceiveDataError, ex.Message);
                Disconnect();
            }
        }
    }
}
