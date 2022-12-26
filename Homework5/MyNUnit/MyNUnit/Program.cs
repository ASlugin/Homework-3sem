using MyNUnitSpace;

if (args.Length != 1)
{
    Console.WriteLine("Args should provide one path to test");
    return;
}

if (!Directory.Exists(args[0]))
{
    throw new DirectoryNotFoundException("There is no such directory");
}
var nUnit = new MyNUnit();
nUnit.RunTests(args[0]);
nUnit.PrintResult();