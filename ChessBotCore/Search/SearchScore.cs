using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Search;

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

public class SearchStats {
    public ulong NodesSearched;
}

public struct SearchResults {
    public required Move BestMove;
    public SearchScore Score;
    public Move? PrincipalVariation;
    public SearchStats? Stats;

    [SetsRequiredMembers]
    public SearchResults(Move bestMove, SearchScore score = default) {
        BestMove = bestMove;
        Score = score;
    }

    public override string ToString() {
        if (Stats is null) return $"{BestMove} ({Score})";
        return $"{BestMove} ({Score}), {Stats?.NodesSearched} nodes searched";
    }
}

internal readonly record struct ScoredMove(Move Move, SearchScore Score);

