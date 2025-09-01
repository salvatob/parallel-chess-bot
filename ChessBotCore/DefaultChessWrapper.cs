namespace ChessBotCore;

public sealed class DefaultChessWrapper : ChessWrapperBase {
    
    public GeneratorWrapper Generator = GeneratorWrapper.Default;
    public MinimaxEvaluator Minimaxer;
    private const int _maxDepth = 6;
    
    
    public DefaultChessWrapper() {
        Minimaxer = new MinimaxEvaluator(Generator);
    }
    
    public override long Perft(State state, int depth) {


        if (depth <= 0) return 1;
        
        long nodesExplored = 1;

        foreach (var move in Generator.GenerateMoves(state)) {
            nodesExplored += Perft(move.StateAfter, depth - 1);
        }

        return nodesExplored;
    }

    public override long EvalPerft(State state, int depth) {

        long score = 0;
        if (depth <= 0) return Evaluator.Evaluate(state);
        

        foreach (var move in Generator.GenerateMoves(state)) {
            score += EvalPerft(move.StateAfter, depth - 1);
        }

        return score;
    }

    public override Task<Move> GetBestMove(State state, int timeLeftMs) {
        return Task.FromResult(Minimaxer.ChooseBestMove(state, _maxDepth));
    }
}