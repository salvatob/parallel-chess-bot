using System.Diagnostics;

namespace ChessBotCore.Game;

public enum GameOutcome {
    NonTerminal,
    WhiteWin,
    BlackWin,
    Draw
}

public record GameResult(GameOutcome Outcome, List<Move> Moves);

public class ChessGame : IDisposable {
    private readonly IPlayer _whitePlayer;
    private readonly IPlayer _blackPlayer;
    private readonly State _state = State.Initial;

    private readonly Timers _timers = new() {
        BaseWhiteTime = TimeSpan.FromMinutes(5),
        BaseBlackTime = TimeSpan.FromMinutes(5),
        Increment = TimeSpan.FromSeconds(2)
    };
    
    public ChessGame(IPlayer whitePlayer, IPlayer blackPlayer) {
        _whitePlayer = whitePlayer;
        _blackPlayer = blackPlayer;
    }

    public ChessGame(IPlayer whitePlayer, IPlayer blackPlayer, Timers timers, State state) : this(whitePlayer,
        blackPlayer) {
        _state = state;
        _timers = timers;
    }

    public void Dispose() {
        _blackPlayer.Dispose();
        _whitePlayer.Dispose();
    }

    private IPlayer ActivePlayer(bool isWhite) {
        return isWhite ? _whitePlayer : _blackPlayer;
    }

    private async Task PushGameStartAsync() {
        var wps = _whitePlayer.OnGameStartAsync(true, _state, _timers);
        var bps = _blackPlayer.OnGameStartAsync(false, _state, _timers);
        await Task.WhenAll(wps, bps);
    }

    private async Task PushGameEndAsync(GameResult gameResult) {
        var wpe = _whitePlayer.OnGameGameEndAsync(true, gameResult);
        var bpe = _blackPlayer.OnGameGameEndAsync(false, gameResult);
        await Task.WhenAll(wpe, bpe);
    }

    
    /// <summary>
    /// Runs the entire game loop until time has ran out, or a terminal state has been reached.
    /// </summary>
    /// <param name="verbosity">Verbosity level (0,1,2)</param>
    /// <returns>The outcome of the game.</returns>
    public async Task<GameResult> PlayAsync(int verbosity = 0) {
        List<Move> moveList = new();
        await PushGameStartAsync();

        while (true) {
            GameOutcome currentOutcome = _state.GetOutcome();
            if (currentOutcome != GameOutcome.NonTerminal) {
                var finalResult = new GameResult(currentOutcome, moveList);
                await PushGameEndAsync(finalResult);
                return finalResult;
            }

            var whiteIsActive = _state.WhiteIsActive;
            var player = ActivePlayer(whiteIsActive);
            ref var playerTime = ref _timers.ActiveTime(whiteIsActive);
            if (verbosity > 0) {
                Console.WriteLine($"Player {player.GetType().Name} turn");
                Console.WriteLine(_state.PrettyPrint());
            }

            using var timeOutCts = new CancellationTokenSource();
            var timeOutTask = Task.Delay(playerTime, timeOutCts.Token);

            var sw = Stopwatch.StartNew();
            using var moveHandle = player.ChooseMoveAsync(_state, _timers);


            var t = await Task.WhenAny(moveHandle.Result, timeOutTask);

            sw.Stop();
            // player has lost on time
            if (t == timeOutTask) {
                moveHandle.Cancel();
                var outcome = whiteIsActive ? GameOutcome.BlackWin : GameOutcome.WhiteWin;
                return new GameResult(outcome, moveList);
            }

            _timers.UpdateTimer(sw.Elapsed, whiteIsActive);
            timeOutCts.Cancel();

            // the result is already finished so the await is instant
            var searchResult = await moveHandle.Result;


            var move = searchResult.BestMove;

            if (verbosity > 0) {
                Console.WriteLine($"Player {player.GetType().Name} made move {move.PrintLAN()}");
                Console.WriteLine();
                Console.WriteLine();
            }

            moveList.Add(move);
            _state.ApplyMove(move);
        }
    }
}
