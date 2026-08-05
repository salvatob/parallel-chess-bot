using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Search;

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
