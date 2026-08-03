using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ChessBotCore.Game;

namespace ChessBotCore.Search;

public class SearchStats {
    public ulong NodesSearched;
}

public readonly record struct SearchScore : IEquatable<SearchScore>, IComparable<SearchScore> {
    public int Score { get; }
    public bool IsMate { get; }

    public SearchScore(int score, bool isMate = false) {
        Score = score;
        IsMate = isMate;
    }

    public static SearchScore Negate(SearchScore s) => new(-s.Score, s.IsMate);
    public static SearchScore operator -(SearchScore s) => Negate(s);

    public static bool operator >(SearchScore a, SearchScore b) {
        if (a.IsMate && !b.IsMate) return a.Score > 0;
        if (!a.IsMate && b.IsMate) return b.Score < 0;
        return a.Score > b.Score;
    }

    public static bool operator <(SearchScore a, SearchScore b) => b > a;
    public static bool operator >=(SearchScore a, SearchScore b) => a > b || (a.Score == b.Score && a.IsMate == b.IsMate);
    public static bool operator <=(SearchScore a, SearchScore b) => a < b || (a.Score == b.Score && a.IsMate == b.IsMate);


    public bool Equals(SearchScore other) => Score == other.Score && IsMate == other.IsMate;
    public override int GetHashCode() => HashCode.Combine(Score, IsMate);

    public int CompareTo(SearchScore other) {
        if (this < other) return -1;
        if (this > other) return 1;
        return 0;
    }

    public override string ToString() => IsMate ? $"Mate ({Score})" : Score.ToString();
}

public struct SearchResults {
    public required Move BestMove;
    public int Score;
    public Move? PrincipalVariation;
    public SearchStats? Stats;

    [SetsRequiredMembers]
    public SearchResults(Move bestMove, int score = 0) {
        BestMove = bestMove;
        Score = score;
    }

    public override string ToString() {
        if (Stats is null) return $"{BestMove} ({Score})";
        return $"{BestMove} ({Score}), {Stats?.NodesSearched} nodes searched";
    }
}

internal struct ScoredMove {
    public Move Move;
    public int Score;

    public ScoredMove(Move move, int score) {
        Move = move;
        Score = score;
    }
}

/// <summary>
///     A type encapsulating a Negamax-based State Space Search of the best move. It is not thread-safe.
/// </summary>
public class MinimaxEvaluator {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Eval(State s) {
        return Evaluator.Evaluate(s);
    }

    private TimeSpan CalculateAllowedTime(Timers timers, bool isWhite) {
        var timeLeft = timers.ActiveTime(isWhite);
        var timePerMoveFraction = timeLeft / 20 + timers.Increment / 2;
        return new[] { timePerMoveFraction, timeLeft / 2 }.Min(); // never should use more than half of remaining time
    }

    public SearchResults ChooseBestMove(State state, int maxDepth, SearchContext searchContext) {
        State copy = state.Clone();
        var (bestMove, bestScore) = NegamaxBase(copy, maxDepth, searchContext, searchContext.Alpha, searchContext.Beta);
        var results = new SearchResults {
            BestMove = bestMove,
            Score = bestScore,
            Stats = searchContext.Stats
        };

        return results;
    }

    public SearchResults PrimitiveIterativeSearch(State state, Timers timers, CancellationToken cancellationToken) {
        var timePerMove = CalculateAllowedTime(timers, state.WhiteIsActive);
        var context = new SearchContext {
            CancellationToken = cancellationToken
        };

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timePerMove);
        context.CancellationToken = cts.Token;

        var scoredMoves = GetInitialScoredMoves(state);
        if (scoredMoves.Count == 0) return new SearchResults(default, 0);

        SearchResults lastCompletedResult = new(scoredMoves[0].Move, scoredMoves[0].Score);

