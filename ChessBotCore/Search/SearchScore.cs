using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Search;


/// <summary>
/// Encapsulates the score of a move, along with info,
/// like if the score was a checkmate.  
/// </summary>
public readonly record struct SearchScore : IComparable<SearchScore> {
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

/// <summary>
/// Statistics relevant to one search operation.
/// </summary>
public class SearchStats {
    public ulong NodesSearched;
}

/// <summary>
/// The result of the whole search operation <seealso cref="MinimaxEvaluator"/>.
/// Contains the best move found, optionally the principal variation,
/// as well as some more metadata useful for debugging end benchmarking.
/// </summary>
public class SearchResults {
    public required Move BestMove;
    public required SearchScore Score;
    public IEnumerable<Move>? PrincipalVariation;
    public required SearchStats Stats;
    /// <summary>
    /// Represents the depth to which the search has been evaluated.
    /// </summary>
    public required int MaxDepth { get; init; }
    
    public SearchResults() {}
    
    [SetsRequiredMembers]
    public SearchResults(ScoredMove sm, SearchStats stats) {
        BestMove = sm.Move;
        Score = sm.Score;
        Stats = stats;
    }
    [SetsRequiredMembers]
    public SearchResults(Move bestMove, SearchScore score, SearchStats stats) {
        BestMove = bestMove;
        Score = score;
        Stats = stats;
    }

    public override string ToString() {
        return $"{BestMove} ({Score}) depth {MaxDepth}, {Stats.NodesSearched} nodes searched";
    }
}

public readonly record struct ScoredMove(Move Move, SearchScore Score);
