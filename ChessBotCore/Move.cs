namespace ChessBotCore;

public struct Move {
    public  State StateAfter { get; set; }
    public required bool IsCapture { get; init; }
    public bool IsPromotion { get; init; }
    public bool IsCheck { get; init; }
    public bool IsCastle { get; init; }
    
    // public string strRepresentation =
    public Coordinates coordsBefore { get; set; }
    public Coordinates coordsAfter { get; set; }
    
    
    public Move(State stateAfter) {
        StateAfter = stateAfter;
    }

    public static string? TryGetNotation(State before, State after) {
        return FenCreator.TryGetMoveNotation(before, after);
    }
    
    public readonly string? TryGetNotation(State before) {
        return FenCreator.TryGetMoveNotation(before, StateAfter);
    }
}