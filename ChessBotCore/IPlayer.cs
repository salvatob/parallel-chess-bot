using ChessBotCore.Game;
using ChessBotCore.Players;

namespace ChessBotCore;

public interface IPlayer : IAsyncDisposable {
    /// <summary>
    /// Runs a task that returns a chess move from the current position.
    /// </summary>
    /// <param name="state">The state from which the player should move.</param>
    /// <param name="timers">Information about time both players have to play.</param>
    /// <throws> <see cref="MoveException"/> if there is a problem with the move retrieval.</throws>
    /// <returns>A nonblocking move Result Handle.</returns>
    public SearchHandle ChooseMoveAsync(State state, Timers timers);

    /// <summary>
    /// Should be called for both players by the game owner before the game itself starts.\
    /// It is okay for this method to be empty for some IPlayers
    /// </summary>
    /// <param name="yourColor">The color of the player. true is white</param>
    /// <param name="state">The initial state of the game.</param>
    /// <returns>A void task signifying if the method ran successfully.</returns>
    public Task OnGameStartAsync(bool yourColor, State state);

    /// <summary>
    /// Should be called for both players by the game owner after the game ends, to notify them of the results.\
    /// It is okay for this method to be empty for some IPlayers.
    /// </summary>
    /// <param name="yourColor"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public Task OnGameGameEndAsync(bool yourColor, GameResult result);
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
