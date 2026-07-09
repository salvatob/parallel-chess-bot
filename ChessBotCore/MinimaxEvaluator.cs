using System.Runtime.CompilerServices;

namespace ChessBotCore;

public struct SearchStats {
    public ulong NodesSearched;
}

public struct SearchResults {
    public Move BestMove;
    public Move PrincipalVariation;
    public SearchStats Stats;
    
    public override string ToString() => $"{BestMove}, {Stats.NodesSearched} nodes searched";
}

public struct MinimaxEvaluator {
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Eval(State s) => Evaluator.Evaluate(s);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsTerminal(State s) => Evaluator.IsTerminal(s);

    private SearchStats _lastSearchStats;
    
    public SearchResults ChooseBestMove(State state, int maxDepth) {
        var bestMove =  NegamaxBase(state, maxDepth);
        var results = new SearchResults {
            BestMove = bestMove,
            Stats = _lastSearchStats
        };
        
        _lastSearchStats = default; // !Important! resetting the stats
        return results;
    }

    private Move NegamaxBase(State state, int maxDepth) {
        
        var moves = new GeneratorWrapper(state).GetLegalMoves();
        moves.Sort();
        
        int bestScore =  int.MinValue;
        Move bestMove = default;
        
        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            int currentScore = - SmartABNegamax(state, maxDepth - 1, int.MinValue+2, int.MaxValue-2);
            state.UndoMove(move, undo);

            if (currentScore > bestScore) {
                bestMove = move;
                bestScore = currentScore;
            }
        }

        return bestMove;
    }

    internal int Negamax(State state, int depth) {
        if (depth <= 0 || IsTerminal(state)) {
            var score = Eval(state);
            return state.WhiteIsActive ? score : -score;
        } 
        
        int bestScore = int.MinValue;
        
        
        var moves = new GeneratorWrapper(state).GetLegalMoves();
        moves.Sort();
        
        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            int currentScore = - Negamax(state, depth - 1);
            state.UndoMove(move, undo);
            
            bestScore = int.Max(bestScore, currentScore);
        
        }

        return bestScore;
    }
    
    // TODO be careful with the a-b values inialization thay will overflow
    internal int ABNegamax(State state, int depth, int alpha, int beta) {
        bool isMaxing = state.WhiteIsActive;
        
        if (depth <= 0 || IsTerminal(state)) {
            var score = Eval(state);
            return isMaxing ? score : -score;
        } 
        
        int bestScore = int.MinValue;
        
        
        var moves = new GeneratorWrapper(state).GetLegalMoves();
        // TODO allow sorting
        // moves.Sort();
        
        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            int currentScore = - ABNegamax(state, depth - 1, -beta, -alpha);
            state.UndoMove(move, undo);
            
            bestScore = int.Max(bestScore, currentScore);
            alpha = int.Max(alpha, bestScore);
            if (alpha >= beta) break;
        
        }

        return bestScore;
    }

    // TODO be careful with the a-b values initialization, they will overflow
    internal int SmartABNegamax(State state, int depth, int alpha, int beta) {
        Interlocked.Increment(ref _lastSearchStats.NodesSearched);
        bool isMaxing = state.WhiteIsActive;
        
        if (depth <= 0 || IsTerminal(state)) {
            
            var score = Eval(state);
            return isMaxing ? score : -score;
        } 
        
        int bestScore = int.MinValue+2;
        
        
        var moves = new GeneratorWrapper(state).GetAllMoves();
        moves.Sort();
        
        foreach (var move in moves) {
            // this is faster, because with pruning, the generator will skip the expensive move legality check altogether
            if (!GeneratorWrapper.CheckMoveLegality(move, state))
                continue;
            var undo = state.ApplyMove(move);
            int currentScore = - SmartABNegamax(state, depth - 1, -beta, -alpha);
            state.UndoMove(move, undo);
            
            bestScore = int.Max(bestScore, currentScore);
            alpha = int.Max(alpha, bestScore);
            if (alpha >= beta) break;
        
        }

        return bestScore;
    }
    
}