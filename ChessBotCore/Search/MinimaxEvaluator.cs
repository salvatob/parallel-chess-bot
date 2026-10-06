using ChessBotCore.Game;
using ChessBotCore.MoveGenerators;

namespace ChessBotCore.Search;


/// <summary>
///     A type encapsulating a Negamax-based State Space Search of the best move. It is not thread-safe.
/// </summary>
public class MinimaxEvaluator {
    private readonly ITimeManager _timeManager = new TimeManager();
    private readonly IMoveGenerator _moveGenerator = new MoveGenerator();

    /// <summary>
    /// Initializes a new instance of the <see cref="MinimaxEvaluator"/> class.
    /// </summary>
    public MinimaxEvaluator(
        ITimeManager? timeManager = null,
        IMoveGenerator? moveGenerator = null)
    {
        if (timeManager is not null)
            _timeManager = timeManager;

        if (moveGenerator is not null)
            _moveGenerator = moveGenerator;
    }
    /// <summary>
    /// Evaluates the board state and returns a score from the perspective of the player to move.
    /// </summary>
    /// <param name="s">The board state.</param>
    /// <returns>The evaluation score.</returns>
    private static SearchScore Eval(State s) {
        int score = Evaluator.Evaluate(s);
        bool isMate = score is short.MinValue or short.MaxValue;
        return new SearchScore(score, isMate);
    }

    

    /// <summary>
    /// A simple way to run the search to a specified depth.
    /// </summary>
    /// <param name="state">The state from which to search the best move</param>
    /// <param name="maxDepth">The depth to which to run the search.</param>
    /// <returns></returns>
    public SearchResults ChooseBestMove(State state, int maxDepth) {
        var searchContext = new SearchContext {
            CleanStopToken = CancellationToken.None,
            Stats = new SearchStats { MaxDepth = maxDepth }
        };
        State copy = state.Clone();
        var scoredMove = SmartABNegamaxBase(copy, maxDepth, searchContext);
        var results = new EngineSearchResults(scoredMove, searchContext.Stats);

        return results;
    }

    /// <summary>
    /// Performs an iterative deepening search to find the best move within the allotted time.
    /// </summary>
    /// <param name="state">The starting board state.</param>
    /// <param name="timers">The game timers.</param>
    /// <param name="abandonSearch">Token to abort the search by throwing <seealso cref="OperationCanceledException"/>>.</param>
    /// <returns>The search results containing the best move found.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the state has no legal moves.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the search is aborted.</exception>
    public SearchResults PrimitiveIterativeSearch(State state, Timers timers, CancellationToken abandonSearch) {
        var timePerMove = _timeManager.CalculateAllowedTime(timers, state.WhiteIsActive);
        
        using var stopSearchCleanly = new CancellationTokenSource(); 
        stopSearchCleanly.CancelAfter(timePerMove);
        
        var scoredMoves = GetInitialScoredMoves(state);
        if (scoredMoves.Count == 0) 
            throw new InvalidOperationException("Current State has no moves available, so it cannot be searched.");
        
        var lastCompletedResult = new EngineSearchResults(scoredMoves[0], new SearchStats {
            NodesSearched = (ulong)scoredMoves.Count,
            MaxDepth = 1
        });

        for (int depth = 2; depth <= 200; depth++) {
            var context = new SearchContext {
                AbortToken = abandonSearch,
                CleanStopToken = stopSearchCleanly.Token,
                Stats = new SearchStats { MaxDepth = depth }
            };
            if (context.ShouldStop()) break;

            PerformSearchIteration(state, depth, context, scoredMoves);
            
            if (!context.ShouldStop()) {
                scoredMoves.Sort((a, b) => b.Score.CompareTo(a.Score));
                
                lastCompletedResult = new EngineSearchResults(scoredMoves[0], context.Stats);
            }
        }

        return lastCompletedResult;
    }
    
    /// <summary>
    /// Scores the provided moves and updates the scores in the list.
    /// </summary>
    /// <param name="state">The current board state.</param>
    /// <param name="depth">The target depth for this iteration.</param>
    /// <param name="context">The search context.</param>
    /// <param name="scoredMoves">The list of moves to evaluate, which will be updated with new scores.</param>
    private void PerformSearchIteration(State state, int depth, SearchContext context, List<ScoredMove> scoredMoves) {
        for (int i = 0; i < scoredMoves.Count; i++) {
            if (context.ShouldStop()) break;
        
            var move = scoredMoves[i].Move;
            var undo = state.ApplyMove(move);
            SearchScore score = -SmartABNegamax(state, depth - 1, context, -context.Beta, -context.Alpha);
            
            state.UndoMove(move, undo);
            
            if (!context.ShouldStop()) {
                scoredMoves[i] = new ScoredMove(move, score);
                if (score > context.Alpha) context.Alpha = score;
            }

            // if (context.Alpha >= context.Beta) break;
        }
    }

