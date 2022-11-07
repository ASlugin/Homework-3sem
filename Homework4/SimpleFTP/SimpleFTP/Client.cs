namespace ClientProgram;

using System.Net;
using System.Net.Sockets;

public class Client
{
    private IPAddress ip;
    private int port;

    public Client(IPAddress ip, int port)
    {
        this.ip = ip;
        this.port = port;
    }

    public async void List(string path)
    {
        using (var client = new TcpClient("localhost", port))
        {
            var stream = client.GetStream();

            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(String.Concat("1 ", path));
            await writer.FlushAsync();

            var reader = new StreamReader(stream);
            var data = await reader.ReadLineAsync();
            Console.WriteLine(data);
        }
    }
}