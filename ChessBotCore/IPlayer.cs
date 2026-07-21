using ChessBotCore.Players;

namespace ChessBotCore;

public interface IPlayer : IDisposable {
    /// <summary>
    /// Runs a task that returns a chess move from the current position.
    /// </summary>
    /// <param name="state">The state from which the player should move.</param>
    /// <param name="timers">Information about time both players have to play.</param>
    /// <throws> <see cref="MoveException"/> if there is a problem with the move retrieval.</throws>
    /// <returns>A nonblocking move Result Handle.</returns>
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
