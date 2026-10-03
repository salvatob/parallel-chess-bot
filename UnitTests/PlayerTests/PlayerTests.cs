using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Players;
using UnitTests.MockPlayers;
using Xunit.Abstractions;

namespace UnitTests.PlayerTests;

// ReSharper disable once InconsistentNaming
public class PlayerTests(ITestOutputHelper _out) {
    public static IEnumerable<object?[]> Players() {
        yield return [new BlockingPlayer(), null];
        yield return [new EnginePlayer(), null];
        
        var blockingReader = new BlockingTextReader(); // provide the 
        yield return [new ConsolePlayer(blockingReader, new StringWriter()), blockingReader.Waiting];
    }

    [Theory]
    [MemberData(nameof(Players))]
    public async Task ThrowsWhenCancelled_DuringMoveRequest(IPlayer player, Task? readerWaiter) {
        // arrange
        var cts = new CancellationTokenSource();
        var state = State.Initial;
        
        var timers = new Timers {
            BaseWhiteTime = TimeSpan.FromHours(1), 
            BaseBlackTime = TimeSpan.FromHours(1), 
            Increment = TimeSpan.Zero 
        };
        // act
        var moveTask = player.ChooseMoveAsync(state, timers, cts.Token);

        // Ensure the player is actually "at work" before we cancel
        if (readerWaiter is not null) {
            await readerWaiter;
        } else {
            // Fallback for players without a ready signal (like EnginePlayer)
            await Task.Delay(100); 
        }

        cts.Cancel();

        // assert
        try {
            // WaitAsync here ensures we don't hang forever if there is a deadlock bug
            await Assert.ThrowsAsync<OperationCanceledException>(async () => 
                await moveTask.WaitAsync(TimeSpan.FromSeconds(1))
            );
        } catch (TimeoutException) {
            _out.WriteLine($"[DEBUG_LOG] {player.GetType().Name} deadlocked.");
            Assert.Fail("The move request deadlocked and did not respond to cancellation within the timeout.");
        }
        

    }
    
}
