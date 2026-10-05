using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Search;
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

    [Fact]
    public async Task ThrowsWhenPlayerThrowsDuringMove() {
        // arrange
        var wp = new ThrowingPlayer();
        var bp = new MockPlayer();
        var game = new ChessGame(wp, bp);

        // act & assert
        try {
            await Assert.ThrowsAsync<MoveException>(async () => await game.PlayAsync());
        }
        finally {
            game.Dispose();
        }

        Assert.True(wp.IsDisposed, "White player should be disposed after exception");
        Assert.True(bp.IsDisposed, "Black player should be disposed after exception");
    }

    [Fact]
    public async Task TerminatesOnClockTimeout() {
        // arrange
        var wp = new BlockingPlayer();
        var bp = new MockPlayer();
        var timers = new Timers {
            BaseWhiteTime = TimeSpan.FromMilliseconds(100),
            BaseBlackTime = TimeSpan.FromMinutes(1),
            Increment = TimeSpan.Zero
        };
        var game = new ChessGame(wp, bp, timers, State.Initial);

        // act
        var gameResult = await game.PlayAsync();

        // assert
        Assert.Equal(GameEndReason.ClockTimedOut, gameResult.GameEndReason);
        Assert.Equal(GameOutcome.BlackWin, gameResult.Outcome); // White timed out

        game.Dispose();
        Assert.True(wp.IsDisposed, "White player should be disposed");
        Assert.True(bp.IsDisposed, "Black player should be disposed");
    }

    [Fact]
    public async Task ThrowsWhenPlayerThrowsDuringOpponentsMove() {
        // arrange
        var wp = new MockPlayer();
        var bp = new ThrowingPlayerOnOpponentMove();
        var game = new ChessGame(wp, bp);

        // act & assert
        try {
            await Assert.ThrowsAsync<MoveException>(async () => await game.PlayAsync());
        }
        finally {
            game.Dispose();
        }

        Assert.True(wp.IsDisposed, "White player should be disposed after exception");
        Assert.True(bp.IsDisposed, "Black player should be disposed after exception");
    }

    private class ThrowingPlayer : MockPlayer {
        public override Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct = default) {
            throw new MoveException("Simulated player failure during move choice");
        }
    }

    private class ThrowingPlayerOnOpponentMove : MockPlayer {
        public override Task OnOpponentsMoveAsync(Move move, State newState) {
            throw new MoveException("Simulated player failure during opponent move notification");
        }
    }
}
