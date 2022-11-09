namespace ControlWork;

using System.Security.Cryptography;

public class Program
{
    public static async Task Main(string[] args)
    {
        /*
        var resultFile = MD5SingleThread.HashFromFile("../../../../Directory/test.txt");
        foreach (var item in resultFile)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
        */
        var resultDirectory = MD5SingleThread.HashFromDirectory("../../../../Directory");
        foreach (var item in resultDirectory)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
        /*
        var resultFileMultiThread = await MD5MultiThread.HashFromFile("../../../../Directory/test.txt");
        foreach (var item in resultFileMultiThread)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
        */
        var resultDirectoryMultiThread = await MD5MultiThread.HashFromDirectory("../../../../Directory");
        foreach (var item in resultDirectoryMultiThread)
        {
            Console.Write($"{item} ");
        }
    }
}