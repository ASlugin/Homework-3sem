namespace ClientProgram;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class for client
/// </summary>
public class Client
{
    private IPAddress ip;
    private int port;

    public Client(IPAddress ip, int port)
    {
        this.ip = ip;
        this.port = port;
    }

    /// <summary>
    /// Returns list of files and directories in given path
    /// </summary>
    /// <returns>String where amount of file and directories and list of them</returns>
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
            if (String.Compare(response!.Split(' ')[0], "-1") == 0)
            {
                throw new DirectoryNotFoundException();
            }
            return response;
        }
    }

    /// <summary>
    /// Returns data about file
    /// </summary>
    /// <param name="path">Path where file is located</param>
    /// <returns>String with length of file and array of bytes of file</returns>
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
            if (String.Compare(response!.Split(' ')[0], "-1") == 0)
            {
                throw new FileNotFoundException();
            }
            return response;
        }
    }

    /// <summary>
    /// Method for work with client from console
    /// </summary>
    /// <param name="request">Command for client, what is requested from server</param>
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