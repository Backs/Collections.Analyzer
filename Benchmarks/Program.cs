using BenchmarkDotNet.Running;
using Benchmarks;

internal class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<InitListBenchmark>();
    }
}