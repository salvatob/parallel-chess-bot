namespace ChessBotCore;

/// <summary>
/// Provides an abstraction over move generation.
/// In itself Should not be bound to the Singleton pattern. 
/// </summary>
public interface IMoveGenerator {
    public IEnumerable<Move> GenerateMoves(State state);
}