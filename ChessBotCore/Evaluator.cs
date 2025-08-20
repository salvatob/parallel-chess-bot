namespace ChessBotCore;

public static class Evaluator {
    public static int Evaluate(State s) {
        // short circuit whole evaluation, because no king means loss
        // returns short, because it is enough to always be larger than normal value,
        // and avoids some overflowing further
        if (s.WhiteKing.RawBits == 0) return short.MinValue;
        if (s.BlackKing.RawBits == 0) return short.MaxValue;

        int val = 0;
        val += EvalPawns(s);
        val += EvalRooks(s);
        val += EvalQueens(s);
        val += EvalKnights(s);
        val += EvalBishops(s);
        
        return val;
    }

    
    private static int EvalPawns(State s) {
        int val = 0;
        const int valPerPawn = 40;
        const int valPerRow = 10;
        
        for (int i = 0; i < 8; i++) {
            int rowCount = (s.WhitePawns & BitMask.Row[i]).PopCount();
            val += rowCount * (valPerRow * i + valPerPawn);
        }
        
        for (int i = 0; i < 8; i++) {
            int rowCount = (s.BlackPawns & BitMask.Row[i]).PopCount();
            val -= rowCount * (valPerRow * (7 - i) + valPerPawn);
        }

        return val;
    }

    private static int EvalKnights(State s) {
        const int valPerKnight = 230;
        int val = 0;
        val += s.WhiteKnights.PopCount() * valPerKnight;
        val -= s.BlackKnights.PopCount() * valPerKnight;
        return val;
    }

    private static int EvalBishops(State s) {
        const int valPerBishop = 250;
        int val = 0;
        val += s.WhiteBishops.PopCount() * valPerBishop;
        val -= s.BlackBishops.PopCount() * valPerBishop;
        return val;
    }

    private static int EvalRooks(State s) {
        const int valPerRook = 450;
        int val = 0;
        val += s.WhiteRooks.PopCount() * valPerRook;
        val -= s.BlackRooks.PopCount() * valPerRook;
        return val;
    }
    
    private static int EvalQueens(State s) {
        const int valPerQueen = 900;
        int val = 0;
        val += s.WhiteQueens.PopCount() * valPerQueen;
        val -= s.BlackQueens.PopCount() * valPerQueen;
        return val;
    }
}