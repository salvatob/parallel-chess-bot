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
        
        // After ApplyMove, WhiteIsActive has flipped.
        // If white just moved, it's now black's turn. 
        // We need to check if white's king is under attack.
        bool wasWhiteTurn = !state.WhiteIsActive;
        Bitboard kingBoard = wasWhiteTurn ? state.WhiteKing : state.BlackKing;
        
        bool legal;
        if (kingBoard.IsEmpty()) {
            legal = false; // Should not happen if king was there before
        } else {
            int kingSquare = kingBoard.TrailingZeroCount();
            legal = !IsSquareAttacked(kingSquare, state.WhiteIsActive, state);
        }

        state.UndoMove(move, undo);
        return legal;
    }

    private static bool IsSquareAttacked(int square, bool byWhite, State state) {
        Bitboard squareMask = BitBoardHelpers.OneBitMask(square);
        Bitboard allPieces = state.GetAllPieces();

        // 1. Knights
        Bitboard knightAttackers = GetKnightAttacks(square) & (byWhite ? state.WhiteKnights : state.BlackKnights);
        if (!knightAttackers.IsEmpty()) return true;

        // 2. Pawns
        if (byWhite) {
            if (!( (squareMask.MovePieces(Direction.SW) & state.WhitePawns).IsEmpty() && 
                   (squareMask.MovePieces(Direction.SE) & state.WhitePawns).IsEmpty() ))
                return true;
        } else {
            if (!( (squareMask.MovePieces(Direction.NW) & state.BlackPawns).IsEmpty() && 
                   (squareMask.MovePieces(Direction.NE) & state.BlackPawns).IsEmpty() ))
                return true;
        }

        // 3. King
        Bitboard kingAttackers = GetKingAttacks(square) & (byWhite ? state.WhiteKing : state.BlackKing);
        if (!kingAttackers.IsEmpty()) return true;

        // 4. Sliding Pieces (Rooks, Bishops, Queens)
        // Orthogonal (Rook/Queen)
        Direction[] orthoDirs = [Direction.N, Direction.S, Direction.E, Direction.W];
        Bitboard orthoSliders = byWhite ? (state.WhiteRooks | state.WhiteQueens) : (state.BlackRooks | state.BlackQueens);
        foreach (var dir in orthoDirs) {
            if (!GetSliderAttack(square, dir, allPieces, orthoSliders).IsEmpty())
                return true;
        }

        // Diagonal (Bishop/Queen)
        Direction[] diagDirs = [Direction.NE, Direction.NW, Direction.SE, Direction.SW];
        Bitboard diagSliders = byWhite ? (state.WhiteBishops | state.WhiteQueens) : (state.BlackBishops | state.BlackQueens);
        foreach (var dir in diagDirs) {
            if (!GetSliderAttack(square, dir, allPieces, diagSliders).IsEmpty())
                return true;
        }

        return false;
    }

    private static Bitboard GetKnightAttacks(int square) {
        Bitboard mask = BitBoardHelpers.OneBitMask(square);
        return BitBoardHelpers.Move(mask, Direction.NNE) |
               BitBoardHelpers.Move(mask, Direction.NEE) |
               BitBoardHelpers.Move(mask, Direction.SEE) |
               BitBoardHelpers.Move(mask, Direction.SSE) |
               BitBoardHelpers.Move(mask, Direction.NNW) |
               BitBoardHelpers.Move(mask, Direction.NWW) |
               BitBoardHelpers.Move(mask, Direction.SWW) |
               BitBoardHelpers.Move(mask, Direction.SSW);
    }

    private static Bitboard GetKingAttacks(int square) {
        Bitboard mask = BitBoardHelpers.OneBitMask(square);
        return BitBoardHelpers.Move(mask, Direction.N) |
               BitBoardHelpers.Move(mask, Direction.S) |
               BitBoardHelpers.Move(mask, Direction.E) |
               BitBoardHelpers.Move(mask, Direction.W) |
               BitBoardHelpers.Move(mask, Direction.NE) |
               BitBoardHelpers.Move(mask, Direction.NW) |
               BitBoardHelpers.Move(mask, Direction.SE) |
               BitBoardHelpers.Move(mask, Direction.SW);
    }

    private static Bitboard GetSliderAttack(int square, Direction dir, Bitboard allPieces, Bitboard attackers) {
        Bitboard ray = BitBoardHelpers.OneBitMask(square);
        while (true) {
            ray = ray.MovePieces(dir);
            if (ray.IsEmpty()) break;
            if (!(ray & attackers).IsEmpty()) return ray;
            if (!(ray & allPieces).IsEmpty()) break; // Blocked by some piece
        }
        return Bitboard.Empty;
    }
}