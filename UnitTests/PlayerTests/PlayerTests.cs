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
        var moveTask = Task.Run(async () => {
            var innerTask = player.ChooseMoveAsync(state, timers, cts.Token);
        
            // Ensure the task has started
            if (readerWaiter is not null) {
                await readerWaiter;
            } else {
                await Task.Delay(100); 
            }
            
            cts.Cancel();

            await innerTask;
        });

        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(1));
        var completedTask = await Task.WhenAny(moveTask, timeoutTask);
    
        // assert
        if (completedTask == timeoutTask) {
            _out.WriteLine($"[DEBUG_LOG] {player.GetType().Name} deadlocked.");
            Assert.Fail("The move request deadlocked and did not respond to cancellation within the timeout.");
            return; 
        }

        try {
            await moveTask;
            Assert.Fail("Expected OperationCanceledException but task completed successfully.");
        } catch (OperationCanceledException) {
            // Success
        } catch (Exception ex) {
            Assert.Fail($"Expected OperationCanceledException but got {ex.GetType().Name}: {ex.Message}");
        }
    
        // player.Dispose();
    }
    
}
