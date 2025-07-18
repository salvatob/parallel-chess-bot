namespace ChessBotCore;

public class BitBoardHelpers {
    public static ulong OneBitMask(int index) {
        return 1UL << index;
    }
    
    public static ulong OneBitMask(Coordinates coords) {
        return 1UL << coords.To1D();
    }
    
}