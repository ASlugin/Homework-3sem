namespace MatrixMultiplication;

class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Run a comparison of multiplication methods? It will take some time\n1 - yes, 0 - no");
        var runComparison = Console.ReadLine();
        
        try
        {
            if (String.Compare(runComparison, "1") == 0)
            {
                Comparison.CompareMethods();
                Console.WriteLine("The result of the comparison is in the file \"ResultOfComparison.txt\"");
            }

            var firstMatrix = new Matrix("../../../FirstMatrix.txt");
            var secondMatrix = new Matrix("../../../SecondMatrix.txt");
            var result = Matrix.Multiplication(firstMatrix, secondMatrix);
            result.WriteMatrixToFile("../../../ResultMatrix.txt");
        }
        catch (FileNotFoundException exception)
        {
            Console.WriteLine(exception.Message);
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
        }

    }
}
