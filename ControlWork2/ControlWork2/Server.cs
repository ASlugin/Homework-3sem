namespace ControlWork2;

using System.Net;
using System.IO;
using System.Net.Sockets;

/// <summary>
/// Class for server for chat
/// </summary>
public class Server
{
    private int port;
    private TcpListener listener;

    private CancellationTokenSource tokenSource; 
    
    public Server (int port)
    {
        this.port = port;
        this.listener = new TcpListener(IPAddress.Any, port);
        this.tokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Starts server
    /// </summary>
    /// <returns></returns>
    public async Task Start()
    {
        listener.Start();
        Console.WriteLine($"Server is started on port {port}...");

        while (!tokenSource.Token.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync();
            var stream = client.GetStream();
            Console.WriteLine("Connection with client is established");
            
            Writer(stream);
            Reader(stream);
        }

        listener.Stop();
        Console.WriteLine("Server is stopped");
    }

    private void Writer(NetworkStream stream)
    {
        Task.Run(async () =>
        {
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            while (!tokenSource.Token.IsCancellationRequested)
            {
                var message = Console.ReadLine();
                await writer.WriteLineAsync(message);
                if (String.Compare(message, "exit") == 0)
                {
                    tokenSource.Cancel();
                }
            }
            writer.Close();
        });
    }

    private void Reader(NetworkStream stream)
    {
        Task.Run(async () =>
        {
            var reader = new StreamReader(stream);
            while (!tokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var data = await reader.ReadLineAsync(tokenSource.Token);
                    Console.WriteLine($"Client: {data}");
                    if (String.Compare(data, "exit") == 0)
                    {
                        tokenSource.Cancel();
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            reader.Close();
        });
    }
}
