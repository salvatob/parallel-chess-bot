using BenchmarkDotNet.Running;

namespace Benchmarks;

public class BenchmarkProgram
{
    public static void Main(string[] args) {
        BenchmarkSwitcher
            .FromAssembly(typeof(BenchmarkProgram).Assembly)
            .Run(args, BenchmarkSettings.Config);
    }
}
