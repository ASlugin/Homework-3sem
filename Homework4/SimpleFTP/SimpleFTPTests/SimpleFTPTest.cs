namespace SimpleFTPTests;

using ServerProgram;
using ClientProgram;
using System.Net;
using System.IO;

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
    public async Task ListOfFilesAndDirectoriesTest()
    {
        const string path = "../../../TestDirectory";
        var result = await client.List(path);
        Assert.IsTrue(String.Compare(result, $"2 {path}\\Test.txt false {path}\\Directory true") == 0);
    }

    [Test]
    public async Task ListOfFilesTest()
    {
        const string path = "../../../TestDirectory/Directory";
        var result = await client.List(path);
        Assert.IsTrue(String.Compare(result, $"1 {path}\\Test2.txt false") == 0);
    }

    [Test]
    public void ListShallThrowDirectoryNotFoundExceptionIfDirectoryIsNotExist()
    {
        const string path = "../../../NonExistentDirectory";
        string result;
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => result = await client.List(path));
    }

    [Test]
    public void GetShallThrowFileNotFoundExceptionIfFileIsNotExist()
    {
        const string path = "../../../TestDirectory/NonExistentFile.txt";
        string result;
        Assert.ThrowsAsync<FileNotFoundException>(async () => result = await client.Get(path));
    }
    
    [Test]
    public async Task GetShallWorkCorrectly()
    {
        const string path = "../../../TestDirectory/Directory/Test2.txt";
        var file = await File.ReadAllBytesAsync(path);
        string expectedResult = $"{file.Length} {Convert.ToBase64String(file)}";
        var result = await client.Get(path);
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task GivenFileAndDownoloadedFileShallBeEqual()
    {
        const string pathFile = "../../../TestDirectory/Test.txt";
        const string pathDownloadedFile = "../../../../Download/Test.txt";
        await client.Get(pathFile);
        var file = File.ReadAllBytes(pathFile);
        var downloadedFile = File.ReadAllBytes(pathDownloadedFile);
        Assert.That(downloadedFile, Is.EqualTo(file));
    }
}
