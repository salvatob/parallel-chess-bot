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
    ///     Prepares the player for an upcoming game.
    ///     This method should be used to provide the player with necessary game context
    ///     that wasn't available at construction time,
    ///     or to run something that takes too much time to be called in a constructor.
    /// </summary>
    /// <param name="yourColor">The color assigned to this player. True for white, false for black.</param>
    /// <param name="state">The initial state of the game board.</param>
    /// <param name="timers">The time settings for both players.</param>
    /// <param name="moveHistory">A reference to the list of moves played so far.</param>
    /// <returns>A task representing the preparation process.</returns>
    public Task PrepareAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> moveHistory);

    /// <summary>
    ///     Notifies the player that the game has officially started and clocks are running.
    ///     This is called after <see cref="PrepareAsync" /> and signifies the beginning of active play.
    /// </summary>
    /// <returns>A task representing the notification process.</returns>
    public Task OnGameStartAsync();

    /// <summary>
    ///     Notifies the player that the game has ended and provides the outcome information.
    ///     Should be called for both players by the game owner after the game ends, to notify them of the results.
    /// </summary>
    /// <param name="yourColor"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public Task OnGameEndAsync(bool yourColor, GameResult result);

    /// <summary>
    ///     Notify the player that their opponent has played a move.
    /// </summary>
    /// <param name="move">The move the opponent played.</param>
    /// <param name="newState">The state of the board after the move was played.</param>
    /// <returns>A void task signifying if the method ran successfully.</returns>
    public Task OnOpponentsMoveAsync(Move move, State newState);

    /// <summary>
    ///     Notify the player that an error has occurred in the lifetime of the player, or the game.
    /// </summary>
    /// <param name="error">The specific error that occurred.</param>
    /// <param name="gameEnd">True, it the error was so critical that the game cannot continue.</param>
    /// <returns></returns>
    public Task OnErrorNotifyAsync(Exception error, bool gameEnd);

    /// <summary>
    ///     Notify the player that an error has occurred in the lifetime of the player, or the game.
    /// </summary>
    /// <param name="errorMessage">The error message that occurred.</param>
    /// <param name="gameEnd">True, it the error was so critical that the game cannot continue.</param>
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
