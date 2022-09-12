using MatrixMultiplication;

try
{
    if (args.Length > 0)
    {
        if (String.Compare(args[0], "runComparison") == 0)
        {
            Comparison.CompareMethods();
            Console.WriteLine("The result of the comparison is in the file \"ResultOfComparison.txt\"");
        }
    }

    var firstMatrix = new Matrix("FirstMatrix.txt");
    var secondMatrix = new Matrix("SecondMatrix.txt");
    var result = Matrix.Multiplication(firstMatrix, secondMatrix);
    result.WriteMatrixToFile("ResultMatrix.txt");
}
catch (FileNotFoundException exception)
{
    Console.WriteLine(exception.Message);
}
catch (ArgumentException exception)
{
    Console.WriteLine(exception.Message);
}
