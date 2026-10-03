namespace UnitTests.PlayerTests;

public class BlockingTextReader : TextReader {
 
    private readonly TaskCompletionSource _waiting =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Waiting => _waiting.Task;
    
    public override Task<string?> ReadLineAsync() {
        // should block here somehow
        _waiting.TrySetResult();
        
        return base.ReadLineAsync();
    }
}