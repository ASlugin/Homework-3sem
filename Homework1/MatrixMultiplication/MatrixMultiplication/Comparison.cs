namespace MatrixMultiplication;

using System.Diagnostics;

/// <summary>
/// Class for comparing matrix multiplication methods
/// </summary>
public class Comparison
{
    /// <summary>
    /// Perfoms comparing matrix multiplication methods
    /// </summary>
    public static void CompareMethods()
    {
        var initialSize = 100;
        var maxSize = 1000;
        var step = 100;

        var numberOfRunsForEachTest = 10;

        using StreamWriter file = new StreamWriter("../../../ResultOfСomparison.txt", false);
        file.WriteLine($"Number of runs for each test: {numberOfRunsForEachTest}\n");
        file.WriteLine("Size of matrices | Average time (sequental) | Standart deviation(sequental) | Average time (threading) | Standart deviation (threading)");
        for (int i = initialSize; i <= maxSize; i += step)
        {
            var firstMatrix = Matrix.CreateMatrix(i, i);
            var secondMatrix = Matrix.CreateMatrix(i, i);

            double mathExpectationSequental = 0;
            double mathExpectationThreading = 0;
            double standardDeviationSequental = 0;
            double standardDeviationThreading = 0;

            for (int j = 0; j < numberOfRunsForEachTest; j++)
            {
                var stopwatch = Stopwatch.StartNew();
                var matrix = Matrix.SequentialMultiplication(firstMatrix, secondMatrix);
                stopwatch.Stop();
                var timeSequential = stopwatch.Elapsed.TotalSeconds;
                mathExpectationSequental += timeSequential;
                standardDeviationSequental += timeSequential * timeSequential;

                stopwatch.Restart();
                matrix = Matrix.Multiplication(firstMatrix, secondMatrix);
                stopwatch.Stop();
                var timeThreading = stopwatch.Elapsed.TotalSeconds;
                mathExpectationThreading += timeThreading;
                standardDeviationThreading += timeThreading * timeThreading;
            }

            mathExpectationSequental /= numberOfRunsForEachTest;
            mathExpectationThreading /= numberOfRunsForEachTest;
            standardDeviationSequental /= numberOfRunsForEachTest;
            standardDeviationThreading /= numberOfRunsForEachTest;

            standardDeviationSequental -= mathExpectationSequental * mathExpectationSequental;
            standardDeviationThreading -= mathExpectationThreading * mathExpectationThreading;

            standardDeviationSequental = Math.Sqrt(standardDeviationSequental);
            standardDeviationThreading = Math.Sqrt(standardDeviationThreading);

            file.Write(i < 1000 ? $"     {i}x{i}     |" : $"    {i}x{i}    |");
            file.Write($"        {mathExpectationSequental:f5}sec        |          {standardDeviationSequental:f5}sec           |");
            file.WriteLine($"        {mathExpectationThreading:f5}sec        |          {standardDeviationThreading:f5}sec           ");
        }

    }

}
