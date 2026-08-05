using System.Runtime.CompilerServices;
using ChessBotCore.Game;

namespace ChessBotCore.Search;


/// <summary>
///     A type encapsulating a Negamax-based State Space Search of the best move. It is not thread-safe.
/// </summary>
public class MinimaxEvaluator {
    private readonly ITimeManager _timeManager = new TimeManager();

    public  MinimaxEvaluator() {}
    public MinimaxEvaluator(ITimeManager timeManager) {
        _timeManager = timeManager;
    }
    
    
    private static SearchScore Eval(State s) {
        int score = Evaluator.Evaluate(s);
        bool isMate = score is short.MinValue or short.MaxValue;
        return new SearchScore(score, isMate);
    }

    

    /// <summary>
    /// A simple way to run the search.
    /// </summary>
    /// <param name="state">The state from which to search the best move</param>
    /// <param name="maxDepth">The depth to which to run the search.</param>
    /// <returns></returns>
    public SearchResults ChooseBestMove(State state, int maxDepth) {
        var searchContext = new SearchContext();
        State copy = state.Clone();
        var (bestMove, bestScore) = NegamaxBase(copy, maxDepth, searchContext, searchContext.Alpha, searchContext.Beta);
        var results = new EngineSearchResults {
            BestMove = bestMove,
            Score = bestScore,
            Stats = searchContext.Stats,
            MaxDepth = maxDepth
        };

        return results;
    }

    public SearchResults PrimitiveIterativeSearch(State state, Timers timers, CancellationToken cancellationToken) {
        var timePerMove = _timeManager.CalculateAllowedTime(timers, state.WhiteIsActive);
        
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timePerMove);
        
        var scoredMoves = GetInitialScoredMoves(state);
        if (scoredMoves.Count == 0) throw new InvalidOperationException("Current State has no moves available, so it cannot be searched.");
        
        var bestMove =  scoredMoves[0];
        var lastCompletedResult = new EngineSearchResults() {
            BestMove = bestMove.Move,
            Score = bestMove.Score,
            Stats = new SearchStats {NodesSearched = (ulong)scoredMoves.Count},
            MaxDepth = 1
        };

        for (int depth = 2; depth <= 100; depth++) {
            var context = new SearchContext {
                CancellationToken = cts.Token,
                Stats = new SearchStats(){  }
            };
            if (context.ShouldStop()) break;

            PerformSearchIteration(state, depth, context, scoredMoves);
            
            if (!context.StopRequested) {
                scoredMoves.Sort((a, b) => b.Score.CompareTo(a.Score));
                
                lastCompletedResult = new EngineSearchResults(scoredMoves[0], context.Stats) {
                    Stats = context.Stats
                };
            }
        }

        return lastCompletedResult;
    }

    private static List<ScoredMove> GetInitialScoredMoves(State state) {
        var moves = new GeneratorWrapper(state).GetLegalMoves();
        moves.Order();
        var scoredMoves = new List<ScoredMove>(moves.Count);
        foreach (var move in moves) {
            scoredMoves.Add(new ScoredMove(move, new SearchScore(0)));
        }
        return scoredMoves;
    }

    private void PerformSearchIteration(State state, int depth, SearchContext context, List<ScoredMove> scoredMoves) {
        for (int i = 0; i < scoredMoves.Count; i++) {
            if (context.ShouldStop()) break;

            var move = scoredMoves[i].Move;
            var undo = state.ApplyMove(move);
            SearchScore score = -SmartABNegamax(state, depth - 1, context, -context.Beta, -context.Alpha);
            state.UndoMove(move, undo);

            if (!context.StopRequested) {
                scoredMoves[i] = new ScoredMove(move, score);
            }
        }
    }

    private ScoredMove NegamaxBase(State state, int maxDepth, SearchContext searchContext, int alpha,
        int beta) {
        searchContext.IncrementNodeCount();
        var moves = new GeneratorWrapper(state).GetLegalMoves();
        moves.Sort();

        SearchScore bestScore = new SearchScore(int.MinValue + 1);
        Move bestMove = default;

        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            SearchScore currentScore = -SmartABNegamax(state, maxDepth - 1, searchContext, -beta, -alpha);
            state.UndoMove(move, undo);

            if (currentScore > bestScore) {
                bestMove = move;
                bestScore = currentScore;
            }

            alpha = int.Max(alpha, bestScore.Score);
            if (alpha >= beta) break;
        }

        return new ScoredMove(bestMove, bestScore);
    }

    internal SearchScore Negamax(State state, int depth) {
        if (depth <= 0 || state.IsTerminal()) {
            var score = Eval(state);
            return state.WhiteIsActive ? score : -score;
        }

        SearchScore bestScore = new SearchScore(int.MinValue);


        var moves = new GeneratorWrapper(state).GetLegalMoves();
        moves.Sort();

        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            SearchScore currentScore = -Negamax(state, depth - 1);
            state.UndoMove(move, undo);

            if (currentScore > bestScore) {
                bestScore = currentScore;
            }
        }

        return bestScore;
    }


    // be careful with the a-b values initialization, they will overflow
    // ReSharper disable once InconsistentNaming
    internal SearchScore SmartABNegamax(State state, int depth, SearchContext searchContext, int alpha, int beta) {
        searchContext.IncrementNodeCount();
        if (searchContext.ShouldStop()) {
            return new SearchScore(alpha);
        }

        bool isMaxing = state.WhiteIsActive;

        if (depth <= 0 || state.IsTerminal()) {
            var score = Eval(state);
            return isMaxing ? score : -score;
        }


        var moves = new GeneratorWrapper(state).GetAllMoves();
        if (moves.Count == 0) return new SearchScore(0); // stalemate

        moves.Sort();

        SearchScore bestScore = new SearchScore(int.MinValue + 1);

        foreach (var move in moves) {
            // this is faster, because with pruning, the generator will skip the expensive move legality check altogether
            if (!GeneratorWrapper.CheckMoveLegality(move, state))
                continue;
            var undo = state.ApplyMove(move);
            SearchScore currentScore = -SmartABNegamax(state, depth - 1, searchContext, -beta, -alpha);
            state.UndoMove(move, undo);

            if (currentScore > bestScore) {
                bestScore = currentScore;
            }
            alpha = int.Max(alpha, bestScore.Score);
            if (alpha >= beta) break;
        }

        return bestScore;
    }

    /// <summary>
    ///     Everything relevant to all threads working on the current search.
    ///     Currently isn't thread safe.
    /// </summary>
    public class SearchContext {
        public CancellationToken CancellationToken { get; set; }

        public bool StopRequested { get; private set; }
        public SearchStats Stats { get; init; } = new();
        
        // TODO think of a way to keep it thread safe
        public int Alpha { get; set; } = int.MinValue + 1; // to prevent negation overflow
        public int Beta { get; set; } = int.MaxValue;

        public void IncrementNodeCount() {
            Interlocked.Increment(ref Stats.NodesSearched);
        }

        /// <summary>
        /// Periodically checks, if the cancellation token has been called. If so 
        /// </summary>
        /// <returns>If the search should stop ASAP.</returns>
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
}
