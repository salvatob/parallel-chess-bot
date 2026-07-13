using ChessBotCore;
using ChessBotCore.Search;

namespace ConsoleInterface;

public interface IPlayer {
    public Task<SearchResults> GetBestMove(State state, TimeSpan timeLeft, CancellationToken cancellationToken);
}

public class ConsolePlayer : IPlayer {
    public Task<SearchResults> GetBestMove(State state, TimeSpan timeLeft, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
    
}