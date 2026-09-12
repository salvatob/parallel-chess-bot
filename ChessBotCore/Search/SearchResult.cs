using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Search;


/// <summary>
/// The outer class representing the result of an <seealso cref="IPlayer"/>'s search.
/// <remarks>An engine can return the more specific <seealso cref="EngineSearchResults"/> type
/// that contains much more information.</remarks>
/// </summary>
public class SearchResults {
    /// <summary> The best move found during the search. </summary>
    public required Move BestMove { get; init; }
        
    /// <summary> Initializes a new instance of the <see cref="SearchResults"/> class. </summary>
    public SearchResults() {}
    
    /// <summary> Initializes a new instance of the <see cref="SearchResults"/> class with the best move. </summary>
    /// <param name="bestMove">The best move found.</param>
    public SearchResults(Move bestMove) {
        BestMove = bestMove;
    }
    
}

/// <summary>
/// Statistics relevant to one search operation.
/// </summary>
public class SearchStats {
    /// <summary> The number of nodes searched. </summary>
    public ulong NodesSearched;
    /// <summary> The principal variation found (sequence of best moves). </summary>
    public IEnumerable<Move>? PrincipalVariation;
    /// <summary> The evaluation score of the principal variation. </summary>
    public SearchScore Score;
    /// <summary>
    /// Represents the depth to which the search has been evaluated.
    /// </summary>
    public int MaxDepth;
}

/// <summary>
/// The result of the whole search operation <seealso cref="MinimaxEvaluator"/>.
/// Contains the best move found, optionally some other metadata.
/// </summary>
public class EngineSearchResults :  SearchResults {
    /// <summary> Detailed search statistics. </summary>
    public required SearchStats Stats {get; init; }
    
    /// <summary> Initializes a new instance of the <see cref="EngineSearchResults"/> class. </summary>
    public EngineSearchResults() { }
    
    /// <summary> Initializes a new instance of the <see cref="EngineSearchResults"/> class with a best move. </summary>
    public EngineSearchResults(Move bestMove) : base(bestMove) { }
    
    /// <summary> Initializes a new instance of the <see cref="EngineSearchResults"/> class with a scored move and stats. </summary>
    [SetsRequiredMembers]
    public EngineSearchResults(ScoredMove sm, SearchStats stats) :  base(sm.Move) {
        BestMove = sm.Move;
        Stats = stats;
        Stats.Score = sm.Score;
    }

    public override string ToString() {
        return $"{BestMove} ... score - ({Stats.Score}), depth {Stats.MaxDepth}, {Stats.NodesSearched} nodes searched";
    }
}

/// <summary>
/// Represents a move along with its evaluation score.
/// </summary>
/// <param name="Move">The chess move.</param>
/// <param name="Score">The evaluation score of the move.</param>
public readonly record struct ScoredMove(Move Move, SearchScore Score);
