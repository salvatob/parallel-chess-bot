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
        foreach (var move in GenerateMoves(state)) {
            if (CheckMoveLegality(move, state)) {
                yield return move;
            }
        }
    }

    private bool CheckMoveLegality(Move move, State state) {
        // castles are already checked
        if (move.IsCastle) return true;
        
        var undo = state.ApplyMove(move);
        bool legal = true;
        
        // After ApplyMove, WhiteIsActive has flipped.
        // If white just moved, it's now black's turn. 
        // We need to check if white's king is under attack.
        bool wasWhiteTurn = !state.WhiteIsActive;
        Bitboard kingBoard = wasWhiteTurn ? state.WhiteKing : state.BlackKing;
        
        if (kingBoard.IsEmpty()) {
            legal = false; // Should not happen if king was there before
        } else {
            int kingSquare = kingBoard.TrailingZeroCount();
            foreach (Move response in GenerateMoves(state)) {
                if (response.To == kingSquare) {
                    legal = false;
                    break;
                }
            }
        }

        state.UndoMove(move, undo);
        return legal;
    }
}