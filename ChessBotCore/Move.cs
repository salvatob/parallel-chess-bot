namespace ChessBotCore;

public struct Move {
    [Flags]
    private enum MoveInit : byte {
        None     = 0,
        FromSet  = 1 << 0,
        ToSet    = 1 << 1,
        FlagSet  = 1 << 2,
        PromoSet = 1 << 3,
        All      = FromSet | ToSet | FlagSet | PromoSet
    }

    public  State StateAfter {
        get;
            
        set ;
    }
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