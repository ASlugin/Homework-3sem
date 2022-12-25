using ControlWork2;
using System.Net;

if (args.Length == 1)
{
    var server = new Server(int.Parse(args[0]));
    await server.Start();
}
else if (args.Length == 2)
{
    var client = new Client(IPAddress.Parse(args[0]), int.Parse(args[1]));
    await client.Start();
}
