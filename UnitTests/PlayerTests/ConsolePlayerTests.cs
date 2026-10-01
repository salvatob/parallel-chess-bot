using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Players;

namespace UnitTests.PlayerTests;

public class ConsolePlayerTests {
    
    
    [Theory]
    public async Task ThrowsWhenCancelled_DuringMoveRequest(IPlayer player) {
        // arrange
        using var cts = new CancellationTokenSource();
        var state = State.Initial;
        var timers = new Timers();
        // act
        var move = player.ChooseMoveAsync(state, timers, cts.Token);
        
        await Task.Delay(500);
        
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await move);
        
        // assert
        player.Dispose();
    }
}