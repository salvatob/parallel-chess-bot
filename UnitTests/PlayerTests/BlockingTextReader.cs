namespace UnitTests.PlayerTests;

public class BlockingTextReader : TextReader {
    private TaskCompletionSource<string?>? _tcs;

    private readonly TaskCompletionSource _waiting =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Waiting => _waiting.Task;

    public override async Task<string?> ReadLineAsync() {
        return await ReadLineAsync(CancellationToken.None);
    }

    public override async ValueTask<string?> ReadLineAsync(CancellationToken ct) {
        _tcs = new TaskCompletionSource<string?>();
        _waiting.TrySetResult();

        try {
            return await _tcs.Task.WaitAsync(ct);
        } catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            ct.ThrowIfCancellationRequested();
            throw;
        }
    }

    /// <summary>
    /// Releases the reader with the specified value.
    /// </summary>
    public void Release(string? value = null) {
        _tcs?.TrySetResult(value);
    }
}