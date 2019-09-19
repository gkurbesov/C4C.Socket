using C4C.Sockets.Arguments;

namespace C4C.Sockets
{
    public delegate void ErrorClientHandler(object Sender, ErrorClientArg e);
    public delegate void MessageClientHandler(object Sender, ReceiveClientArg e);
    public delegate void SendDataClientHandler(object Sender, SendClientArg e);
    public delegate void ConnectionStatusHandler(object Sender);

    public delegate void ErrorServerHandler(object Sender, ErrorServerArg e);
    public delegate void MessageServerHandler(object Sender, ReceiveServerArg e);
    public delegate void ConnectionHandler(object Sender, ClientConnection e);
    public delegate void SendDataServerHandler(object Sender, SendServerArg e);
    public delegate void ServerStatusHandler(object Sender, ServerStatus e);
}
