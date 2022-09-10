public class Tests
{
    [TestCase(50, 50, 50, 50)]
    [TestCase(10, 5, 5, 10)]
    [TestCase(23, 100, 100, 97)]
    public void AllMethodsOfMultiplicationShallReturnEqualMatrices(int rows1, int columns1, int rows2, int columns2)
    {
        var firstMatrix = Matrix.CreateMatrix(rows1, columns1);
        var secondMatrix = Matrix.CreateMatrix(rows2, columns2);

        var resultMatrixThreading = Matrix.Multiplication(firstMatrix, secondMatrix);
        var resultMatrixSequental = Matrix.SequentialMultiplication(firstMatrix, secondMatrix);

        Assert.IsTrue(Matrix.AreEqual(resultMatrixSequental, resultMatrixThreading));
    }

    [TestCase(5, 7, 5, 7)]
    [TestCase(3, 4, 5, 6)]
    [TestCase(10, 3, 10, 10)]
    public void IfMatricesCannotBeMultipliedMethodsShallThrowException(int rows1, int columns1, int rows2, int columns2)
    {
        var firstMatrix = Matrix.CreateMatrix(rows1, columns1);
        var secondMatrix = Matrix.CreateMatrix(rows2, columns2);

        Assert.Throws<ArgumentException>(() => Matrix.Multiplication(firstMatrix, secondMatrix));
        Assert.Throws<ArgumentException>(() => Matrix.SequentialMultiplication(firstMatrix, secondMatrix));
    }
}