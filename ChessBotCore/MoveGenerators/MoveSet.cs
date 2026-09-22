namespace ChessBotCore.MoveGenerators;

public class MoveSet {
    private readonly Lazy<List<Move>> _filteredBuffer;
    private readonly List<Move> _pseudoLegalMoves;

    public State State { get; }
    
    
    /// <summary>
    /// Gets all pseudo-legal moves in the current state.
    /// </summary>
    /// <returns>A list of moves.</returns>
    public List<Move> GetAllMoves() => _pseudoLegalMoves;
    
    /// <summary>
    /// Gets all legal moves in the current state, filtering out moves that leave the king in check.
    /// </summary>
    /// <returns>A list of legal moves.</returns>
    public List<Move> GetLegalMoves() => _filteredBuffer.Value;

    
    public MoveSet(State state, List<Move> buffer) {
        State = state;
        _pseudoLegalMoves = buffer;
        _filteredBuffer = new Lazy<List<Move>>(FilterLegalMoves);
        
    }
    
    private List<Move> FilterLegalMoves() {
        List<Move> filtered = new(20);
        foreach (var move in _pseudoLegalMoves) {
            if (MoveGenerator.CheckMoveLegality(move, State))
                filtered.Add(move);
        }
        return filtered;
    }
}
