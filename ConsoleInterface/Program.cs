

using System.Diagnostics;
using ChessBotCore;


internal class Program {

    private static State BlackStartPos = State.Initial with { WhiteIsActive = false };
    
    public static void Main(string[] args) {
     
  
        var chessSingle = new DefaultChessWrapper();
        var chessMulti = new ParallelChessWrapper();
        // TryPerft(3, chessSingle);
        // TryPerft(3, chessMulti);
        // TryPerft(7, chess);

        DividePerft(State.Initial, 3, chessSingle);

        // Console.WriteLine("normal");
        // TryPerft(6, new DefaultChessWrapper().EvalPerft);
        // Console.WriteLine("parallel");
        // TryPerft(6, new ParallelChessWrapper().EvalPerft);

    }

    static void DividePerft(State s, int depth, IChessWrapper chess) {
        var g = GeneratorWrapper.Default;
        // long count = 0;

        var moveCounts = new Dictionary<string, long>();
        moveCounts["unmarked"] = 0;
        
        foreach (var m in g.GetLegalMoves(s)) {
            long nodes = chess.Perft(m.StateAfter, depth - 1);
            var key = m.TryGetNotation(s);
            if (key is null) moveCounts["unmarked"] += nodes;
            else moveCounts[key] = nodes;
        }

        
        foreach (var m in moveCounts.Keys.Order()) {
            Console.WriteLine($"{m}: {moveCounts[m]}");
        }

        Console.WriteLine($"Nodes searched: {moveCounts.Values.Sum()}");
    }
    
    static void TryPerft(int depth, IChessWrapper chess) {
        // void TryPerft(int depth, Func<State, int, long> perft) {
 
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var sw = Stopwatch.StartNew();

        // var memoryBefore = GC.GetAllocatedBytesForCurrentThread();
        // var leavesExplored = chess.EvalPerft(State.Initial, depth);
        var leavesExplored = chess.Perft(State.Initial, depth);

        // var memoryAfter = GC.GetAllocatedBytesForCurrentThread();
        sw.Stop();
        // var mem = memoryAfter - memoryBefore;


        Console.WriteLine($"Pertf has explored {leavesExplored:N0} leaves in depth {depth} in {sw.Elapsed.ToString()}.");
        Console.WriteLine($"Comes up to around {(leavesExplored * 1000) / sw.ElapsedMilliseconds:N0} leaves per second");
        // Console.WriteLine($"{mem/1000_000} MB of memory has been allocated during");
    }
}

