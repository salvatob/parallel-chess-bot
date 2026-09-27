using System.Diagnostics;

namespace ChessBotCore.Game;

/// <summary>
/// Represents the possible states of a chess game.
/// </summary>
public enum GameOutcome {
    NonTerminal,
    WhiteWin,
    BlackWin,
    Draw
}

/// <summary>
/// Specifies the reason why a chess game ended.
/// </summary>
public enum GameEndReason {
    ClockTimedOut,
    InsufficientMaterial,
    Stalemate,
    FiftyMoveRule,
    ThreeFoldRepetition,
    Checkmate,
    Resignation,
    DrawAgreed,
    Unknown
}

/// <summary>
/// Represents the result of a completed chess game.
/// </summary>
/// <param name="Outcome">The final outcome of the game.</param>
/// <param name="GameEndReason">The reason why the game ended.</param>
/// <param name="Moves">The list of moves played during the game.</param>
public record GameResult(GameOutcome Outcome, GameEndReason GameEndReason, List<Move> Moves);

/// <summary>
/// Manages the execution of a chess game between two players.
/// </summary>
public class ChessGame : IDisposable {
    private static readonly int HalfMoveLimit = 100;
    
    private readonly IPlayer _whitePlayer;
    private readonly IPlayer _blackPlayer;
    private readonly State _state = State.Initial;
    private readonly List<Move> _moveList = new();

    private readonly Dictionary<State, int> _stateRepetitionCounter = new(new ThreeFoldRepetitionStateComparer());
    private bool _started = false;
    
