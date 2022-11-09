namespace ControlWork;

using System.Security.Cryptography;

public class Program
{
    public static async Task Main(string[] args)
    {
        var resultFile = MD5SingleThread.HashFromFile("../../../../Directory/test.txt");
        foreach (var item in resultFile)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
        var resultDirectory = MD5SingleThread.HashFromDirectory("../../../../Directory");

        var resultFileMultiThread = await MD5MultiThread.HashFromFile("../../../../Directory/test.txt");
        foreach (var item in resultFileMultiThread)
        {
            Console.Write($"{item} ");
        }
        
    }
}