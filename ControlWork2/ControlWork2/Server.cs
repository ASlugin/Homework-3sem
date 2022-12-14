namespace ControlWork2;

using System.Net;
using System.Net.Sockets;

public class Server
{
    private int port;
    
    public Server (int port)
    {
        this.port = port;
    }

    public async Task Start()
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Server started on port {port}...");
        
        var socket = await listener.AcceptSocketAsync();
        Console.WriteLine("Соединение с клиентом установлено");

        var stream = new NetworkStream(socket);
        var reader = new StreamReader(stream);
        var writer = new StreamWriter(stream);
        while (true)
        { 
            await Task.Run(async () =>
            {
                while (true)
                {
                    var data = await reader.ReadLineAsync();
                    if (data != null)
                    {
                        Console.WriteLine(data);
                    }
                }
            });

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
        socket.Close();
        Console.WriteLine("Соединение с клиентом прервано");

    }

}
