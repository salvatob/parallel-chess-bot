namespace ChessBotCore.Search;

public class SearchHandle : IDisposable {
    private readonly CancellationTokenSource _cts;
    public Task<SearchResults> Result { get; }

    internal SearchHandle(CancellationTokenSource cts, Task<SearchResults> result) {
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
