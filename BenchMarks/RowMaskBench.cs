using BenchmarkDotNet.Attributes;
using ChessBotCore;

namespace Benchmarks;

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