        for (int depth = 2; depth <= 100; depth++) {
            if (context.ShouldStop()) break;

            PerformSearchIteration(state, depth, context, scoredMoves);
            
            scoredMoves.Sort((a, b) => b.Score.CompareTo(a.Score));
            lastCompletedResult = new SearchResults(scoredMoves[0].Move, scoredMoves[0].Score) {
                Stats = context.Stats
            };

        }

        return lastCompletedResult;
    }

    private List<ScoredMove> GetInitialScoredMoves(State state) {
        var moves = new GeneratorWrapper(state).GetLegalMoves();
        moves.Order();
        var scoredMoves = new List<ScoredMove>(moves.Count);
        foreach (var move in moves) {
            scoredMoves.Add(new ScoredMove(move, 0));
        }
        return scoredMoves;
    }

    private void PerformSearchIteration(State state, int depth, SearchContext context, List<ScoredMove> scoredMoves) {
        for (int i = 0; i < scoredMoves.Count; i++) {
            if (context.ShouldStop()) break;

            var move = scoredMoves[i].Move;
            var undo = state.ApplyMove(move);
            int score = -SmartABNegamax(state, depth - 1, context, -context.Beta, -context.Alpha);
            state.UndoMove(move, undo);

            if (!context.StopRequested) {
                scoredMoves[i] = new ScoredMove(move, score);
            }
        }
    }

    private (Move BestMove, int BestScore) NegamaxBase(State state, int maxDepth, SearchContext searchContext, int alpha,
        int beta) {
        var moves = new GeneratorWrapper(state).GetLegalMoves();
        moves.Sort();

        int bestScore = int.MinValue + 1;
        Move bestMove = default;

        foreach (var move in moves) {
            var undo = state.ApplyMove(move);
            int currentScore = -SmartABNegamax(state, maxDepth - 1, searchContext, -beta, -alpha);
            state.UndoMove(move, undo);

            if (currentScore > bestScore) {
                bestMove = move;
                bestScore = currentScore;
            }

            alpha = int.Max(alpha, bestScore);
            if (alpha >= beta) break;
        }

        return (bestMove, bestScore);
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
            int currentScore = -Negamax(state, depth - 1);
            state.UndoMove(move, undo);

            bestScore = int.Max(bestScore, currentScore);
        }

        return bestScore;
    }

    // be careful with the a-b values initialization that will overflow
    // ReSharper disable once InconsistentNaming
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
            int currentScore = -ABNegamax(state, depth - 1, -beta, -alpha);
            state.UndoMove(move, undo);

            bestScore = int.Max(bestScore, currentScore);
            alpha = int.Max(alpha, bestScore);
            if (alpha >= beta) break;
        }

        return bestScore;
    }

    // be careful with the a-b values initialization, they will overflow
    // ReSharper disable once InconsistentNaming
    internal int SmartABNegamax(State state, int depth, SearchContext searchContext, int alpha, int beta) {
        if (searchContext.ShouldStop()) {
            return alpha;
        }

        bool isMaxing = state.WhiteIsActive;

        if (depth <= 0 || state.IsTerminal()) {
            var score = Eval(state);
            return isMaxing ? score : -score;
        }


        var moves = new GeneratorWrapper(state).GetAllMoves();
        if (moves.Count == 0) return 0; // stalemate

        moves.Sort();

        int bestScore = int.MinValue + 1;

        foreach (var move in moves) {
            // this is faster, because with pruning, the generator will skip the expensive move legality check altogether
            if (!GeneratorWrapper.CheckMoveLegality(move, state))
                continue;
            var undo = state.ApplyMove(move);
            int currentScore = -SmartABNegamax(state, depth - 1, searchContext, -beta, -alpha);
            state.UndoMove(move, undo);

            bestScore = int.Max(bestScore, currentScore);
            alpha = int.Max(alpha, bestScore);
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
        public SearchStats Stats { get; } = new();
        // TODO think of a way to keep it thread safe
        public int Alpha { get; set; } = int.MinValue + 1; // to prevent negation overflow
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
}
