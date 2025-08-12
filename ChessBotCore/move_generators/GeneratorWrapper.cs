namespace ChessBotCore;

public sealed class GeneratorWrapper : IMoveGenerator {

    private IMoveGenerator[] _generators;

    public GeneratorWrapper(params IMoveGenerator[] generators) {
        _generators = generators;
    }

    public static GeneratorWrapper Default => new GeneratorWrapper(
        KingMoveGenerator.Instance,
        KnightMoveGenerator.Instance,
        RookMoveGenerator.Instance ,
        // TODO
        // PawnMoveGenerator.Instance,
        QueenMoveGenerator.Instance,
        BishopMoveGenerator.Instance
    );
    
    public IEnumerable<Move> GenerateMoves(State state) {
        foreach (IMoveGenerator generator in _generators) {
            foreach (Move move in generator.GenerateMoves(state)) {
                yield return move;
            }
        }
    }

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