namespace ChessBotCore;

public sealed class ParallelChessWrapper : ChessWrapperBase {
    private long _nodesExplored = 0;
    private object _nodesLock = new();
    
    public GeneratorWrapper Generator = GeneratorWrapper.Default;


    public override long Perft(State state, int depth) {
        _nodesExplored = 0;
        if (depth <= 0) return 1;
        
        var po = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        var moves = Generator.GetLegalMoves(state);
        
        Parallel.ForEach( 
            moves,
            po,
            move => {
                var nextState = state.Clone();
                nextState.ApplyMove(move);
                long nodexFound = PerftHelper(nextState, depth - 1);
                
                Interlocked.Add(ref _nodesExplored, nodexFound);
                // lock (_nodesLock) {
                //     _nodesExplored += nodexFound;
                // }

            }
        );

        return _nodesExplored;        
    }


    private long PerftHelper(State state, int depth) {
        if (depth <= 0) return 1;
        
        long nodesExplored = 0;

        foreach (var move in Generator.GetLegalMoves(state)) {
            var undo = state.ApplyMove(move);
            nodesExplored += PerftHelper(state, depth - 1);
            state.UndoMove(move, undo);
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
                var nextState = state.Clone();
                nextState.ApplyMove(move);
                long currScore = EvalPerftHelper(nextState, depth - 1);
                
                lock (scoreLock) {
                    score += currScore;
                }

            }
        );

        return score;   
    }

    public override Task<Move> GetBestMove(State state, int timeMs) {
        throw new NotImplementedException();
    }

    private long EvalPerftHelper(State state, int depth) {
        if (depth <= 0) return Evaluator.Evaluate(state);
        
        long score = 0;

        foreach (var move in Generator.GenerateMoves(state)) {
            var undo = state.ApplyMove(move);
            score += EvalPerftHelper(state, depth - 1);
            state.UndoMove(move, undo);
        }

        return score;
    }
}