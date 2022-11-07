using ClientProgram;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        Client client = new(IPAddress.Parse("127.0.0.1"), 8888);
        client.List("./");
        client.List("./ololo/");
    }
}
