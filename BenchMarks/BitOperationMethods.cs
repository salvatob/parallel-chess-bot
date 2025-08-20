using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public sealed class BitOperationMethods {

    public static List<ulong> RandomData { get; set; }

    [GlobalSetup]
    public static void PopulateData() {
        int size = 1000;
        RandomData = new List<ulong>(size);
        var rng = new Random();
        for (int i = 0; i < size; i++) {
            RandomData.Add((ulong)rng.Next());
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
    public void UseRefParameter() {
        ulong t = 0;
        for (var i = 0; i < RandomData.Count; i++) {
            var ul = RandomData[i];
            ChangeExistingUlong(ref ul);
            t = ul;
        }
    }

    [Benchmark]
    public void UseNormalParameter() {
        ulong t = 0;

        for (var i = 0; i < RandomData.Count; i++) {
            var ul = RandomData[i];
            t += ReturnNewUlong(ul);
        }
    }
}