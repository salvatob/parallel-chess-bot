using ChessBotCore.Game;
using ChessBotCore.Search;

namespace ChessBotCore.Players;

/// <summary>
/// A chess player implementation that uses a minimax engine to select moves.
/// </summary>
public class EnginePlayer : IPlayer {
    private readonly MinimaxEvaluator _negamaxer = new();
    
    /// <inheritdoc/>
    public Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct) {
        var moveTask = Task.Run(
            () => _negamaxer.PrimitiveIterativeSearch(state, timers, ct),
            ct
        );

        return moveTask;
    }
    
    /// <inheritdoc/>
    public Task PrepareAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> moveHistory) {
        // currently we don't store the move list
        
        // This could probably be pretty nice later, we could fire up some lookup tables,
        // or load an opening book or do similar work before the game starts
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnGameStartAsync() {
        // pondering could start here 
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
        return OnErrorNotifyAsync(error.Message, gameEnd);
    }

    /// <inheritdoc/>
    public Task OnErrorNotifyAsync(string errorMessage, bool gameEnd) {
        // I don't see anything an engine should do on error. Possibly log it if I decide to add that
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose() {
        // its totally okay that its empty for now
        // in future, this could save the results into some kind of log file so I can inspect the games.
    }
}
