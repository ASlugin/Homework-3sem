namespace MatrixMultiplication;

using System;
using System.Threading;

/// <summary>
/// Class for matrices and actions on matrices
/// </summary>
public class Matrix
{
    private int[,] matrix;

    /// <summary>
    /// Amount of rows in matrix
    /// </summary>
    public int Rows { get; private set; }

    /// <summary>
    /// Amount of columns in matrix
    /// </summary>
    public int Columns { get; private set; }


    /// <summary>
    /// Creates empty matrix with amount rows and columns
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    public Matrix(int rows, int columns)
    {
        this.matrix = new int[rows, columns];
        Rows = rows;
        Columns = columns;
    }

    /// <summary>
    /// Creates new matrix from file
    /// </summary>
    public Matrix(string pathToFile)
    {
        using StreamReader file = new(pathToFile);
        var firstString = file.ReadLine();
        if (firstString == null)
        {
            throw new ArgumentException("File with matrix is empty");
        }
        var numbersFromFirstString = firstString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (numbersFromFirstString.Length != 2)
        {
            throw new ArgumentException("Number of matrix size arguments is incorrect");
        }

        Rows = Int32.Parse(numbersFromFirstString[0]);
        Columns = Int32.Parse(numbersFromFirstString[1]);
        this.matrix = new int[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            var str = file.ReadLine();
            if (str == null)
            {
                throw new ArgumentException("Matrix is incorrect");
            }
            var numbers = str.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (numbers.Length != Columns)
            {
                throw new ArgumentException("Matrix is incorrect");
            }
            for (int j = 0; j < Columns; j++)
            {
                matrix[i, j] = Int32.Parse(numbers[j]);
            }
        }
    }

    /// <summary>
    /// Creates new matrix with random values
    /// </summary>
    /// <param name="rows">Amount of rows for matrix</param>
    /// <param name="columns">Amount of columns for matrix</param>
    public static Matrix CreateMatrix(int rows, int columns)
    {
        var matrix = new Matrix(rows, columns);
        var rand = new Random();
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < columns; ++j)
            {
                matrix.matrix[i, j] = rand.Next() % 1000;
            }
        }
        return matrix;
    }

    /// <summary>
    /// Performs sequential matrix multiplication
    /// </summary>
    /// <returns>Resulting matrix</returns>
    public static Matrix SequentialMultiplication(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.Columns != secondMatrix.Rows)
        {
            throw new ArgumentException("Matrices cannot be multiplied");
        }
        Matrix resultMatrix = new(firstMatrix.Rows, secondMatrix.Columns);

        for (int i = 0; i < firstMatrix.Rows; ++i)
        {
            for (int j = 0; j < secondMatrix.Columns; ++j)
            {
                for (int k = 0; k < firstMatrix.Columns; ++k)
                {
                    resultMatrix.matrix[i, j] += firstMatrix.matrix[i, k] * secondMatrix.matrix[k, j];
                }
            }
        }
        return resultMatrix;
    }

    /// <summary>
    /// Performs multithreaded matrix multiplication
    /// </summary>
    /// <returns>Resulting matrix</returns>
    public static Matrix Multiplication(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.Columns != secondMatrix.Rows)
        {
            throw new ArgumentException("Matrices cannot be multiplied");
        }
        Matrix resultMatrix = new(firstMatrix.Rows, secondMatrix.Columns);

        var threads = new Thread[Environment.ProcessorCount];
        var chunkSize = firstMatrix.Rows / threads.Length + 1;

        for (int i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() => 
                {
                    for (int j = localI * chunkSize; j < (localI + 1) * chunkSize && j < firstMatrix.Rows; ++j)
                    {
                        for (int g = 0; g < secondMatrix.Columns; ++g)
                        {
                            for (int k = 0; k < firstMatrix.Columns; ++k)
                            {
                                resultMatrix.matrix[j, g] += firstMatrix.matrix[j, k] * secondMatrix.matrix[k, g];
                            }
                        }
                    }
                });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }

        return resultMatrix;
    }

    /// <summary>
    /// Writes matrix to file
    /// </summary>
    public void WriteMatrixToFile(string pathToResultFile)
    {
        using StreamWriter file = new StreamWriter(pathToResultFile, false);
        file.WriteLine($"{Rows} {Columns}");
        for (int i = 0; i < Rows; ++i)
        {
            for (int j = 0; j < Columns; ++j)
            {
                file.Write($"{matrix[i, j]} ");
            }
            file.Write("\n");
        }
    }

    /// <summary>
    /// Check whether matrices are equal
    /// </summary>
    /// <returns>Returns true is matrices are equal, else returns false</returns>
    public static bool AreEqual(Matrix firstMatrix, Matrix secondMatrix)
    {
        if (firstMatrix.Rows != secondMatrix.Rows || firstMatrix.Columns != secondMatrix.Columns)
        {
            return false;
        }
        
        for (int i = 0; i < firstMatrix.Rows; ++i)
        {
            for (int j = 0; j < firstMatrix.Columns; ++j)
            {
                if (firstMatrix.matrix[i, j] != secondMatrix.matrix[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }
}
