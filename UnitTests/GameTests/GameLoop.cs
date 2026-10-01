using ChessBotCore.Game;
using UnitTests.MockPlayers;

namespace UnitTests.GameTests;

public class GameLoop {
    [Fact]
    public async Task Cancels() {
        // arrange
        using var cts = new CancellationTokenSource();
        
        var wp = new BlockingPlayer();
        var bp = new SequencePlayer([]);
        
        using var game = new ChessGame(wp, bp);

        // act
        var gameResult = game.PlayAsync(0, cts.Token);
        
        await wp.Waiting;
        await cts.CancelAsync();

        // assert
        await Assert.ThrowsAsync<OperationCanceledException>( () => gameResult);
        
        Assert.True(gameResult.IsCanceled, "Game should be canceled");
        
    }
}