    /// <summary>
    /// Gets the initial list of moves for the search, sorted by their heuristic priority.
    /// </summary>
    /// <param name="state">The board state.</param>
    /// <returns>A list of moves with initial zero scores.</returns>
    private List<ScoredMove> GetInitialScoredMoves(State state) {
        var moves = _moveGenerator.GenerateMoves(state).GetLegalMoves();
        moves.Sort();
        var scoredMoves = new List<ScoredMove>(moves.Count);
        foreach (var move in moves) {
            scoredMoves.Add(new ScoredMove(move, new SearchScore(0)));
        }
        return scoredMoves;
    }
    
    
    /// <summary>
    /// The entry point for a recursive Negamax search with alpha-beta pruning.
    /// </summary>
    /// <param name="state">The board state.</param>
    /// <param name="maxDepth">The maximum depth to search.</param>
    /// <param name="context">The search context.</param>
    /// <returns>The best move found and its score.</returns>
    // ReSharper disable once InconsistentNaming
    private ScoredMove SmartABNegamaxBase(State state, int maxDepth, SearchContext context) {
        context.IncrementNodeCount();
                var moves = _moveGenerator.GenerateMoves(state).GetLegalMoves();

        moves.Sort();

        SearchScore bestScore = new SearchScore(int.MinValue + 1);
        Move bestMove = default;

        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            SearchScore currentScore = -SmartABNegamax(state, maxDepth - 1, context, -context.Beta, -context.Alpha);
            state.UndoMove(move, undo);

            if (currentScore > bestScore) {
                bestMove = move;
                bestScore = currentScore;
            }
            if (bestScore > context.Alpha) context.Alpha = bestScore;

            if (context.Alpha >= context.Beta) break;
        }

        return new ScoredMove(bestMove, bestScore);
    }

    /// <summary>
    /// A standard recursive Negamax search without pruning.
    /// </summary>
    /// <param name="state">The board state.</param>
    /// <param name="depth">The remaining depth to search.</param>
    /// <returns>The score of the position.</returns>
    internal SearchScore Negamax(State state, int depth) {
        if (depth <= 0 || state.IsTerminal()) {
            var score = Eval(state);
            return state.WhiteIsActive ? score : -score;
        }

        SearchScore bestScore = new SearchScore(int.MinValue);


                var moves = _moveGenerator.GenerateMoves(state).GetLegalMoves();

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


    /// <summary>
    /// A recursive Negamax search with alpha-beta pruning.
    /// </summary>
    /// <param name="state">The board state.</param>
    /// <param name="depth">The remaining depth to search.</param>
    /// <param name="searchContext">The search context.</param>
    /// <param name="alpha">The alpha bound.</param>
    /// <param name="beta">The beta bound.</param>
    /// <returns>The score of the position.</returns>
    // be careful with the a-b values initialization, they will overflow
    // ReSharper disable once InconsistentNaming
    internal SearchScore SmartABNegamax(State state, int depth, SearchContext searchContext, SearchScore alpha, SearchScore beta) {
        searchContext.IncrementNodeCount();
        if (searchContext.ShouldStop()) {
            return alpha;
        }

        bool isMaxing = state.WhiteIsActive;

        if (depth <= 0 || state.IsTerminal()) {
            var score = Eval(state);
            return isMaxing ? score : -score;
        }


        var moves = _moveGenerator.GenerateMoves(state).GetAllMoves();

        if (moves.Count == 0) return new SearchScore(0); // stalemate

        moves.Sort();

        SearchScore bestScore = new SearchScore(int.MinValue + 1);

        foreach (var move in moves) {
            // this is faster, because with pruning, the generator will skip the expensive move legality check altogether
            if (!MoveGenerator.CheckMoveLegality(move, state))
                continue;
            var undo = state.ApplyMove(move);
            SearchScore currentScore = -SmartABNegamax(state, depth - 1, searchContext, -beta, -alpha);
            state.UndoMove(move, undo);

            if (currentScore > bestScore) {
                bestScore = currentScore;
            }
            if (bestScore > alpha) alpha = bestScore;  
            
            if (alpha >= beta) break;
        }

        return bestScore;
    }

    /// <summary>
    ///     Everything relevant to all threads working on the current search.
    ///     Currently isn't thread safe.
    /// </summary>
    public class SearchContext {
        /// <summary>
        /// Used to abort the search by throwing <seealso cref="OperationCanceledException"/> and discarding any search results.
        /// </summary>
        public CancellationToken AbortToken { get; init; }
        
        /// <summary>
        /// Used internally by the search algorithm to signal end of search, and to return the best result found.
        /// </summary>
        public required CancellationToken CleanStopToken { get; set; }

        public SearchStats Stats { get; init; } = new();

        private volatile bool _stopRequested = false;
        
        // TODO think of a way to keep it thread safe
        public SearchScore Alpha { get; set; } = new (int.MinValue + 1); // to prevent negation overflow
        public SearchScore Beta { get; set; } = new (int.MaxValue);

        public void IncrementNodeCount() {
            Interlocked.Increment(ref Stats.NodesSearched);
        }
        
        public bool ShouldStop() {
            // in future this should also handle things like if we already found mate,
            // or in case of another thread finding a dominating move (alpha beta hit).
            
            // filter the token calls to every few thousand nodes
            if ((Stats.NodesSearched & 0xFFF) != 0) {
                if (AbortToken.IsCancellationRequested) {
                    // just abort the whole search
                    throw new OperationCanceledException();
                }

                if (CleanStopToken.IsCancellationRequested) {
                    // store the intent to stop the search,
                    // so other threads see it without waiting for the node count condition 
                    _stopRequested = true;
                }
            }
            return _stopRequested;
        }
    }
}
