namespace ChessBotCore;

/// <summary>
/// Provides an abstraction over move generation.
/// In itself should not be bound to the Singleton pattern of some specific move generator implementations.
/// </summary>
public interface IMoveGenerator {
    public IEnumerable<Move> GenerateMoves(State state);
}