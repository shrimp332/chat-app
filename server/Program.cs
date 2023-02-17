using System.Net.Sockets;
namespace ChatApp;
class Program
{
    static TcpListener? _listener;
    public static List<Client>? _users;

    static void Main()
    {
        _users = new();
        _listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 8483);
        _listener.Start();
        Console.WriteLine("Server listening on port 8483");
        while (true)
        {
            listenForJoin();
        }

    }
    static void listenForJoin()
    {
        TcpClient handler = _listener.AcceptTcpClient();
        Client client = new(handler);
        _users.Add(client);
        BroadcastAlert($"[{DateTime.Now}] [Alert] [{client.username}] has joined the chat");
    }
    public static void BroadcastMessageBytes(byte[] message, int length)
    {
        PacketBuilder broadcastPacket = new();
        broadcastPacket.WriteOpCode(1);
        broadcastPacket.WriteBytes(message, length);
        Broadcast(broadcastPacket.GetPacketBytes());
    }
    public static void BroadcastAlert(string message)
    {
        PacketBuilder broadcastPacket = new();
        broadcastPacket.WriteOpCode(2);
        broadcastPacket.WriteMessage(message);
        Broadcast(broadcastPacket.GetPacketBytes());
    }
    public static void Broadcast(Byte[] message)
    {
        if (_users.Count() == 0) return;
        foreach (Client user in _users)
        {
                user.clientSocket.Client.Send(message);
        }
    }
}