using ChessBotCore.Game;
using ChessBotCore.Players;

namespace ChessBotCore;

public interface IPlayer : IDisposable {
    /// <summary>
    ///     Runs a task that returns a chess move from the current position.
    /// </summary>
    /// <param name="state">The state from which the player should move.</param>
    /// <param name="timers">Information about time both players have to play.</param>
    /// <throws> <see cref="MoveException" /> if there is a problem with the move retrieval.</throws>
    /// <returns>A nonblocking move Result Handle.</returns>
    public SearchHandle ChooseMoveAsync(State state, Timers timers);

    /// <summary>
    ///     Should be called for both players by the game owner before the game itself starts.\
    ///     It is okay for this method to be empty for some IPlayers
    /// </summary>
    /// <param name="yourColor">The color of the player. true is white</param>
    /// <param name="state">The initial state of the game.</param>
    /// <param name="timers">The time info for both players.</param>
    /// <param name="moveHistory">Reference to a record of played moves, if the IPlayer wants to store it.</param>
    /// <returns>A void task signifying if the method ran successfully.</returns>
    public Task OnGameStartAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> moveHistory);

    /// <summary>
    ///     Should be called for both players by the game owner after the game ends, to notify them of the results.\
    ///     It is okay for this method to be empty for some IPlayers.
    /// </summary>
    /// <param name="yourColor"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public Task OnGameGameEndAsync(bool yourColor, GameResult result);

    /// <summary>
    ///     Notify the player that an error has occured in the lifetime of the player, or the game.
    /// </summary>
    /// <param name="error">The specific error that occured.</param>
    /// <param name="gameEnd">True, it the error was so critical, that the game cannot continue.</param>
    /// <returns></returns>
    public Task OnErrorNotifyAsync(Exception error, bool gameEnd);

    /// <summary>
    ///     Notify the player that an error has occured in the lifetime of the player, or the game.
    /// </summary>
    /// <param name="errorMessage">The error message that occured.</param>
    /// <param name="gameEnd">True, it the error was so critical, that the game cannot continue.</param>
    /// <returns></returns>
    public Task OnErrorNotifyAsync(string errorMessage, bool gameEnd);
}

public abstract class PlayerException : ApplicationException {
    protected PlayerException(string message) : base(message) { }
}

/// <summary>
///     A general exception, an <see cref="IPlayer" /> should throw, if the move selection has failed.
/// </summary>
public class MoveException : PlayerException {
    public MoveException(string message) : base(message) { }
}

//
// public class InvalidMoveFormatException : PlayerException {
//     public InvalidMoveFormatException(string move) : base($"Invalid move format: {move}") { }
// }
