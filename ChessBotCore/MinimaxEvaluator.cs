namespace ChessBotCore;

public sealed class MinimaxEvaluator {
    // private static readonly GeneratorWrapper Generator = GeneratorWrapper.Default;
    private readonly GeneratorWrapper _generator;
    private static int Eval(State s) => Evaluator.Evaluate(s);
    private static bool IsTerminal(State s) => Evaluator.IsTerminal(s);

    public MinimaxEvaluator(GeneratorWrapper moveGenerator) {
        _generator = moveGenerator;
    }
    
    public Move ChooseBestMove(State state, int maxDepth) {
        return MinimaxSetup(state, maxDepth);
    }

    private Move MinimaxSetup(State state, int maxDepth) {
        bool isMaxing = state.WhiteIsActive;
        
        // todo sort the moves somehow 
        using var moves = _generator.GetLegalMoves(state).ToList().GetEnumerator();

        
        // querying for a move when stalemated is undefined behaviour
        if (!moves.MoveNext()) return default;

        
        int bestScore = isMaxing ? int.MinValue : int.MaxValue;
        Move bestMove = default;

        do {
            int currentScore = Minimax(moves.Current.StateAfter, maxDepth - 1);
            if (isMaxing) {
                if (currentScore > bestScore) {
                    bestMove = moves.Current;
                    bestScore = currentScore;
                }
            }
            else {
                if (currentScore < bestScore) {
                    bestMove = moves.Current;
                    bestScore = currentScore;
                }
            }
            
        } while (moves.MoveNext());

        return bestMove;
    }

    private int Minimax(State state, int depth) {
        if (depth <= 0 || IsTerminal(state)) return Eval(state);
        
        bool isMaxing = state.WhiteIsActive;
        
        int bestScore = isMaxing ? int.MinValue : int.MaxValue;
        
        // todo sort the moves somehow 
        using var moves = _generator.GenerateMoves(state).GetEnumerator();

        // no moves means stalemate, which is loss for both players
        if (!moves.MoveNext()) return 0;

        do {
            int currentScore = Minimax(moves.Current.StateAfter, depth - 1);
            bestScore = isMaxing ? int.Max(bestScore, currentScore) : int.Min(bestScore, currentScore);

        } while (moves.MoveNext());
        

        return bestScore;
    }
}