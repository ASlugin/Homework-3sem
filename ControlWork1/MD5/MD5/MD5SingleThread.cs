namespace ControlWork;

using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Class for computes hash from files and from directories
/// </summary>
public static class MD5SingleThread
{
    /// <summary>
    /// Computes hash from given file
    /// </summary>
    /// <returns>Array of bytes</returns>
    public static byte[] HashFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }
        using var md5 = MD5.Create();
        var file = File.ReadAllBytes(path);
        return md5.ComputeHash(file);
    }

    /// <summary>
    /// Compuutes hash from given directory
    /// </summary>
    /// <returns>Array of bytes</returns>
    public static byte[] HashFromDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }
        using var md5 = MD5.Create();
        var arrayToComputeHash = ByteArrayFromDirectoryRecursive(path);
        return md5.ComputeHash(arrayToComputeHash);
    }

    private static byte[] ByteArrayFromDirectoryRecursive(string path)
    {
        byte[] arrayToComputeHash = Encoding.UTF8.GetBytes(path);
        var directories = Directory.GetDirectories(path);
        foreach (var directory in directories)
        {
            arrayToComputeHash = arrayToComputeHash.Concat<byte>(ByteArrayFromDirectoryRecursive(directory)).ToArray();
        }
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            arrayToComputeHash =  arrayToComputeHash.Concat<byte>(File.ReadAllBytes(file)).ToArray();
        }
        return arrayToComputeHash;
    }
}
