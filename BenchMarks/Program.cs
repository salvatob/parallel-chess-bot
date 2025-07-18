using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace Benchmarks;


public class Program
{
    public static void Main(string[] args) {
        var inProcessConfig = ManualConfig.Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance));
        // BenchmarkRunner.Run<RowMaskBench>(inProcessConfig);
        BenchmarkRunner.Run<BitOperationMethods>(inProcessConfig);
    }
}