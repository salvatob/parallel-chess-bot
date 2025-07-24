using ChessBotCore;

namespace TestProject1;

public class TestCoordinates {
    [Theory]
    [InlineData(0, 0, "a1")]
    [InlineData(1, 0, "a2")]
    [InlineData(0, 1, "b1")]
    [InlineData(1, 1, "b2")]
    [InlineData(4, 6, "g5")]
    [InlineData(6, 7, "h7")]
    [InlineData(7, 6, "g8")]
    [InlineData(7, 7, "h8")]
    public void Coordinates_ToString(int i, int j, string expected) {
        var coords = new Coordinates { Row = i, Col = j };
        string actual = coords.ToString();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("a1", 0, 0)]
    [InlineData("a2", 1, 0)]
    [InlineData("b1", 0, 1)]
    [InlineData("b2", 1, 1)]
    [InlineData("g5", 4, 6)]
    [InlineData("h7", 6, 7)]
    [InlineData("g8", 7, 6)]
    [InlineData("h8", 7, 7)]
    public void Coordinates_FromString(string actual, int i, int j) {
        Coordinates coords = Coordinates.FromString(actual);

        Assert.Equal(i, coords.Row);
        Assert.Equal(j, coords.Col);
    }

    [Fact]
    public void Coordinates_FromMask() {
        //arrange
        var mask = Bitboard.FromCoords(Coordinates.FromString("b3"));
        var expected = new Coordinates(2, 1);
        //act
        var actual = Coordinates.FromMask(mask);
        //assert
        Assert.Equal(expected, actual);
        
    }
}