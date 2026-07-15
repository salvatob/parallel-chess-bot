using ChessBotCore.Search;

namespace ChessBotCore.Players;

/// <summary>
/// A handle returned by any <see cref="IPlayer"/>, providing the caller an option to stop the search at any moment. 
/// </summary>
public class SearchHandle : IDisposable {
    private readonly CancellationTokenSource _cts;
    public Task<SearchResults> Result { get; }

    public SearchHandle(CancellationTokenSource cts, Task<SearchResults> result) {
        _cts = cts;
        Result = result;
    }

    public void Cancel() {
        _cts.Cancel();
    }

    public void Register(Action callback) {
        _cts.Token.Register(callback);
    }
    
    public void Dispose() {
        // GC.SuppressFinalize(this);
        _cts.Dispose();
    }
    
}
