using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using ChessBotCore;

namespace Benchmarks;


public class Program
{
    public static void Main(string[] args) {
        var inProcessConfig = ManualConfig.Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance));
        BenchmarkRunner.Run<RowMaskBench>(inProcessConfig);
        // BenchmarkRunner.Run<RowMaskBench>();
    }
}

[MemoryDiagnoser]
public class RowMaskBench {
    


    // Run once per index 0–7
    [Params(0,1,2,3,4,5,6,7)]
    public int RowIndex;
    
    [Benchmark]
    public ulong ArrayLoad()
        => BitMask.Row[RowIndex];
    
    [Benchmark]
    public ulong SwitchLoad()
        => RowMasksCompletelyStatic.GetMask(RowIndex);
}