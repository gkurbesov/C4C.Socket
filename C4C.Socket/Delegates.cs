using C4C.Sockets.Arguments;

namespace C4C.Sockets
{
    public delegate void ErrorClientHandler(object Sender, ErrorClientArgs e);
    public delegate void MessageClientHandler(object Sender, ReceiveClientArgs e);
    public delegate void SendDataClientHandler(object Sender, SendClientArgs e);
    public delegate void ConnectionStatusHandler(object Sender);

    public delegate void ErrorServerHandler(object Sender, ErrorServerArgs e);
    public delegate void MessageServerHandler(object Sender, ReceiveServerArgs e);
    public delegate void ConnectionHandler(object Sender, ClientConnectionArgs e);
    public delegate void SendDataServerHandler(object Sender, SendServerArgs e);
    public delegate void ServerStatusHandler(object Sender, ServerStatus e);
}
