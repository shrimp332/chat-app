using System.Net.Sockets;
namespace Client;
class Program
{
    static TcpClient? _client;
    static string? username;
    static string? str;
    static void Main()
    {
        Console.WriteLine("Please enter a username");
        while (true)
        {
            username = Console.ReadLine();
            if (username != null && username.Length > 0)
            {
                break;
            }
            ClearLastLine();
            Console.WriteLine("You must enter a username");
        }
        ConnectToServer(username);
        while (true)
        {
            str = "";
            ReadText();
            Console.WriteLine("");
            ClearLastLine();
            SendMessage(str);
        }
    }

    static bool connected;
    static void Process()
    {
        if (_client == null) return;
        PacketReader _packetReader = new(_client.GetStream());
        while (true)
        {
            try
            {
                byte opcode = _packetReader.ReadByte();
                switch (opcode)
                {
                    case 1:
                        string message = _packetReader.ReadMessage();
                        ClearCurrentLine();
                        Console.WriteLine(message);
                        WriteToConsole();
                        break;
                    case 2:
                        string serverMessage = _packetReader.ReadMessage();
                        ClearCurrentLine();
                        Console.WriteLine(serverMessage);
                        WriteToConsole();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                connected = false;
                _client.Dispose();
                ConnectionFailed();
                break;
            }
        }
    }
    static void ConnectToServer(string username)
    {
        try
        {
            if (connected) return;
            _client = new();
            _client.Connect("127.0.0.1", 8483);
            PacketBuilder connectPacket = new PacketBuilder();
            connectPacket.WriteOpCode(0);
            connectPacket.WriteMessage(username);
            _client.Client.Send(connectPacket.GetPacketBytes());
            connected = true;
            Task.Run(() => Process());
        }
        catch (Exception)
        {
            ConnectionFailed();
        }
    }
    static void ConnectionFailed()
    {
        if (!connected && username != null)
        {
            Thread.Sleep(3000);
            Console.WriteLine("Connection Failed");
            Thread.Sleep(2000);
            Console.WriteLine("Retrying Connection");
            ConnectToServer(username);
        }
    }
    static void SendMessage(string message)
    {
        if (_client == null)
        {
            ConnectionFailed();
            return;
        }
        try
        {
            PacketBuilder messagePacket = new();
            messagePacket.WriteOpCode(1);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
        catch (Exception)
        {
            ConnectionFailed();
        }
    }
    public static void ClearLastLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.BufferWidth));
        Console.SetCursorPosition(0, Console.CursorTop - 1);
    }
    public static void ClearCurrentLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.BufferWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
    }
    public static void ReadText()
    {
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            if (key.Key == ConsoleKey.Backspace)
            {
                if (str == null || str.Length == 0) continue;
                str = str.Remove(str.Length - 1);
                WriteToConsole();
                continue;
            }
            str += key.KeyChar;
            WriteToConsole();
        }
    }
    public static void WriteToConsole()
    {
        ClearCurrentLine();
        Console.Write(str);
    }
}