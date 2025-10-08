using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace Benchmarks;


public class BenchmarkProgram
{
    public static void Main(string[] args) {
        var inProcessConfig = ManualConfig.Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithToolchain(InProcessNoEmitToolchain.Instance));
        // BenchmarkRunner.Run<RowMaskBench>(inProcessConfig);
        // BenchmarkRunner.Run<BitOperationMethods>(inProcessConfig);
        BenchmarkRunner.Run<AllocationExperiments>(inProcessConfig);
    }
}