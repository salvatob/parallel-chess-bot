using ChessBotCore.Game;
using ChessBotCore.MoveGenerators;
using ChessBotCore.Search;

namespace ChessBotCore.Players;

/// <summary>
/// A chess player implementation that selects moves randomly from the list of legal moves.
/// </summary>
public class RandomPlayer : IPlayer {
    // ReSharper disable once InconsistentNaming
    private static readonly Random _random = new();
    /// <inheritdoc/>
    public void Dispose() { }

    /// <inheritdoc/>
    public SearchHandle ChooseMoveAsync(State state, Timers timers) {
        var generator = new MoveGenerator();
        var moves = generator.GenerateMoves(state).GetLegalMoves();

        Move move = moves[_random.Next(moves.Count)];

        var result = new SearchResults {
            BestMove = move
        };
        
        return new SearchHandle(new CancellationTokenSource(), Task.FromResult(result));
    }

    /// <inheritdoc/>
    public Task PrepareAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> moveHistory) {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnGameStartAsync() {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnGameEndAsync(bool yourColor, GameResult result) {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnOpponentsMoveAsync(Move move, State newState) {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnErrorNotifyAsync(Exception error, bool gameEnd) {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnErrorNotifyAsync(string errorMessage, bool gameEnd) {
        return Task.CompletedTask;
    }
}
