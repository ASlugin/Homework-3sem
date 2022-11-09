namespace ControlWork;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Class for asynchronously computes hash from files and from directories
/// </summary>
public static class MD5MultiThread
{
    /// <summary>
    /// Asynchronously computes hash from given file 
    /// </summary>
    public static async Task<byte[]> HashFromFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }
        using var md5 = MD5.Create();
        var hash = await md5.ComputeHashAsync(new FileStream(path, FileMode.Open), new CancellationToken());
        return hash;
    }

    /// <summary>
    /// Asynchronously compuutes hash from given directory
    /// </summary>
    /// <returns>Array of bytes</returns>
    public static async Task<byte[]> HashFromDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException();
        }
        using var md5 = MD5.Create();
        var arrayToComputeHash = await ByteArrayFromDirectoryRecursive(path);
        return arrayToComputeHash;
        //return await md5.ComputeHashAsync(new Stream(), new CancellationToken());
    }

    private static async Task<byte[]> ByteArrayFromDirectoryRecursive(string path)
    {
        byte[] arrayToComputeHash = Encoding.UTF8.GetBytes(path);
        var directories = Directory.GetDirectories(path);
        foreach (var directory in directories)
        {
            arrayToComputeHash = arrayToComputeHash.Concat<byte>(await ByteArrayFromDirectoryRecursive(directory)).ToArray();
        }
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            arrayToComputeHash = arrayToComputeHash.Concat<byte>(await File.ReadAllBytesAsync(file)).ToArray();
        }
        return arrayToComputeHash;
    }

}
