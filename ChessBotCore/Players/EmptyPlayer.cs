using ChessBotCore.Game;
using ChessBotCore.Search;

namespace ChessBotCore.Players;

public abstract class EmptyPlayer : IPlayer {
    public abstract Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct=default);
    
    public virtual Task PrepareAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> moveHistory) {
        return Task.CompletedTask;
    }
    
    public virtual Task OnGameStartAsync() {
        return Task.CompletedTask;
    }
    
    public virtual Task OnGameEndAsync(bool yourColor, GameResult result) {
        return Task.CompletedTask;
    }
    
    public virtual Task OnOpponentsMoveAsync(Move move, State newState) {
        return Task.CompletedTask;
    }
    
    public virtual Task OnErrorNotifyAsync(Exception error, bool gameEnd) {
        return Task.CompletedTask;
    }
    
    public virtual Task OnErrorNotifyAsync(string errorMessage, bool gameEnd) {
        return Task.CompletedTask;
    }
    
    public virtual void Dispose() { }
}