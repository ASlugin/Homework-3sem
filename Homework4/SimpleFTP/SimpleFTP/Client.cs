namespace ClientProgram;

using System.Buffers.Text;
using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Class for client
/// </summary>
public class Client
{
    private string directoryToDownloadFile = "../../../../Download/";
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
        using var client = new TcpClient();
        await client.ConnectAsync(ip, port);
        var stream = client.GetStream();
        var writer = new StreamWriter(stream);
        var reader = new StreamReader(stream);
        
        await writer.WriteLineAsync($"1 {path}");
        await writer.FlushAsync();

        var response = await reader.ReadLineAsync();
        if (String.Compare(response!.Split(' ')[0], "-1") == 0)
        {
            throw new DirectoryNotFoundException();
        }
        return response;
    }

    /// <summary>
    /// Returns data about file
    /// </summary>
    /// <param name="path">Path where file is located</param>
    public async Task<string> Get(string path)
    {
        using var client = new TcpClient();
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

        var pathSplitBySlash = path.Split('/');
        var pathSplitByBackSlash = pathSplitBySlash[pathSplitBySlash.Length -1].Split('\\');
        var fileName = pathSplitByBackSlash[pathSplitByBackSlash.Length - 1];

        var bytesForFile = Convert.FromBase64String(response.Split(' ')[1]);
        await File.WriteAllBytesAsync(String.Concat(directoryToDownloadFile, fileName), bytesForFile);
            
        return response;
    }
}