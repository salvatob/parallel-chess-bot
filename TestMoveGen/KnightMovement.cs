using ChessBotCore;

namespace TestMoveGen;

public class KnightMovement {
    [Fact]
    public void Test1() {
        //arrange
        var before = 
            """
            0000 0000
            0000 0000
            0000 0000
            0000 0000
            0001 0000
            0000 0000
            0000 0000
            0000 0000
            """;
        var after = 
            """
            0000 0000
            0000 0000
            0000 1000
            0000 0000
            0000 0000
            0000 0000
            0000 0000
            0000 0000
            """;
        //act
        var expected = Bitboard.Parse(after);

        

        //assert
    }
}