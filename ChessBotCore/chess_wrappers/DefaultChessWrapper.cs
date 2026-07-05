namespace ChessBotCore;

public sealed class DefaultChessWrapper : ChessWrapperBase {
    
    public GeneratorWrapper Generator = GeneratorWrapper.Default;
    public MinimaxEvaluator Minimaxer;
    private const int _maxDepth = 4;
    
    
    public DefaultChessWrapper() {
        Minimaxer = new MinimaxEvaluator(Generator);
    }
    
    public override long Perft(State state, int depth) {
        if (depth <= 0) return 1;
        
        long nodesExplored = 0;

        foreach (var move in Generator.GetLegalMoves(state)) {
            var undo = state.ApplyMove(move);
            nodesExplored += Perft(state, depth - 1);
            state.UndoMove(move, undo);
        }

        return nodesExplored;
    }

    public override long EvalPerft(State state, int depth) {
        if (depth <= 0) return Evaluator.Evaluate(state);
        
        long score = 0;

        foreach (var move in Generator.GenerateMoves(state)) {
            var undo = state.ApplyMove(move);
            score += EvalPerft(state, depth - 1);
            state.UndoMove(move, undo);
        }

        return score;
    }

    public override Task<Move> GetBestMove(State state, int timeLeftMs) {
        return Task.FromResult(Minimaxer.ChooseBestMove(state, _maxDepth));
    }
}