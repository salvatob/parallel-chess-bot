namespace ChessBotCore;

public class ParallelChessWrapper : ChessWrapperBase {
    private long _nodesExplored = 0;
    private object _nodesLock = new();
    
    public GeneratorWrapper Generator = GeneratorWrapper.Default;


    public override long Perft(State state, int depth) {
        _nodesExplored = 1;
        if (depth <= 0) return 1;
        
        var po = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        var moves = Generator.GenerateMoves(state);
        Parallel.ForEach( 
            moves,
            po,
            move => {
                long nodexFound = PerftHelper(move.StateAfter, depth - 1);
                
                lock (_nodesLock) {
                    _nodesExplored += nodexFound;
                }

            }
        );

        return _nodesExplored;        
    }


    private long PerftHelper(State state, int depth) {

        if (depth == 0) return 1;
        
        long nodesExplored = 1;

        foreach (var move in Generator.GenerateMoves(state)) {
            nodesExplored += PerftHelper(move.StateAfter, depth - 1);
        }

        return nodesExplored;
    }
    
    
    public override long EvalPerft(State state, int depth) {
        long score = 0;
        object scoreLock = new object();
        
        
        var po = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        var moves = Generator.GenerateMoves(state);
        
        Parallel.ForEach( 
            moves,
            po,
            move => {
                long currScore = EvalPerftHelper(move.StateAfter, depth - 1);
                
                lock (scoreLock) {
                    score += currScore;
                }

            }
        );

        return score;   
    }
    
    private long EvalPerftHelper(State state, int depth) {
        long score = 0;

        if (depth <= 0) return Evaluator.Evaluate(state);
        

        foreach (var move in Generator.GenerateMoves(state)) {
            score += EvalPerft(move.StateAfter, depth - 1);
        }

        return score;
    }
}