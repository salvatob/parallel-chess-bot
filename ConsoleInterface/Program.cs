using System.Diagnostics;
using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.MoveGenerators;
using ChessBotCore.Performance;
using ChessBotCore.Players;
using ChessBotCore.Search;

namespace ConsoleInterface;

internal class Program {
    public static async Task Main(string[] args) {
        // await 
        // await OneMove();
        // await PlayGame();

        var n = new MinimaxEvaluator();
        var m = n.ChooseBestMove(State.Initial, 6);
        Console.WriteLine(m.BestMove.PrintLAN());
        // var r = Performance.ParallelPerft(State.Initial, 6);
        // Console.WriteLine(r);
    }

    public static async Task<GameResult> PlayGame() {
        var whitePlayer = new ConsolePlayer();
        // var blackPlayer = new EnginePlayer();
        // var whitePlayer = new RandomPlayer();
        var blackPlayer = new RandomPlayer();
        var game = new ChessGame(whitePlayer, blackPlayer);
        return await game.PlayAsync(1);
    }

    public static async Task OneMove() {
        var state = State.Initial;
        var ai = new EnginePlayer();

        Timers timers = new();
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        var nextMove = ai.ChooseMoveAsync(state, timers, ct);

        ct.Register(() =>
            Console.WriteLine("Cancellation requested!"));

        var command = Task.Run(Console.ReadLine);

        var nextMoveRequested = await Task.WhenAny(nextMove, command);


        if (nextMoveRequested == command)
            if (command.Result != null && command.Result.StartsWith("ok"))
                await cts.CancelAsync();

        var aiMoveResult = await nextMove;

        Console.WriteLine(aiMoveResult.BestMove);
    }
    private static void CompareMoveGeneration() {
        // DividePerft(State.Initial, 3, chessSingle);

        // Console.WriteLine("normal");
        // TryPerft(6, new DefaultChessWrapper().EvalPerft);
        // Console.WriteLine("parallel");
        // TryPerft(6, new ParallelChessWrapper().EvalPerft);
    }

    private static void DividePerft(State s, int depth) {
        var moveCounts = Performance.DividePerft(s, depth);
        
        foreach (var m in moveCounts.Keys.Order())
            Console.WriteLine($"{m.PrintLAN()}: {moveCounts[m]}");

        Console.WriteLine($"Nodes searched: {moveCounts.Values.Sum()}");
    }

    private static void PerftStats(int depth, State? state = null) {
        state ??= State.Initial;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var sw = Stopwatch.StartNew();

        var memoryBefore = GC.GetAllocatedBytesForCurrentThread();
        var leavesExplored = Performance.Perft(state, depth);

        var memoryAfter = GC.GetAllocatedBytesForCurrentThread();
        sw.Stop();
        var mem = memoryAfter - memoryBefore;


        Console.WriteLine($"Pertf has explored {leavesExplored:N0} leaves in depth {depth} in {sw.Elapsed.ToString()}.");
        Console.WriteLine($"Comes up to around {(leavesExplored * 1000) / sw.ElapsedMilliseconds:N0} leaves per second");
        Console.WriteLine($"{mem/1000_000} MB of memory has been allocated during");
    }
}
