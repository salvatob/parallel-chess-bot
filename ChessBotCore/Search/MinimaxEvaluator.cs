using System.Diagnostics;
using System.Runtime.CompilerServices;
using ChessBotCore.MoveGenerators;
using ChessBotCore.Players;

namespace ChessBotCore.Search;



public class SearchStats {
    public ulong NodesSearched;
}

public struct SearchResults {
    public Move BestMove;
    public Move? PrincipalVariation;
    public SearchStats? Stats;
    
    public override string ToString() {
        if (Stats is null) return BestMove.ToString();
        return $"{BestMove}, {Stats?.NodesSearched} nodes searched";
    }
}

/// <summary>
/// A type encapsulating a Negamax-based State Space Search of the best move. It is not thread-safe.
/// </summary>
public class MinimaxEvaluator {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Eval(State s) => Evaluator.Evaluate(s);
    
    private SearchResults ChooseBestMove(State state, int maxDepth, SearchContext searchContext) {
        State copy = state.Clone();
        var bestMove =  NegamaxBase(copy, maxDepth, searchContext, searchContext.Alpha, searchContext.Beta);
        var results = new SearchResults {
            BestMove = bestMove,
            Stats = searchContext.Stats
        };
         // !Important! resetting the stats
        return results;
    }

    /// <summary>
    /// Everything relevant to all threads working on the current search.
    /// Currently isn't thread safe.
    /// </summary>
    internal class SearchContext {
        public CancellationToken CancellationToken { get; init; }
        
        public bool StopRequested { get; private set; } = false;
        public Stopwatch Stopwatch { get; } = new ();
        public SearchStats Stats { get; private set; } = new();

        // TODO think of a way to keep it thread safe
        public int Alpha { get; set; } = int.MinValue+1; // to prevent negation overflow
        public int Beta { get; set; } = int.MaxValue;

        public void IncrementNodeCount() {
            Interlocked.Increment(ref Stats.NodesSearched);
        }
        
        public bool ShouldStop() {
            // in future this should also handle things like if we already found mate,
            // or in case of another thread finding a dominating move (alpha beta hit).
            
            if ((Stats.NodesSearched & 0x3FFF) != 0)
                return StopRequested;

            if (CancellationToken.IsCancellationRequested)
                StopRequested = true;

            return StopRequested;
        }
    }
    
    // TODO this needs to be much better in time, hopefully the API stays the same tho
    public SearchResults PrimitiveIterativeSearch(State state, Timers timers, CancellationToken cancellationToken) {
        // context (mainly its stopwatch) should start ASAP
        var context = new SearchContext {
            CancellationToken = cancellationToken
        };
        context.Stopwatch.Start();
        
        List<SearchResults> results = [] ;
        TimeSpan plannedTimeForSearch = timers.ActiveTime(state.WhiteIsActive)/ 20 + timers.Increment / 2;

        
        int depth = 1;
        while (true) {
            if (cancellationToken.IsCancellationRequested) 
                return results.Last();
            var r = ChooseBestMove(state, depth, context);
            Console.WriteLine($"Search of depth {depth} hase yielded move {r.BestMove}");
            results.Add(r);
            depth++;
        }
    }

    
    private Move NegamaxBase(State state, int maxDepth, SearchContext searchContext, int alpha, int beta) {
        var moves = new GeneratorWrapper(state).GetLegalMoves();
        moves.Sort();
        
        int bestScore =  int.MinValue;
        Move bestMove = default;
        
        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            int currentScore = - SmartABNegamax(state, maxDepth - 1, searchContext , -beta, -alpha);
            state.UndoMove(move, undo);

            if (currentScore > bestScore) {
                bestMove = move;
                bestScore = currentScore;
            }
        }

        return bestMove;
    }

    internal int Negamax(State state, int depth) {
        if (depth <= 0 || state.IsTerminal()) {
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
    
    // be careful with the a-b values initialization that will overflow
    internal int ABNegamax(State state, int depth, int alpha, int beta) {
        bool isMaxing = state.WhiteIsActive;
        
        if (depth <= 0 || state.IsTerminal()) {
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

    // be careful with the a-b values initialization, they will overflow
    internal int SmartABNegamax(State state, int depth, SearchContext searchContext, int alpha, int beta) {
        
        if (searchContext.ShouldStop()) {
            return int.MaxValue;
        }
    
        bool isMaxing = state.WhiteIsActive;
        
        if (depth <= 0 || state.IsTerminal()) {
            var score = Eval(state);
            return isMaxing ? score : -score;
        } 
        
        
        var moves = new GeneratorWrapper(state).GetAllMoves();
        if (moves.Count == 0) return 0; // stalemate
        
        moves.Sort();
        
        int bestScore = int.MinValue+2;
        
        foreach (var move in moves) {
            // this is faster, because with pruning, the generator will skip the expensive move legality check altogether
            if (!GeneratorWrapper.CheckMoveLegality(move, state))
                continue;
            var undo = state.ApplyMove(move);
            int currentScore = - SmartABNegamax(state, depth - 1, searchContext, -beta, -alpha);
            state.UndoMove(move, undo);
            
            bestScore = int.Max(bestScore, currentScore);
            alpha = int.Max(alpha, bestScore);
            if (alpha >= beta) break;
        
        }

        return bestScore;
    }
    
}