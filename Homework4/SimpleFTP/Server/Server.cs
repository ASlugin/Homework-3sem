namespace ServerProgram;

using System.Net;
using System.Net.Sockets;

public class Server
{
    private TcpListener listener;
    private CancellationTokenSource tokenSource;
    private List<Task> requests;

    private int port;

    public Server(IPAddress ip, int port)
    {
        this.listener = new(IPAddress.Parse("127.0.0.1"), port);
        this.port = port;
        this.tokenSource = new();
        this.requests = new();
    }

    public async Task Start()
    {
        listener.Start();
        Console.WriteLine($"Server started on port {port}...");
        while (!tokenSource.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync();
            requests.Add(Task.Run(async () =>
            {
                var stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream);

                var request = (await reader.ReadLineAsync())?.Split(' ');
                if (request is null)
                {
                    throw new Exception();  ////
                }
                if (String.Compare(request[0], "1") == 0)
                {
                    await List(request[1], writer);
                }
                else if (String.Compare(request[0], "2") == 0)
                {
                    await Get(request[1], writer);
                }

                socket.Close();
            }));
        }
        await Task.WhenAll(requests);
        listener.Stop();
    }

    public void Stop()
    {
        tokenSource.Cancel();
    }

    private async Task List(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
            return;
        }

        var files = Directory.GetFiles(path);
        var directories = Directory.GetDirectories(path);

        var result = (files.Length + directories.Length).ToString();
        foreach (var file in files)
        {
            result = String.Concat(result, " ", file, " false");
        }
        foreach (var directory in directories)
        {
            result = String.Concat(result, " ", directory, " true");
        }
        await writer.WriteLineAsync(result);
        await writer.FlushAsync();
    }

    private async Task Get(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
            return;
        }
    }

}
