using ChessBotCore;
using FluentAssertions;

namespace TestProject1;

public class MovingPieces {
    
    public static IEnumerable<object[]> MovePieces_Data => new[] {
        new object[] {
            Direction.NE,
            """
            0000 0010
            0000 0000
            0000 0000
            1000 0000
            0000 0000
            0000 0000
            0000 0001
            1001 0000
            """,
            """
            0000 0000
            0000 0000
            0100 0000
            0000 0000
            0000 0000
            0000 0000
            0100 1000
            0000 0000
            """
        },

    }
    // .Select(i=> new object[]{i})
    ;
    
    [Theory]
    [MemberData(nameof(MovePieces_Data))]
    public void MovePiecesInBitboard(Direction dir, string before, string after) {
        //arrange
        var beforeBoard = Bitboard.Parse(before);

        var expected = Bitboard.Parse(after);

        //act

        var movedBoard = beforeBoard.MovePieces(dir);
        //assert

        movedBoard.Should().BeEquivalentTo(expected);
    }
}