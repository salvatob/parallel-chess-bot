using ChessBotCore.Search;

namespace ChessBotCore.Players;

/// <summary>
///     A handle returned by any <see cref="IPlayer" />, providing the caller an option to stop the search at any moment.
/// </summary>
public class SearchHandle : IDisposable {
    private readonly CancellationTokenSource _cts;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchHandle"/> class.
    /// </summary>
    /// <param name="cts">The cancellation token source for the search.</param>
    /// <param name="result">The task that will return the search results.</param>
    public SearchHandle(CancellationTokenSource cts, Task<SearchResults> result) {
        _cts = cts;
        Result = result;
    }

    /// <summary>
    /// Gets the task representing the ongoing search.
    /// </summary>
    public Task<SearchResults> Result { get; }

    /// <summary>
    /// Disposes the underlying cancellation token source.
    /// </summary>
    public void Dispose() {
        _cts.Dispose();
    }

    /// <summary>
    /// Requests that the search be cancelled.
    /// </summary>
    public void Cancel() {
        _cts.Cancel();
    }

    /// <summary>
    /// Registers a callback to be executed when the search is cancelled.
    /// </summary>
    /// <param name="callback">The callback to register.</param>
    public void Register(Action callback) {
        _cts.Token.Register(callback);
    }
}
