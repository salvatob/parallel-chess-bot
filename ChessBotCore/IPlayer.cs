namespace ChessBotCore.Players;

public interface IPlayer {
    public SearchHandle GetBestMove(State state, Timers timers);
}
