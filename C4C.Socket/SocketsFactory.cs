using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4C.Sockets
{
    /// <summary>
    /// Фабрика экземпляров сокетов
    /// </summary>
    public class SocketsFactory
    {
        public static Tcp.TcpClient TcpClient()
        { 
            return new Tcp.TcpClient();
        }
        public static Tcp.TcpClient TcpClient(Encoding encoding, int buffre_size = 1024, int receive_timeout = 500, int send_timeout = 500)
        {
            return new Tcp.TcpClient(encoding, buffre_size, receive_timeout, send_timeout);
        }
        public static Tcp.TcpServer TcpServer()
        {
            return new Tcp.TcpServer();
        }
        public static Tcp.TcpServer TcpServer(Encoding encoding, int buffre_size = 1024, int receive_timeout = 500, int send_timeout = 500)
        {
            return new Tcp.TcpServer(encoding, buffre_size, receive_timeout, send_timeout);
        }
        public static Udp.UdpClient UdpClient()
        {
            return new Udp.UdpClient();
        }
        public static Udp.UdpClient UdpClient(Encoding encoding, int buffre_size = 1024, int receive_timeout = 500, int send_timeout = 500)
        {
            return new Udp.UdpClient(encoding, buffre_size, receive_timeout, send_timeout);
        }
        public static Udp.UdpServer UdpServer()
        {
            return new Udp.UdpServer();
        }
        public static Udp.UdpServer UdpServer(Encoding encoding, int buffre_size = 1024, int receive_timeout = 500, int send_timeout = 500)
        {
            return new Udp.UdpServer(encoding, buffre_size, receive_timeout, send_timeout);
        }
    }
}
