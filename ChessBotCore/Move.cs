namespace ChessBotCore;

public struct Move {
    public  State StateAfter { get; set; }
    public required bool IsCapture { get; init; }
    public bool IsPromotion { get; init; }
    public bool IsCheck { get; init; }
    // public string strRepresentation =
    public Coordinates coordsBefore { get; set; }
    public Coordinates coordsAfter { get; set; }
    
    
    public Move(State stateAfter) {
        StateAfter = stateAfter;
    }
}