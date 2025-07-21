namespace ChessBotCore;

public struct Move {
    public  State StateAfter { get; }
    private byte _info = 0;
    // bit 0 is for capture
    // bit 1 is for promotion
    // bit 2 is for check

    
    public Move(State stateAfter) {
        StateAfter = stateAfter;
    }
}