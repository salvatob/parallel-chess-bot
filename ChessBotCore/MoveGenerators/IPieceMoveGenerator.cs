namespace ChessBotCore.MoveGenerators;

/// <summary>
/// Provides methods to fill a buffer with Moves.\
/// Primarily intended for individual piece generators
/// </summary>
public interface IPieceMoveGenerator {
    /// <summary>
    /// Appends newly generated moves to the provided buffer. 
    /// </summary>
    /// <param name="state">State from which to generate the moves</param>
    /// <param name="buffer">A possibly non empty list, where new moves should be stored.</param>
    public void GenerateMoves(State state, List<Move> buffer);
}