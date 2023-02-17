using System.Net.Sockets;
using System.Text;
namespace ChatApp;
class PacketReader : BinaryReader
{
    public NetworkStream _ns { get; set; }
    public PacketReader(NetworkStream ns) : base(ns)
    {
        this._ns = ns;
    }

    public string ReadMessage()
    {
        byte[] msgBuffer;
        int length = ReadInt32();
        msgBuffer = new byte[length];
        _ns.Read(msgBuffer, 0, length);
        string msg = Encoding.ASCII.GetString(msgBuffer);
        return msg;
    }
    public byte[] ReadMessageBytes(int length)
    {
        byte[] msgBuffer = new byte[length];
        _ns.Read(msgBuffer, 0, length);
        return msgBuffer;
    }
}