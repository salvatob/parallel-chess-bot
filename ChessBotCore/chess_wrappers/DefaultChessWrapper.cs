namespace ChessBotCore;

public sealed class DefaultChessWrapper : ChessWrapperBase {
    
    public MinimaxEvaluator Minimaxer;
    private const int _maxDepth = 4;
    
    
    public DefaultChessWrapper() {
        Minimaxer = new MinimaxEvaluator();
    }
    
    public override long Perft(State state, int depth) {
        if (depth <= 0) return 1;
        
        long nodesExplored = 0;
        var moves = new GeneratorWrapper(state).GetLegalMoves();
        
        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            nodesExplored += Perft(state, depth - 1);
            state.UndoMove(move, undo);
        }

        return nodesExplored;
    }

    public override long EvalPerft(State state, int depth) {
        if (depth <= 0) return Evaluator.Evaluate(state);
        
        long score = 0;

        var moves = new GeneratorWrapper(state).GetLegalMoves();
        
        foreach (var move in moves) {
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