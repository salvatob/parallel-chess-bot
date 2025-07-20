using ChessBotCore;
using FluentAssertions;

namespace TestMoveGen;

public class PawnMovement {
    [Fact]
    public void White_OneCellForward() {
        var inputStr =
            """
            0000 1000
            0000 1000
            0010 0000
            0000 0000
            0000 0010
            0000 0100
            0111 0110
            0000 0000
            """;

        string[] expectedStr = [
            """
            0000 1000
            0010 1000
            0000 0000
            0000 0000
            0000 0010
            0000 0100
            0111 0110
            0000 0000
            """,
            """
            0000 1000
            0000 1000
            0010 0000
            0000 0010
            0000 0000
            0000 0100
            0111 0110
            0000 0000
            """,
            """
            0000 1000
            0000 1000
            0010 0000
            0000 0000
            0000 0110
            0000 0000
            0111 0110
            0000 0000
            """,
            """
            0000 1000
            0000 1000
            0010 0000
            0000 0000
            0000 0010
            0000 0110
            0111 0100
            0000 0000
            """,
            """
            0000 1000
            0000 1000
            0010 0000
            0000 0000
            0000 0010
            0100 0100
            0011 0110
            0000 0000
            """,
            """
            0000 1000
            0000 1000
            0010 0000
            0000 0000
            0000 0010
            0010 0100
            0101 0110
            0000 0000
            """,
            """
            0000 1000
            0000 1000
            0010 0000
            0000 0000
            0000 0010
            0001 0100
            0110 0110
            0000 0000
            """
        ];

        
        var pawns = Bitboard.Parse(inputStr);

        var possibleMoves = PawnMoveGenerator.OneCellForward(pawns, ~pawns);

        var expectedMoves = expectedStr.Select(Bitboard.Parse);

        possibleMoves.Should().BeEquivalentTo(expectedMoves);
    }

    [Fact]
    public void White_DoubleMove() {
        //arrange
        var pawnsStr = 
            """
            0000 0000
            0000 0000
            0000 0000
            0000 0000
            0000 0000
            1000 1000
            0100 1010
            0000 0000
            """;
        string[] expectedStr = [
            """
            0000 0000
            0000 0000
            0000 0000
            0000 0000
            0100 0000
            1000 1000
            0000 1010
            0000 0000
            """
        ];
        string otherPiecesStr = 
            """
            0000 0000
            0000 0000
            0000 0000
            0000 0000
            0000 0000
            0000 0010
            0000 0000
            0000 0000
            """;
        //act

        var pawns = Bitboard.Parse(pawnsStr);
        var otherPieces = Bitboard.Parse(otherPiecesStr) | pawns;
        var expected = expectedStr.Select(Bitboard.Parse);
        
        var pawnDoubleMovesOnly = PawnMoveGenerator.DoubleMoveForward(pawns, ~otherPieces);
        //assert

        pawnDoubleMovesOnly.Should().BeEquivalentTo(expected);

    }
}