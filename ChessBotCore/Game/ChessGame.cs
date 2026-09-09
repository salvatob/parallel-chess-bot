using System.Diagnostics;

namespace ChessBotCore.Game;

public enum GameOutcome {
    NonTerminal,
    WhiteWin,
    BlackWin,
    Draw
}

public enum GameEndReason {
    ClockTimedOut,
    InsufficientMaterial,
    Stalemate,
    FiftyMoveRule,
    Checkmate,
    Resignation,
    DrawAgreed,
    Unknown
}



public record GameResult(GameOutcome Outcome, GameEndReason GameEndReason, List<Move> Moves);

public class ChessGame : IDisposable {
    private readonly IPlayer _whitePlayer;
    private readonly IPlayer _blackPlayer;
    private readonly State _state = State.Initial;
    private readonly List<Move> _moveList = new();
    private bool _started = false;
    
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

    private IPlayer ActivePlayer() {
        return _state.WhiteIsActive ? _whitePlayer : _blackPlayer;
    }
    
    private IPlayer InactivePlayer() {
        return _state.WhiteIsActive ? _blackPlayer : _whitePlayer;
    }

    private async Task PushGameStartAsync() {
        var wpSetup = _whitePlayer.PrepareAsync(true, _state, _timers, _moveList);
        var bpSetup = _blackPlayer.PrepareAsync(false, _state, _timers, _moveList);
        await Task.WhenAll(wpSetup, bpSetup);

        var wps = _whitePlayer.OnGameStartAsync();
        var bps = _blackPlayer.OnGameStartAsync();
        await Task.WhenAll(wps, bps);
    }

    private async Task PushGameEndAsync(GameResult gameResult) {
        var wpe = _whitePlayer.OnGameEndAsync(true, gameResult);
        var bpe = _blackPlayer.OnGameEndAsync(false, gameResult);
        await Task.WhenAll(wpe, bpe);
    }

    private void ThrowIfGameStarted() {
        // if started is true, then the game has already been ran and should throw
        if (Interlocked.CompareExchange(ref _started, true, false) == true) {
            throw new InvalidOperationException(
                "A Game instance can only be played once.");
        }

    }
    
    /// <summary>
    /// Runs the entire game loop until time has ran out, or a terminal state has been reached.
    /// </summary>
    /// <param name="verbosity">Verbosity level (0,1,2)</param>
    /// <returns>The outcome of the game.</returns>
    /// <exception cref="InvalidOperationException">The game has already been played.</exception>
    public async Task<GameResult> PlayAsync(int verbosity = 0) {
        ThrowIfGameStarted();
        await PushGameStartAsync();

        while (true) {
            var (currentOutcome, reason) = _state.GetDetailedOutcome();
            // TODO should handle threefold repetition here
            if (currentOutcome != GameOutcome.NonTerminal) {
                var finalResult = new GameResult(currentOutcome, reason, _moveList);
                await PushGameEndAsync(finalResult);
                return finalResult;
            }

            var whiteIsActive = _state.WhiteIsActive;
            var playerTime = _timers.ActiveTime(whiteIsActive);
            
            LogGameProgress(verbosity, ActivePlayer(), playerTime);

            using var timeOutCts = new CancellationTokenSource();
            var timeOutTask = Task.Delay(playerTime, timeOutCts.Token);

            var sw = Stopwatch.StartNew();
            using var moveHandle = ActivePlayer().ChooseMoveAsync(_state, _timers);


            var t = await Task.WhenAny(moveHandle.Result, timeOutTask);

            sw.Stop();
            // player has lost on time
            if (t == timeOutTask) {
                var outcome = whiteIsActive ? GameOutcome.BlackWin : GameOutcome.WhiteWin;
                var gameResult = new GameResult(outcome, GameEndReason.ClockTimedOut ,_moveList);
                
                if (verbosity > 0)
                    Console.WriteLine($"Player {ActivePlayer().GetType().Name} lost on time.");
                
                await PushGameEndAsync(gameResult);
                // the handle should be cancelled after the game end is propagated
                moveHandle.Cancel();
                return gameResult;
            }

            _timers.UpdateTimer(sw.Elapsed, whiteIsActive);
            timeOutCts.Cancel();

            // the result is already finished, so the await is instant
            var searchResult = await moveHandle.Result;


            var move = searchResult.BestMove;
            
            LogMovePlayed(verbosity, ActivePlayer(), move);

            _moveList.Add(move);
            _state.ApplyMove(move);
            
            await InactivePlayer().OnOpponentsMoveAsync(move, _state);
        }
    }

    private void LogGameProgress(int verbosity, IPlayer currentPlayer, TimeSpan time) {
        if (verbosity == 0) return;
        Console.WriteLine($"Player {currentPlayer.GetType().Name} turn.");
        Console.WriteLine($"They have {time} time.");
        Console.WriteLine(_state.PrettyPrint());
    }

    private void LogMovePlayed(int verbosity, IPlayer player, Move move) {
        if (verbosity == 0) return;
        Console.WriteLine($"Player {player.GetType().Name} made move {move.PrintLAN()}");
        Console.WriteLine();
        Console.WriteLine();
    }
}
