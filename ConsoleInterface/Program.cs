using System.Diagnostics;
using ChessBotCore;
using ChessBotCore.ChessWrappers;
using ChessBotCore.MoveGenerators;
using ChessBotCore.Search;
using ConsoleInterface;


internal class Program {
    public static async Task<int> Main(string[] args) {
        try {
            await TryUCI();
            
        } catch (Exception e) {
            Console.WriteLine(e);
        }
        return 0;
    }

    public static async Task TryUCI() {
        State state = State.Initial;
        var negamaxer = new EnginePlayer();
        while (true) {
            try {
                var nextMove = negamaxer.StartSearch(state, TimeSpan.FromSeconds(6));
                
                nextMove.Register(() =>
                    Console.WriteLine("Cancellation requested!"));

                var command = Task.Run(Console.ReadLine);
                
                var nextMoveRequested = await Task.WhenAny(nextMove.Result, command);

                if (nextMoveRequested == command) {
                    if (command.Result != null && command.Result.StartsWith("ok"))
                        nextMove.Cancel();
                }
                // command.WaitAsync()
                var aiMoveResult = await nextMove.Result;
                
                
                var aiMove = aiMoveResult.BestMove;
                state.ApplyMove(aiMove);
                Console.WriteLine("---------after AI----------");
                Console.WriteLine(state.PrettyPrint());
                Console.WriteLine("-------------------------------");

                var playerMove = Move.Parse(Console.ReadLine());
                state.ApplyMoveWithoutMetadata(playerMove);
                Console.WriteLine("---------after player------");
                Console.WriteLine(state.PrettyPrint());
                Console.WriteLine("-------------------------------");
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }

        }
        
    }
    
    static void CompareMoveGeneration() {
        
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

