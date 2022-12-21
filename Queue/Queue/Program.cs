namespace Queue;

public class Program
{
    public static void Main(string[] args)
    {
        var queue = new QueueWithPriority<int>();
        queue.Enqueue(1, 2);
        queue.Enqueue(2, 10);
        queue.Enqueue(3, 5);
        queue.Enqueue(4, 5);

        Console.WriteLine(queue.Dequeue());
    }
}