using ChessBotCore;
using ChessBotCore.Search;
using ChessBotCore.Players;

namespace ConsoleInterface;

public class ConsolePlayer : IPlayer {
    public SearchHandle GetBestMove(State state, Timers timers) {
        throw new NotImplementedException();
    }
}