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
        return NegamaxBase(state, maxDepth);
    }

    private Move NegamaxBase(State state, int maxDepth) {
        var moves = _generator.GenerateMoves(state).ToList();
        moves.Sort();
        
        int bestScore =  int.MinValue;
        Move bestMove = default;
        
        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            int currentScore = - Negamax(state, maxDepth - 1);
            state.UndoMove(move, undo);

            if (currentScore > bestScore) {
                bestMove = move;
                bestScore = currentScore;
            }
        };

        return bestMove;
    }

    private int Negamax(State state, int depth) {
        if (depth <= 0 || IsTerminal(state)) {
            var score = Eval(state);
            return state.WhiteIsActive ? score : -score;
        } 
        
        int bestScore = int.MinValue;
        
        
        var moves = _generator.GenerateMoves(state).ToList();
        moves.Sort();
        
        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            int currentScore = - Negamax(state, depth - 1);
            state.UndoMove(move, undo);
            
            bestScore = int.Max(bestScore, currentScore);
        
        };

        return bestScore;
    }

  
    
}