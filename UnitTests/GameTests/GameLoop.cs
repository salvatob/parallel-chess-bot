using ChessBotCore.Game;
using UnitTests.MockPlayers;

namespace UnitTests.GameTests;

public class GameLoop {
    [Fact]
    public async Task CancelsOnMove() {
        // arrange
        using var cts = new CancellationTokenSource();
        
        var wp = new BlockingPlayer();
        var bp = new SequencePlayer([]);
        
        var game = new ChessGame(wp, bp);

        // act
        var gameResult = game.PlayAsync(0, cts.Token);
        
        await wp.Waiting;
        await cts.CancelAsync();

        // assert
        try {
            await Assert.ThrowsAsync<OperationCanceledException>( async () => await gameResult.WaitAsync(TimeSpan.FromSeconds(1)));
        } catch (TimeoutException) {
            Assert.Fail("The game request deadlocked and did not respond to cancellation within the timeout.");
        } finally {
            game.Dispose();
        }
        
        Assert.True(gameResult.IsCanceled, "Game should be canceled");
        
        
        Assert.True(wp.IsDisposed, "White player should be disposed");
        Assert.True(bp.IsDisposed, "Black player should be disposed");
    }
}
