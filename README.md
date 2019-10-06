# C4C.Socket
Библиотека для облегчения работы с System.Net.Sockets

Библиотека использует событийно-ориентированную модель, а сама работа с сокетом внутри библиотеки происходит в асинхронном режиме.


## Примеры использования

Пример использования TCP сервера

```C#
using System;
using C4C.Sockets;
using C4C.Sockets.Arguments;

namespace ConsoleApp
{
    class Program
    {
        public static C4C.Sockets.Tcp.TcpServer server;
        static void Main(string[] args)
        {
            // Creat socket
            server = SocketsFactory.TcpServer();

            // Subscribing to server events
            server.ServerErrors += Server_ServerErrors;
            server.ListenerStatus += Server_ListenerStatus;
            server.ClientConnect += Server_ClientConnect;
            server.ClientDisconnect += Server_ClientDisconnect;
            server.ReceiveMessage += Server_ReceiveMessage;

            Console.WriteLine("Start socket server");
            
            // Start listening on tcp port 80
            server.Start(80);

            Console.ReadLine();
            Console.WriteLine("Stop socket server");
            Console.ReadLine();
        }
        private static void Server_ReceiveMessage(object sender, ReceiveServerArgs e)
        {
            Console.WriteLine("Receive from client ", e.ClientID.ToString());
            Console.WriteLine("Message: ", e.MessageString);
            // Return message
            //server.Send(e.ClientID, e.MessageBytes);
            server.Send(e.ClientID, e.MessageString);
        }
        private static void Server_ClientDisconnect(object sender, ClientConnectionArgs e)
        {
            Console.WriteLine("Client disconnect {0}", e.ClientID.ToString());
        }
        private static void Server_ClientConnect(object sender, ClientConnectionArgs e)
        {
            Console.WriteLine("Client connect {0}", e.ClientID.ToString());
        }
        private static void Server_ListenerStatus(object sender, ServerStatus e)
        {
            Console.WriteLine("Server status - {0}", e.ToString());
        }
        private static void Server_ServerErrors(object sender, ErrorServerArgs e)
        {
            Console.WriteLine("Error: {0} - {1}", e.Type.ToString(), e.Message);
        }
    }
}
```

Пример использования TCP клиента
```C#
using System;
using C4C.Sockets;
using C4C.Sockets.Arguments;

namespace ConsoleApp
{
    class Program
    {
        public static C4C.Sockets.Tcp.TcpClient client;
        static void Main(string[] args)
        {
            // Creat socket
            client = SocketsFactory.TcpClient();
            
            // Subscribing to client events
            client.ClientErrors += Client_ClientErrors;
            client.Connected += Client_Connected;
            client.Disconnected += Client_Disconnected;
            client.ReceiveMessage += Client_ReceiveMessage;
            
            Console.WriteLine("Connect to 127.0.0.1:80");
            
            // Start connect to 127.0.0.1:80
            client.Connect("127.0.0.1", 80);
            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToUpperInvariant().Equals("EXIT")) break;
                client.Send(input);
            }
            Console.ReadLine();
            Console.WriteLine("Stop socket client");
            client.Disconnect();
            Console.ReadLine();
        }
        private static void Client_ReceiveMessage(object sender, ReceiveClientArgs e)
        {
            Console.WriteLine("REceive from server: {0}", e.MessageString);
        }
        private static void Client_Disconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
        }
        private static void Client_Connected(object sender, EventArgs e)
        {
            Console.WriteLine("Connected");
        }
        private static void Client_ClientErrors(object sender, ErrorClientArgs e)
        {
            Console.WriteLine("Error {0} - {1}", e.Type.ToString(), e.Message);
        }
    }
}
```
