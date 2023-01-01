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
    private Task task = new(() => { });

    [OneTimeSetUp]
    public void Setup()
    {
        task = server.Start();
    }

    [OneTimeTearDown]
    public void TearDownAfterAllTests()
    {
        server.Stop();
        task.Wait();
    }

    [TearDown]
    public void TearDown()
    {
        var directoryForRemoveAllFiles = new DirectoryInfo("../../../../Download/");
        foreach (var file in directoryForRemoveAllFiles.GetFiles())
        {
            file.Delete();
        }
    }

    [Test]
    public async Task ListOfFilesAndDirectoriesTest()
    {
        const string path = "../../../TestDirectory";
        var result = await client.List(path);
        Assert.That(result, Is.EqualTo($"2 {path}\\Test.txt false {path}\\Directory true"));
    }

    [Test]
    public async Task ListOfFilesTest()
    {
        const string path = "../../../TestDirectory/Directory";
        var result = await client.List(path);
        Assert.That(result, Is.EqualTo($"2 {path}\\Test1.txt false {path}\\Test2.txt false"));
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
    public async Task GivenFileAndDownloadedFileShallBeEqual()
    {
        const string pathFile = "../../../TestDirectory/Test.txt";
        const string pathDownloadedFile = "../../../../Download/Test.txt";
        await client.Get(pathFile);
        var file = File.ReadAllBytes(pathFile);
        var downloadedFile = File.ReadAllBytes(pathDownloadedFile);
        Assert.That(downloadedFile, Is.EqualTo(file));
    }
    
    [Test]
    public async Task SeveralClientShallWorkWithServerCorrecltly()
    {
        var client1 = new Client(IPAddress.Parse(ip), port);
        var client2 = new Client(IPAddress.Parse(ip), port);

        var clientList = new List<Client>() { client1, client2 };

        var taskList = new List<Task>();
        for (int i = 1; i <= clientList.Count; i++)
        {
            var localI = i;
            taskList.Add(Task.Run(async () =>
            {
                string path = $"../../../TestDirectory/Directory/Test{localI}.txt";
                var result = await client.Get(path);
            }));
        } 
        await Task.WhenAll(taskList);

        for (int i = 1; i <= taskList.Count; i++)
        {
            string pathFile = $"../../../TestDirectory/Directory/Test{i}.txt";
            string pathDownloadedFile = $"../../../../Download/Test{i}.txt";
            await client.Get(pathFile);
            var file = File.ReadAllBytes(pathFile);
            var downloadedFile = File.ReadAllBytes(pathDownloadedFile);
            Assert.That(downloadedFile, Is.EqualTo(file));
        }
    }
}
