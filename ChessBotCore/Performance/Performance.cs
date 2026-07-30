namespace ChessBotCore.Performance;

public static class Performance {
    
    /// <summary>
    /// Performs a DFS search, to the specified depth, and returns the number of leaf nodes, including duplicate states.\
    /// For a faster alternative, see <seealso cref="ParallelPerft"/>
    /// </summary>
    /// <param name="state">State from which to count the leaves.</param>
    /// <param name="depth">Depth to which search.</param>
    /// <returns>The number of leaves found including duplicates.</returns>
    public static long Perft(State state, int depth) {
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

    
    /// <summary>
    /// Performs a parallel DFS search, to the specified depth, and returns the number of leaf nodes, including duplicate states.
    /// Should return the same exact results as <seealso cref="Perft"/>, but is around 6 times faster.
    /// </summary>
    /// <param name="state">State from which to count the leaves.</param>
    /// <param name="depth">Depth to which search.</param>
    /// <returns>The number of leaves found including duplicates.</returns>
    public static long ParallelPerft(State state, int depth) {
        var p = new ParallelPerformance();
        return p.InnerParallelPerft(state, depth);
    }
    
    /// <summary>
    /// Computes <seealso cref="Perft"/> values for each possible move, and returns a table. Useful for debugging.
    /// </summary>
    /// <param name="s">The state from which the search is performed.</param>
    /// <param name="depth">A depth to which perform the search.</param>
    /// <returns>A dictionary of leaf node counts from each move.</returns>
    public static Dictionary<Move, long> DividePerft(State s, int depth) {
        var moves = new GeneratorWrapper(s).GetLegalMoves().ToList();

        var moveCounts = new Dictionary<Move, long>();

        foreach (var m in moves) {
            var nextState = s.Clone();
            nextState.ApplyMove(m);
            var nodeCount = Perft(nextState, depth - 1);
            moveCounts[m] = nodeCount;
        }
        
        return moveCounts;
    }
    
    class ParallelPerformance {
        private long  _nodesExplored = 0;
        
         public long InnerParallelPerft(State state, int depth) {
            // _nodesExplored = 0;
            if (depth <= 0) return 1;

            var po = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            var moves = new GeneratorWrapper(state).GetLegalMoves();

            Parallel.ForEach(
                moves,
                po,
                move => {
                    var nextState = state.Clone();
                    nextState.ApplyMove(move);
                    long nodexFound = PerftHelper(nextState, depth - 1);

                    Interlocked.Add(ref _nodesExplored, nodexFound);
                }
            );

            return _nodesExplored;
        }


        private long PerftHelper(State state, int depth) {
            if (depth <= 0) return 1;

            long nodesExplored = 0;
            var moves = new GeneratorWrapper(state).GetLegalMoves();

            foreach (var move in moves) {
                var undo = state.ApplyMove(move);
                nodesExplored += PerftHelper(state, depth - 1);
                state.UndoMove(move, undo);
            }

            return nodesExplored;
        }
    }
}
