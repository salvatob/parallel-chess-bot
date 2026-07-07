

using System.Diagnostics;
using ChessBotCore;


internal class Program {
    public static void Main(string[] args) {
        var chessSingle = new DefaultChessWrapper();
        var chessMulti = new ParallelChessWrapper();
        // State state = State.FromFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1"); 
        State state = State.FromFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1 "); 
        // State state = State.FromFen("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1"); 
        
        int depth = 6; // I want 119,060,324 nodes without castles
        Console.WriteLine("single threaded");
        PerftStats(depth, chessSingle);
        
        // Console.WriteLine("multi threaded");
        // PerftStats(depth, chessMulti, state);

        
        // DividePerft(State.Initial, 3, chessSingle);

        // Console.WriteLine("normal");
        // TryPerft(6, new DefaultChessWrapper().EvalPerft);
        // Console.WriteLine("parallel");
        // TryPerft(6, new ParallelChessWrapper().EvalPerft);

    }

    static void DividePerft(State s, int depth, IChessWrapper chess) {
        var moves = new GeneratorWrapper(s).GetLegalMoves().ToList();
        
        var moveCounts = new Dictionary<string, long>();
        moveCounts["unmarked"] = 0;
        
        foreach (var m in moves) {
            var nextState = s.Clone();
            nextState.ApplyMove(m);
            long nodes = chess.Perft(nextState, depth - 1);
            var key = Move.TryGetNotation(s, nextState);
            if (key is null) moveCounts["unmarked"] += nodes;
            else moveCounts[key] = nodes;
        }

        
        foreach (var m in moveCounts.Keys.Order()) {
            Console.WriteLine($"{m}: {moveCounts[m]}");
        }

        Console.WriteLine($"Nodes searched: {moveCounts.Values.Sum()}");
    }
    
    static void PerftStats(int depth, IChessWrapper chess, State? state=null) {
        state ??= State.Initial;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var sw = Stopwatch.StartNew();

        var memoryBefore = GC.GetAllocatedBytesForCurrentThread();
        var leavesExplored = chess.Perft(state, depth);

        var memoryAfter = GC.GetAllocatedBytesForCurrentThread();
        sw.Stop();
        var mem = memoryAfter - memoryBefore;


        Console.WriteLine($"Pertf has explored {leavesExplored:N0} leaves in depth {depth} in {sw.Elapsed.ToString()}.");
        Console.WriteLine($"Comes up to around {(leavesExplored * 1000) / sw.ElapsedMilliseconds:N0} leaves per second");
        Console.WriteLine($"{mem/1000_000} MB of memory has been allocated during");
    }
}