    private readonly Timers _timers = new() {
        BaseWhiteTime = TimeSpan.FromMinutes(5),
        BaseBlackTime = TimeSpan.FromMinutes(5),
        Increment = TimeSpan.FromSeconds(2)
    };
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ChessGame"/> class with default timers and initial board state.
    /// </summary>
    /// <param name="whitePlayer">The player controlling the white pieces.</param>
    /// <param name="blackPlayer">The player controlling the black pieces.</param>
    public ChessGame(IPlayer whitePlayer, IPlayer blackPlayer) {
        _whitePlayer = whitePlayer;
        _blackPlayer = blackPlayer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChessGame"/> class with specific timers and board state.
    /// </summary>
    /// <param name="whitePlayer">The player controlling the white pieces.</param>
    /// <param name="blackPlayer">The player controlling the black pieces.</param>
    /// <param name="timers">The time settings for the game.</param>
    /// <param name="state">The starting board state.</param>
    public ChessGame(IPlayer whitePlayer, IPlayer blackPlayer, Timers timers, State state) 
        : this(whitePlayer, blackPlayer) {
        _state = state;
        _timers = timers;
    }

    /// <summary>
    /// Disposes of the players and releases their resources.
    /// </summary>
    public void Dispose() {
        _blackPlayer.Dispose();
        _whitePlayer.Dispose();
    }

    /// <summary>
    /// Gets the player whose turn it currently is.
    /// </summary>
    /// <returns>The active player.</returns>
    private IPlayer ActivePlayer() {
        return _state.WhiteIsActive ? _whitePlayer : _blackPlayer;
    }
    
    /// <summary>
    /// Gets the player whose turn it is NOT currently.
    /// </summary>
    /// <returns>The inactive player.</returns>
    private IPlayer InactivePlayer() {
        return _state.WhiteIsActive ? _blackPlayer : _whitePlayer;
    }

    /// <summary>
    /// Notifies both players that the game is about to start.
    /// </summary>
    /// <returns>A task representing the notification process.</returns>
    private async Task PushGameStartAsync() {
        var wpSetup = _whitePlayer.PrepareAsync(true, _state, _timers, _moveList);
        var bpSetup = _blackPlayer.PrepareAsync(false, _state, _timers, _moveList);
        await Task.WhenAll(wpSetup, bpSetup);

        var wps = _whitePlayer.OnGameStartAsync();
        var bps = _blackPlayer.OnGameStartAsync();
        await Task.WhenAll(wps, bps);
    }

    /// <summary>
    /// Notifies both players that the game has ended.
    /// </summary>
    /// <param name="gameResult">The final result of the game.</param>
    /// <returns>A task representing the notification process.</returns>
    private async Task PushGameEndAsync(GameResult gameResult) {
        var wpe = _whitePlayer.OnGameEndAsync(true, gameResult);
        var bpe = _blackPlayer.OnGameEndAsync(false, gameResult);
        await Task.WhenAll(wpe, bpe);
    }

    /// <summary>
    /// Ensures that a game instance is played only once by checking and updating the started flag.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the game has already been started.</exception>
    private void ThrowIfGameStarted() {
        // if started is true, then the game has already been ran and should throw
        if (Interlocked.CompareExchange(ref _started, true, false) == true) {
            throw new InvalidOperationException(
                "A Game instance can only be played once.");
        }

    }

    /// <summary>
    /// Checks if any position has occurred three times, which warrants a draw.
    /// </summary>
    /// <returns>True if a threefold repetition has occurred, false otherwise.</returns>
    private bool CheckThreeFoldRepetition() {
        return _stateRepetitionCounter.Values.Any(x => x >= 3);
    }

    /// <summary>
    /// Increments the repetition counter for the given state.
    /// </summary>
    /// <param name="state">The state to count.</param>
    private void IncrementRepetitionCounter(State state) {
        if (!_stateRepetitionCounter.TryAdd(state.Clone(), 1)) {
            _stateRepetitionCounter[state]++;
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
        IncrementRepetitionCounter( _state);
        await PushGameStartAsync();

        while (true) {
            if (CheckThreeFoldRepetition()) {
                var finalResult = new GameResult(GameOutcome.Draw, GameEndReason.ThreeFoldRepetition, _moveList);
                await PushGameEndAsync(finalResult);
                return finalResult;                
            }
            
            var (currentOutcome, reason) = GetDetailedOutcome(_state);
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
            IncrementRepetitionCounter(_state);
        }
    }


    /// <summary>
    ///     Determines the current outcome and reason of the game.
    /// If the game is unresolved, returns (GameOutcome.NonTerminal, GameEndReason.Unknown).
    /// </summary>
    /// <returns>A tuple of GameOutcome and GameEndReason.</returns>
    public static (GameOutcome Outcome, GameEndReason Reason) GetDetailedOutcome(State state) {
        if (state.HalfMovesSincePawnMoveOrCapture >= HalfMoveLimit) return (GameOutcome.Draw, GameEndReason.FiftyMoveRule);
        
        if (state.GetAllPieces().PopCount() <= 2) return (GameOutcome.Draw, GameEndReason.InsufficientMaterial); // insufficient material (KK)
        // cannot mate with only a knight (KNK)
        if (state.GetAllPieces().PopCount() == 3 && !(state.WhiteKnights | state.BlackKnights).IsEmpty())
            return (GameOutcome.Draw, GameEndReason.InsufficientMaterial);
        // TODO: Other insufficient material cases (KBK, etc.)

        var generator = new MoveGenerator();
        var legalMoves = generator.GenerateMoves(state).GetLegalMoves();

        if (legalMoves.Count == 0) {
            var activeKing = state.WhiteIsActive ? state.WhiteKing : state.BlackKing;
            if (MoveGenerator.IsSquareAttacked(activeKing.TrailingZeroCount(), !state.WhiteIsActive, state)) {
                return (state.WhiteIsActive ? GameOutcome.BlackWin : GameOutcome.WhiteWin, GameEndReason.Checkmate);
            }

            return (GameOutcome.Draw, GameEndReason.Stalemate);
        }

        return (GameOutcome.NonTerminal, GameEndReason.Unknown);
    }
    
    /// <summary>
    /// Logs the current state of the game to the console if verbosity is enabled.
    /// </summary>
    /// <param name="verbosity">The current verbosity level.</param>
    /// <param name="currentPlayer">The player whose turn it is.</param>
    /// <param name="time">The time remaining for the active player.</param>
    private void LogGameProgress(int verbosity, IPlayer currentPlayer, TimeSpan time) {
        if (verbosity == 0) return;
        Console.WriteLine($"Player {currentPlayer.GetType().Name} turn.");
        Console.WriteLine($"They have {time} time.");
        Console.WriteLine(_state.PrettyPrint());
    }

    /// <summary>
    /// Logs the move played by a player to the console if verbosity is enabled.
    /// </summary>
    /// <param name="verbosity">The current verbosity level.</param>
    /// <param name="player">The player who made the move.</param>
    /// <param name="move">The move that was played.</param>
    private void LogMovePlayed(int verbosity, IPlayer player, Move move) {
        if (verbosity == 0) return;
        Console.WriteLine($"Player {player.GetType().Name} made move {move.PrintLAN()}");
        Console.WriteLine();
        Console.WriteLine();
    }
}
