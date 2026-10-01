using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Players;
using ChessBotCore.Search;


namespace UnitTests;


public class MockPlayer : EmptyPlayer {
    public bool Disposed { get; private set; }
    public int OnGameEndCalledCount { get; private set; }
    public TaskCompletionSource PlayTaskSource { get; } = new();

    public override Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct = default) {
        // Return a dummy search result if needed, but for ChessManager tests
        // we might just want to control the game loop.
        
        var generator = new MoveGenerator();
        var move = generator.GenerateMoves(state).GetLegalMoves().First();

        return Task.FromResult(new SearchResults { BestMove = move });
    }
    
    public override Task OnGameEndAsync(bool yourColor, GameResult result) {
        OnGameEndCalledCount++;
        return Task.CompletedTask;
    }
    
    public override void Dispose() {
        Disposed = true;
    }
}


public class BlockingPlayer : MockPlayer {
    private TaskCompletionSource<SearchResults> _tcs = new();

    public override Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct = default) {
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
    public override Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct = default) {
        throw new InvalidOperationException("Simulated crash");
    }
}