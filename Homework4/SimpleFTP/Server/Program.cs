using ServerProgram;
using System.Net;

if (args.Length != 2)
{
    Console.WriteLine("Need to give IP adress and port");
    return;
}
if (!IPAddress.TryParse(args[0], out IPAddress? ip))
{
    Console.WriteLine("IPAdress is incorrect");
    return;
}
if (!Int32.TryParse(args[1], out int port) && port <= 65535 && port >= 0)
{
    Console.WriteLine("Port is incorrect");
    return;
}

Server server = new(ip, port);
await server.Start();
Console.WriteLine("Enter 'stop' to stop server");

while (true)
{
    if (String.Compare(Console.ReadLine(), "stop") == 0)
    {
        server.Stop();
        break;
    }
}