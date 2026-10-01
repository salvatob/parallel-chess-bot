using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Players;
using ChessBotCore.Search;

namespace UnitTests.MockPlayers;

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