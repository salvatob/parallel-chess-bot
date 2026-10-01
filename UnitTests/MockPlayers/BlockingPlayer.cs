using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Search;

namespace UnitTests.MockPlayers;

public class BlockingPlayer : MockPlayer {
    private TaskCompletionSource<SearchResults> _tcs = new();

    private readonly TaskCompletionSource _waiting =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Waiting => _waiting.Task;

    
    public override async Task<SearchResults> ChooseMoveAsync(State state,
        Timers timers,
        CancellationToken ct = default)
    {
        _waiting.TrySetResult();
        try {
            // WaitAsync(ct) will throw a TaskCanceledException if 'ct' is canceled.
            // This exception is a subclass of OperationCanceledException.
            return await _tcs.Task.WaitAsync(ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            // Re-throw to ensure the stack trace and token info matches a standard cancellation
            ct.ThrowIfCancellationRequested();
            throw; // Should not reach here
        }
    }

    /// <summary>
    /// Manually provides a move and allows the game to continue.
    /// </summary>
    public void Release(Move moveToPlay = default) {
        var oldTcs = _tcs;
        _tcs = new TaskCompletionSource<SearchResults>();
        oldTcs.TrySetResult(new SearchResults { BestMove = moveToPlay });
    }

    /// <summary>
    /// Forces the player to stop and throw an OperationCanceledException, 
    /// simulating an internal abort.
    /// </summary>
    public void EndGame() {
        // Use TrySetCanceled to trigger the cancellation path in ChooseMoveAsync
        _tcs.TrySetCanceled();
    }
}