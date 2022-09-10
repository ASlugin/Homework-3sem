/// <summary>
/// Class for matrices and actions on matrices
/// </summary>
public class Matrix
{
    private int[,] matrix;

    /// <summary>
    /// Amount of rows in matrix
    /// </summary>
    public int Rows 
    {
        get { return this.matrix.GetLength(0); } 
    }

    /// <summary>
    /// Amount of columns in matrix
    /// </summary>
    public int Columns
    {
        get { return this.matrix.GetLength(1); }
    }

    /// <summary>
    /// Creates empty matrix
    /// </summary>
    /// <param name="rows">Amount of rows for new matrix</param>
    /// <param name="columns">Amount of columns for new matrix</param>
    public Matrix(int rows, int columns)
    {
        this.matrix = new int[rows, columns];
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

        this.matrix = new int[Int32.Parse(numbersFromFirstString[0]), Int32.Parse(numbersFromFirstString[1])];

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

    private static Random rand = new();

    /// <summary>
    /// Creates new matrix with random values
    /// </summary>
    /// <param name="rows">Amount of rows for matrix</param>
    /// <param name="columns">Amount of columns for matrix</param>
    public static Matrix CreateMatrix(int rows, int columns)
    {
        var matrix = new Matrix(rows, columns);
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

        for (int row = 0; row < firstMatrix.Rows; ++row)
        {
            for (int column = 0; column < secondMatrix.Columns; ++column)
            {
                for (int i = 0; i < firstMatrix.Columns; ++i)
                {
                    resultMatrix.matrix[row, column] += firstMatrix.matrix[row, i] * secondMatrix.matrix[i, column];
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

        var threads = firstMatrix.Rows < Environment.ProcessorCount ? new Thread[firstMatrix.Rows] : new Thread[Environment.ProcessorCount];
        var chunkSize = firstMatrix.Rows / threads.Length + 1;

        for (int i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() => 
                {
                    for (int row = localI * chunkSize; row < (localI + 1) * chunkSize && row < firstMatrix.Rows; ++row)
                    {
                        for (int column = 0; column < secondMatrix.Columns; ++column)
                        {
                            for (int k = 0; k < firstMatrix.Columns; ++k)
                            {
                                resultMatrix.matrix[row, column] += firstMatrix.matrix[row, k] * secondMatrix.matrix[k, column];
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
        using StreamWriter file = new(pathToResultFile, false);
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
