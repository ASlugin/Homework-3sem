namespace SimpleFTPTests;

using ServerProgram;
using ClientProgram;
using System.Net;

public class Tests
{
    private const string ip = "127.0.0.1";
    private const int port = 8888;
    private Client client = new(IPAddress.Parse(ip), port);
    private Server server = new(IPAddress.Parse(ip), port);

    [OneTimeSetUp]
    public void Setup()
    {
        server.Start();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        server.Stop();
    }

    [Test]
    public async Task ListOfFilesTest()
    {
        var result = await client.List("1 ../../../../SimpleFtpTests/TestDirectory");
        Assert.That(0, Is.EqualTo(String.Compare(result, "2 ../../../../SimpleFtpTests/TestDirectory\\Test.txt false ../../../../SimpleFtpTests/TestDirectory\\Directory true")));
    }
}