namespace ControlWork2;

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

    public async Task Start()
    {
        using (var client = new TcpClient())
        {
            client.Connect(ip, port);
            var stream = client.GetStream();
            Console.WriteLine("Соединение установлено");

            var writer = new StreamWriter(stream);
            var reader = new StreamReader(stream);

            while (true)
            {
                /*
                await Task.Run(async () =>
                {
                    var reader = new StreamReader(stream);

                    var data = await reader.ReadLineAsync();
                    Console.WriteLine(data);
                });*/

                await Task.Run(async () =>
                {
                    while (true)
                    {
                        var message = Console.ReadLine();
                        await writer.WriteLineAsync(message);
                        await writer.FlushAsync();
                    }
                });
            }
        }

        Console.WriteLine("Client stopped");
    }

}
