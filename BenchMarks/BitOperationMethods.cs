using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public class BitOperationMethods {

    private List<ulong> _randomData = null!;

    [GlobalSetup]
    public void PopulateData() {
        int size = 1000;
        _randomData = new List<ulong>(size);
        var rng = new Random(42);
        for (int i = 0; i < size; i++) {
            _randomData.Add((ulong)rng.Next());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReturnNewUlong(ulong bits) {
        return bits << 9;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]    
    public static void ChangeExistingUlong(ref ulong bits) {
        bits <<= 9;
    }

    [Benchmark]
    public ulong UseRefParameter() {
        ulong t = 0;
        for (var i = 0; i < _randomData.Count; i++) {
            var ul = _randomData[i];
            ChangeExistingUlong(ref ul);
            t += ul;
        }

        return t;
    }

    [Benchmark]
    public ulong UseNormalParameter() {
        ulong t = 0;

        for (var i = 0; i < _randomData.Count; i++) {
            var ul = _randomData[i];
            t += ReturnNewUlong(ul);
        }

        return t;
    }
}
