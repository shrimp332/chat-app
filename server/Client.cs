using System.Net.Sockets;
using System.Text;
namespace ChatApp;
class Client
{
    public string username { get; set; }
    public Guid UID { get; set; }
    public TcpClient clientSocket { get; set; }
    PacketReader _packetReader;
    public Client(TcpClient clientSocket)
    {
        this.clientSocket = clientSocket;
        this.UID = Guid.NewGuid();
        _packetReader = new PacketReader(clientSocket.GetStream());
        int opcode = _packetReader.ReadByte();
        username = _packetReader.ReadMessage();
        Console.WriteLine($"[{DateTime.Now}] [{username}] connected");

        Task.Run(() => Process());
    }
    

    void Process()
    {
        while (true)
        {
            try
            {
                byte opcode = _packetReader.ReadByte();
                switch (opcode)
                {
                    case 1:
                        int length = _packetReader.ReadInt32();
                        List<byte> message = _packetReader.ReadMessageBytes(length).ToList();
                        string detailstr = $"[{DateTime.Now}] {username}: ";
                        length += detailstr.Length;
                        List<byte> details = Encoding.ASCII.GetBytes(detailstr).ToList();
                        List<byte> concatList = details.Concat(message).ToList();
                        byte[] toSend = concatList.ToArray();
                        Program.BroadcastMessageBytes(toSend, length);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"[{DateTime.Now}] [{username}] disconnected");
                clientSocket.Dispose();
                string tempname = this.username;
                Program._users.Remove(this);
                Program.BroadcastAlert($"[{DateTime.Now}] [Alert] [{username}] disconnected");
                
                break;
            }
        }
    }
}