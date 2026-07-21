using ChessBotCore.Players;

namespace ChessBotCore;

public interface IPlayer {
    public SearchHandle GetBestMove(State state, Timers timers);
}


public abstract class PlayerException : ApplicationException {
    protected PlayerException(string message) : base(message) { }
}

/// <summary>
/// A general exception, an <see cref="IPlayer"/> should throw, if the move selection has failed.
/// </summary>
public class MoveException : PlayerException {
    public MoveException(string message) : base(message) { }
}

//
// public class InvalidMoveFormatException : PlayerException {
//     public InvalidMoveFormatException(string move) : base($"Invalid move format: {move}") { }
// }
