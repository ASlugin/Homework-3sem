namespace ControlWork2;

using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Class for client for chat
/// </summary>
public class Client
{
    private IPAddress ip;
    private int port;

    private CancellationTokenSource tokenSource;

    private List<Task> tasks;

    public Client(IPAddress ip, int port)
    {
        this.ip = ip;
        this.port = port;
        this.tokenSource = new CancellationTokenSource();
        this.tasks = new();
    }

    /// <summary>
    /// Starts client
    /// </summary>
    public async Task Start()
    {
        using var client = new TcpClient();

        await client.ConnectAsync(ip, port);
        var stream = client.GetStream();
        Console.WriteLine("Connection with server is established");

        Writer(stream);
        Reader(stream);

        await Task.WhenAll(tasks);
        client.Close();
        Console.WriteLine("Client is stopped");
    }

    private void Writer(NetworkStream stream)
    {
        tasks.Add(Task.Run(async () =>
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
            })
        );
    }

    private void Reader(NetworkStream stream)
    {
        tasks.Add(Task.Run(async () =>
            {
                var reader = new StreamReader(stream);
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var data = await reader.ReadLineAsync(tokenSource.Token);
                        Console.WriteLine($"Server: {data}");
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
            })
        );
    }
}
