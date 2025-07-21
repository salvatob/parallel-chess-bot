namespace ChessBotCore;

public struct Move {
    public  State StateAfter { get; }
    public required bool IsCapture { get; init; }
    public bool IsPromotion { get; init; }
    public bool IsCheck { get; init; }

    
    public Move(State stateAfter) {
        StateAfter = stateAfter;
    }
}