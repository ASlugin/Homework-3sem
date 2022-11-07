using ServerProgram;
using ClientProgram;

using System.Net;
using System.Threading;

internal class Program
{
    private static void Main(string[] args)
    {
        var server = new Server(IPAddress.Parse("127.0.0.1"), 8888);
        server.Start();

        while(true)
        {
            if (String.Compare(Console.ReadLine(), "stop") == 0)
            {
                server.Stop();
                break;
            }
        }
    }
}