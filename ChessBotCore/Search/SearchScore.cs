namespace ChessBotCore.Search;

/// <summary>
/// Encapsulates the score of a move, along with info,
/// like if the score was a checkmate.  
/// </summary>
public readonly record struct SearchScore : IComparable<SearchScore> {
    /// <summary> The evaluation score. </summary>
    public int Score { get; }
    /// <summary> True if the score represents a checkmate. </summary>
    public bool IsMate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchScore"/> struct.
    /// </summary>
    /// <param name="score">The evaluation score.</param>
    /// <param name="isMate">Whether it's a checkmate score.</param>
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
