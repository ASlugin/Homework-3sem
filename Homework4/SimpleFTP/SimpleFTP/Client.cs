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

    public async Task<string> List(string path)
    {
        using (var client = new TcpClient())
        {
            await client.ConnectAsync(ip, port);
            var stream = client.GetStream();

            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(String.Concat("1 ", path));
            await writer.FlushAsync();

            var reader = new StreamReader(stream);
            var response = await reader.ReadLineAsync();
            if (response is null)
            {
                throw new Exception(); ////////
            }
            if (String.Compare(response.Split(' ')[0], "-1") == 0)
            {
                throw new DirectoryNotFoundException(); //////
            }
            return response;
        }
    }

    public async Task<string> Get(string path)
    {
        using (var client = new TcpClient())
        {
            await client.ConnectAsync(ip, port);
            var stream = client.GetStream();

            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync(String.Concat("2 ", path));
            await writer.FlushAsync();

            var reader = new StreamReader(stream); 
            var response = await reader.ReadLineAsync();
            if (response is null)
            {
                throw new Exception(); ////////
            }
            if (String.Compare(response.Split(' ')[0], "-1") == 0)
            {
                throw new FileNotFoundException(); //////
            }
            return response;
        }
    }

    public async void Execute(string request)
    {
        var command = request.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (command.Length != 2)
        {
            Console.WriteLine("Incorrect amount of arguments");
            return;
        }
        if (String.Compare(command[0], "1") == 0)
        {
            var result = await List(command[1]);
            Console.WriteLine(result);
        }
        else if (String.Compare(command[0], "2") == 0)
        {
            var result = await Get(command[1]);
            Console.WriteLine(result);
        }
        else
        {
            Console.WriteLine("Incorrect command");
        }
    }
}