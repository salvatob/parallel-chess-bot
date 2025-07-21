using ChessBotCore;
using FluentAssertions;
using Xunit.Abstractions;

namespace TestProject1;

public class MovingPieces {
    private readonly ITestOutputHelper _testOutputHelper;

    public MovingPieces(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;
    }

    public static IEnumerable<object[]> MovePieces_Data => new[] { [
            Direction.E,
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
            0000 0001
            0000 0000
            0000 0000
            0100 0000
            0000 0000
            0000 0000
            0000 0000
            0100 1000
            """
        ],
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
        }, [
            Direction.N,
            """
            1111 1111
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1111 1111

            """,
            """
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1111 1111
            0000 0000
            """
        ], [
            Direction.S,
            """
            1111 1111
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1111 1111
            """,
            """
            0000 0000
            1111 1111
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            """
        ], [
            Direction.E,
            """
            1111 1111
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1111 1111
            """,
            """
            0111 1111
            0100 0000
            0100 0000
            0100 0000
            0100 0000
            0100 0000
            0100 0000
            0111 1111
            """
        ], [
            Direction.W,
            """
            1111 1111
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1111 1111
            """,
            """
            1111 1110
            0000 0010
            0000 0010
            0000 0010
            0000 0010
            0000 0010
            0000 0010
            1111 1110
            """
        ], [
            Direction.NE,
            """
            1111 1111
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1111 1111
            """,
            """
            0100 0000
            0100 0000
            0100 0000
            0100 0000
            0100 0000
            0100 0000
            0111 1111
            0000 0000
            """
        ], [
            Direction.NW,
            """
            1111 1111
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1111 1111
            """,
            """
            0000 0010
            0000 0010
            0000 0010
            0000 0010
            0000 0010
            0000 0010
            1111 1110
            0000 0000
            """
        ], [
            Direction.SE,
            """
            1111 1111
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1111 1111
            """,
            """
            0000 0000
            0111 1111
            0100 0000
            0100 0000
            0100 0000
            0100 0000
            0100 0000
            0100 0000
            """
        ], [
            Direction.SW,
            """
            1111 1111
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1000 0001
            1111 1111
            """,
            """
            0000 0000
            1111 1110
            0000 0010
            0000 0010
            0000 0010
            0000 0010
            0000 0010
            0000 0010
            """
        ], 

    };
    
    [Theory]
    [MemberData(nameof(MovePieces_Data))]
    public void MovePiecesInBitboard(Direction dir, string before, string after) {
        //arrange
        var beforeBoard = Bitboard.Parse(before);

        var expected = Bitboard.Parse(after);

        //act

        var movedBoard = beforeBoard.MovePieces(dir);
        //assert
        
        _testOutputHelper.WriteLine("expected");
        _testOutputHelper.WriteLine(expected.PrettyPrint());
        _testOutputHelper.WriteLine("movedBoard");
        _testOutputHelper.WriteLine(movedBoard.PrettyPrint());
        
        movedBoard.Should().BeEquivalentTo(expected);
    }
}