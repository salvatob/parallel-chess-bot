using ChessBotCore.Players;

namespace ChessBotCore;

public interface IPlayer {
    public SearchHandle GetBestMove(State state, Timers timers);
}
