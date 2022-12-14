namespace ControlWork2;

using System.Net;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 1)
        {
            var server = new Server(int.Parse(args[0]));
            await server.Start();
        }
        if (args.Length == 2)
        {
            var client = new Client(IPAddress.Parse(args[0]), int.Parse(args[1]));
            await client.Start();
        }
    }
}
