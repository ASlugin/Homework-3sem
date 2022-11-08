using ServerProgram;

using System.Net;
using System.Threading;

internal class Program
{
    private static void Main(string[] args)
    {
        Server server = new(IPAddress.Parse("127.0.0.1"), 8888);
        server.Start();
        Console.WriteLine("Enter 'stop' to stop server");

        while (true)
        {
            if (String.Compare(Console.ReadLine(), "stop") == 0)
            {
                server.Stop();
                break;
            }
        }
    }
}