using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Players;
using UnitTests.MockPlayers;
using Xunit.Abstractions;

namespace UnitTests.PlayerTests;

public class ConsolePlayerTests {
    private readonly ITestOutputHelper _out;
    public ConsolePlayerTests(ITestOutputHelper @out) {
        _out = @out;
    }
    
    public static IEnumerable<object[]> Players() {
        yield return [new BlockingPlayer()];
        yield return [new ConsolePlayer(new StringReader(""), new StringWriter())];
        yield return [new EnginePlayer()];
    }

    [Theory]
    [MemberData(nameof(Players))]
    public async Task ThrowsWhenCancelled_DuringMoveRequest(IPlayer player) {
        // act
        var moveTask = Task.Run(async () => {
            // arrange
            using var cts = new CancellationTokenSource();
            var state = State.Initial;
            
            var timers = new Timers {
                BaseWhiteTime = TimeSpan.FromHours(1), 
                BaseBlackTime = TimeSpan.FromHours(1), 
                Increment = TimeSpan.Zero 
            };
        
            var innerTask = player.ChooseMoveAsync(state, timers, cts.Token);
        
            // Ensure the task has started
            await Task.Delay(100); 
            cts.Cancel();

            await innerTask;
        });

        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));
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
    
        player.Dispose();
    }
}
