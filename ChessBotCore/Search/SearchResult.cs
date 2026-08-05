using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Search;

public class SearchResults {
    public required Move BestMove { get; init; }
        
    public SearchResults() {}
    
    public SearchResults(Move bestMove) {
        BestMove = bestMove;
    }
    
}

/// <summary>
/// Statistics relevant to one search operation.
/// </summary>
public class SearchStats {
    public ulong NodesSearched;
    public IEnumerable<Move>? PrincipalVariation;
    public SearchScore Score;
    /// <summary>
    /// Represents the depth to which the search has been evaluated.
    /// </summary>
    public int MaxDepth { get; init; }
}

/// <summary>
/// The result of the whole search operation <seealso cref="MinimaxEvaluator"/>.
/// Contains the best move found, optionally some other metadata.
/// </summary>
public class EngineSearchResults :  SearchResults {
    public required SearchStats Stats {get; init; }
    
    public SearchScore? Score { get; init; }
    
    /// <summary>
    /// Represents the depth to which the search has been evaluated.
    /// </summary>
    public int MaxDepth { get; init; }
    
    public EngineSearchResults() { }
    
    public EngineSearchResults(Move bestMove) : base(bestMove) { }
    
    [SetsRequiredMembers]
    public EngineSearchResults(ScoredMove sm, SearchStats stats) :  base(sm.Move) {
        BestMove = sm.Move;
        Stats = stats;
        Stats.Score = sm.Score;
    }

    public override string ToString() {
        if (Stats is null) return BestMove.ToString();
        return $"{BestMove} ... score - ({Stats.Score}), depth {Stats.MaxDepth}, {Stats.NodesSearched} nodes searched";
    }
}

public readonly record struct ScoredMove(Move Move, SearchScore Score);
