namespace ChessBotCore;

public sealed class GeneratorWrapper : IMoveGenerator {

    private IMoveGenerator[] _generators;

    public GeneratorWrapper(params IMoveGenerator[] generators) {
        if (generators.Length == 0)
            throw new ArgumentException("At least one generator should be provided");
        _generators = generators;
    }

    
    public static GeneratorWrapper Default => new GeneratorWrapper(
        KingMoveGenerator.Instance,
        KnightMoveGenerator.Instance,
        RookMoveGenerator.Instance ,
        PawnMoveGenerator.Instance,
        QueenMoveGenerator.Instance,
        BishopMoveGenerator.Instance
    );
    
    /// <summary>
    /// Generates all pseudo-legal moves from a set state struct
    /// </summary>
    /// <param name="state">The state from which to generate the moves</param>
    /// <returns>Lazy IEnumerable of Move structs</returns>
    public IEnumerable<Move> GenerateMoves(State state) {
        foreach (IMoveGenerator generator in _generators) {
            foreach (Move move in generator.GenerateMoves(state)) {
                yield return move;
            }
        }
    }

    
    /// <summary>
    /// Generates all <b>LEGAL</b> moves from a set state struct
    /// </summary>
    /// <param name="state">The state from which to generate the moves</param>
    /// <returns>Lazy IEnumerable of Move structs</returns>
    public IEnumerable<Move> GetLegalMoves(State state) {
        return GenerateMoves(state).Where(CheckMoveLegality);
    }

    private bool CheckMoveLegality(Move move) {
        // castles are already checked
        if (move.IsCastle) return true;
        
        // if true, we are checking for white king
        bool kingColor = !move.StateAfter.WhiteIsActive;
        foreach (Move moveAfter in GenerateMoves(move.StateAfter)) {
            if (kingColor) {
                if (moveAfter.StateAfter.WhiteKing.IsEmpty()) return false;
            } else {
                if (moveAfter.StateAfter.BlackKing.IsEmpty()) return false;
            }
        }

        return true;
    }
}