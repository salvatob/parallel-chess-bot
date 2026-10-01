using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Search;


namespace UnitTests;


public class MockPlayer : IPlayer {
    public bool Disposed { get; private set; }
    public int OnGameEndCalledCount { get; private set; }
    public TaskCompletionSource PlayTaskSource { get; } = new();

    public virtual Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct = default) {
        // Return a dummy search result if needed, but for ChessManager tests
        // we might just want to control the game loop.
        
        var generator = new MoveGenerator();
        var move = generator.GenerateMoves(state).GetLegalMoves().First();

        return Task.FromResult(new SearchResults { BestMove = move });
    }

    public Task PrepareAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> moveHistory) => Task.CompletedTask;

    public Task OnGameStartAsync() => Task.CompletedTask;

    public Task OnGameEndAsync(bool yourColor, GameResult result) {
        OnGameEndCalledCount++;
        return Task.CompletedTask;
    }

    public Task OnOpponentsMoveAsync(Move move, State newState) => Task.CompletedTask;

    public Task OnErrorNotifyAsync(Exception error, bool gameEnd) => Task.CompletedTask;

    public Task OnErrorNotifyAsync(string errorMessage, bool gameEnd) => Task.CompletedTask;

    public virtual void Dispose() {
        Disposed = true;
    }
}


public class BlockingPlayer : MockPlayer {
    private TaskCompletionSource<SearchResults> _tcs = new();

    public override Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct) {
        return _tcs.Task;
    }

    public void Release() {
        var oldTcs = _tcs;
        _tcs = new TaskCompletionSource<ChessBotCore.Search.SearchResults>();
        var move = new Move(0, 0); // Still default move, but now we ensure Game 1 ends eventually
        oldTcs.SetResult(new ChessBotCore.Search.SearchResults { BestMove = move });
    }

    public void EndGame() {
        _tcs.TrySetException(new OperationCanceledException());
    }
}

public class ThrowingPlayer : MockPlayer {
    public override Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct) {
        throw new Exception("Simulated crash");
    }
}