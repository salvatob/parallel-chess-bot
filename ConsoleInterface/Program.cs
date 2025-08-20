

using System.Diagnostics;
using ChessBotCore;



void TryPerft(int depth, IChessWrapper chess) {
// void TryPerft(int depth, Func<State, int, long> perft) {
 
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();

    var sw = Stopwatch.StartNew();

    // var memoryBefore = GC.GetAllocatedBytesForCurrentThread();
    var leavesExplored = chess.EvalPerft(State.Initial, depth);
    // var leavesExplored =  perft(State.Initial, depth);

    // var memoryAfter = GC.GetAllocatedBytesForCurrentThread();
    sw.Stop();
    // var mem = memoryAfter - memoryBefore;


    Console.WriteLine($"Pertf has explored {leavesExplored:N0} leaves in depth {depth} in {sw.Elapsed.ToString()}.");
    Console.WriteLine($"Comes up to around {(leavesExplored * 1000) / sw.ElapsedMilliseconds:N0} leaves per second");
    // Console.WriteLine($"{mem/1000_000} MB of memory has been allocated during");
}

var chess = new ParallelChessWrapper();
TryPerft(5, chess);
TryPerft(6, chess);


return;

// Console.WriteLine("normal");
// TryPerft(6, new DefaultChessWrapper().EvalPerft);
// Console.WriteLine("parallel");
// TryPerft(6, new ParallelChessWrapper().EvalPerft);

