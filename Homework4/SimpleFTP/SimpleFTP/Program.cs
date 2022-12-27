using ClientProgram;
using System.Net;

if (args.Length != 2)
{
    Console.WriteLine("Need to give IP adress and port");
    return;
}
if (!IPAddress.TryParse(args[0], out IPAddress? ip))
{
    Console.WriteLine("IPAdress is incorrect");
    return;
}
if (!Int32.TryParse(args[1], out int port) && port <= 65535 && port >= 0)
{
    Console.WriteLine("Port is incorrect");
    return;
}

Client client = new(ip, port);
while (true)
{
    var request = Console.ReadLine();
    if (request is null)
    {
        continue;
    }

    var command = request.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (command.Length != 2)
    {
        Console.WriteLine("Incorrect amount of arguments");
        continue;
    }
    if (String.Compare(command[0], "1") == 0)
    {
        try
        {
            var result = await client.List(command[1]);
            Console.WriteLine(result);
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Such directory doesn't exist");
        }
    }
    else if (String.Compare(command[0], "2") == 0)
    {
        try
        {
            var result = await client.Get(command[1]);
            Console.WriteLine("File is downoloaded");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Such file doesn't exist");
        }
    }
    else
    {
        Console.WriteLine("Incorrect command");
    }
}
