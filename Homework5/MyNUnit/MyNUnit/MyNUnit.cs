namespace MyNUnitSpace;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Attributes;

/// <summary>
/// Class for testing
/// </summary>
public class MyNUnit
{
    /// <summary>
    /// Collection with test results
    /// </summary>
    public ConcurrentBag<TestInfo> TestResults { get; set; }

    public MyNUnit()
    {
        TestResults = new();
    }
    
    /// <summary>
    /// Prints result of all test methods to console
    /// </summary>
    public void PrintResult()
    {
        foreach (var test in TestResults)
        {
            switch (test.Result)
            {
                case TestInfo.TestState.Passed:
                    Console.WriteLine($"Test {test.Name} passed. Elapsed time: {test.Time.TotalMilliseconds} ms.");
                    break;
                case TestInfo.TestState.Failed:
                    Console.Write($"Test {test.Name} failed. Elapsed time: {test.Time.TotalMilliseconds} ms.");
                    if (test.Message is not null)
                    {
                        Console.Write($" {test.Message}");
                    }
                    Console.WriteLine();
                    break;
                case TestInfo.TestState.Ignored:
                    Console.WriteLine($"Test {test.Name} ignored, because: {test.Message}.");
                    break;
            }
        }
    }

    /// <summary>
    /// Runs all methods which marked test attribute
    /// </summary>
    /// <param name="path">Directory for search assembly</param>
    public void RunTests(string path)
    {
        var files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

        var classes = files.Select(Assembly.LoadFrom).SelectMany(a => a.ExportedTypes).Where(t => t.IsClass);
        var testClasses = classes.Where(a => a.GetMethods().Any(b => Attribute.GetCustomAttributes(b).Any(c => c is TestAttribute)));
        
        Parallel.ForEach(testClasses, StartTests);
    }

    private void StartTests(Type classForTest)
    {
        var methods = SortMethods(classForTest);
        if (!RunBeforeOrAfterClassTests(methods.BeforeClass))
        {
            foreach (var methodForTest in methods.Test)
            {
                var testResult = new TestInfo(methodForTest.Name, TestInfo.TestState.Ignored, TimeSpan.Zero, "Before class method failed");
                TestResults.Add(testResult);
            }
            return;
        }

        Parallel.ForEach(methods.Test, methodForTest => RunBeforeAndTestAndAfterTests(methods, classForTest, methodForTest));
    
        if (!RunBeforeOrAfterClassTests(methods.AfterClass))
        {
            foreach (var methodForTest in methods.Test)
            {
                var testResult = new TestInfo(methodForTest.Name, TestInfo.TestState.Failed, TimeSpan.Zero, "After class method failed");
                TestResults.Add(testResult);
            }
        }
    }

    private bool RunBeforeOrAfterClassTests(List<MethodInfo> methods)
    {
        foreach (var method in methods)
        {
            try
            {
                method.Invoke(null, null);
            }
            catch (Exception)
            {
                return false;
            }
        }
        return true;
    }

    private void RunBeforeAndTestAndAfterTests(MethodsList methods, Type classForTest, MethodInfo methodForTest)
    {
        var instance = Activator.CreateInstance(classForTest);
        if (!RunBeforeOrAfterTests(methods.Before, instance))
        {
            var testResult = new TestInfo(methodForTest.Name, TestInfo.TestState.Ignored, TimeSpan.Zero, "Before test method failed");
            TestResults.Add(testResult);
            return;
        }

        var result = RunTest(methodForTest, instance);
        if (result.Result == TestInfo.TestState.Ignored || result.Result == TestInfo.TestState.Failed)
        {
            TestResults.Add(result);
            return;
        }

        if (!RunBeforeOrAfterTests(methods.After, instance))
        {
            var testResult = new TestInfo(methodForTest.Name, TestInfo.TestState.Failed, result.Time, "After test method failed");
            return;
        }
        TestResults.Add(result);
    }

    private bool RunBeforeOrAfterTests(List<MethodInfo> methods, object? instance)
    {
        foreach (var method in methods)
        {
            try
            {
                method.Invoke(instance, null);
            }
            catch (Exception)
            {
                return false;
            }
        }
        return true;
    }

    private TestInfo RunTest(MethodInfo methodForTest, Object? Instance)
    {
        var attribute = (TestAttribute) methodForTest.GetCustomAttribute(typeof(TestAttribute))!;
        if (attribute.Ignore is not null)
        {
            return new TestInfo(methodForTest.Name, TestInfo.TestState.Ignored, TimeSpan.Zero, attribute.Ignore);
        }

        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            methodForTest.Invoke(Instance, null);
            stopwatch.Stop();
            if (attribute.Expected is not null)
            {
                return new TestInfo(methodForTest.Name, TestInfo.TestState.Failed, stopwatch.Elapsed, "Expected exception was not thrown");
            }
        }
        catch (Exception exc)
        {
            stopwatch.Stop();
            if (attribute.Expected is null)
            {
                return new TestInfo(methodForTest.Name, TestInfo.TestState.Failed, stopwatch.Elapsed, $"{exc!.InnerException?.GetType()} was thrown");
            }
            else if (exc!.InnerException?.GetType() != attribute.Expected)
            {
                return new TestInfo(methodForTest.Name, TestInfo.TestState.Failed, stopwatch.Elapsed, $"Expected {attribute.Expected}, but {exc!.InnerException?.GetType()} was thrown");
            }
        }

        return new TestInfo(methodForTest.Name, TestInfo.TestState.Passed, stopwatch.Elapsed);
    }

    private MethodsList SortMethods(Type classForTest)
    {
        var methodList = new MethodsList();
        foreach (var method in classForTest.GetMethods())
        {
            foreach (var attribute in Attribute.GetCustomAttributes(method))
            {
                switch (attribute)
                {
                    case BeforeAttribute:
                        methodList.Before.Add(method);
                        break;
                    case BeforeClassAttribute:
                        if (!method.IsStatic)
                        {
                            throw new InvalidOperationException("Method with before class attribute should be static");
                        }
                        methodList.BeforeClass.Add(method);
                        break;
                    case AfterAttribute:
                        methodList.After.Add(method);
                        break;
                    case AfterClassAttribute:
                        if (!method.IsStatic)
                        {
                            throw new InvalidOperationException("Method with after class attribute should be static");
                        }
                        methodList.AfterClass.Add(method);
                        break;
                    case TestAttribute:
                        methodList.Test.Add(method);
                        break;
                }
            }
        }
        return methodList;
    }

    /// <summary>
    /// Structure for storing sorted methods
    /// </summary>
    private class MethodsList
    {
        public MethodsList()
        {
            Before = new();
            BeforeClass = new();
            After = new();
            AfterClass = new();
            Test = new();
        }
        
        /// <summary>
        /// List of methods that should run before each test
        /// </summary>
        public List<MethodInfo> Before { get; set; }

        /// <summary>
        /// List of methods that should run once before testing
        /// </summary>
        public List<MethodInfo> BeforeClass { get; set; }

        /// <summary>
        /// List of methods that should run after each test
        /// </summary>
        public List<MethodInfo> After { get; set; }

        /// <summary>
        /// List of methods that should run once after testing
        /// </summary>
        public List<MethodInfo> AfterClass { get; set; }

        /// <summary>
        /// List of methods that should run to test
        /// </summary>
        public List<MethodInfo> Test { get; set; }
    }
}
