using System.Text;
namespace Client;
class PacketBuilder
{
    MemoryStream _ms;
    public PacketBuilder()
    {
        _ms = new();
    }
    public void WriteOpCode(byte opcode)
    {
        _ms.WriteByte(opcode);
    }
    public void WriteMessage(string msg)
    {
        int msglength = msg.Length;
        _ms.Write(BitConverter.GetBytes(msglength));
        _ms.Write(Encoding.ASCII.GetBytes(msg));
    }
    public byte[] GetPacketBytes()
    {
        return _ms.ToArray();
    }
}