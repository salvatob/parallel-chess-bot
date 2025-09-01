using ChessBotCore;

namespace TestProject1;

public class FenToFen {
    [Theory]
    [InlineData(State.DefaultFen)]
    [InlineData("8/4k3/8/8/8/8/r6r/R3K2R w KQ - 0 1")]
    [InlineData("8/4k3/8/8/8/8/r6r/R3K2R w Q - 0 1")]
    [InlineData("r3r1k1/pp3pbp/1qp1b1p1/2B5/2BP4/Q1n2N2/P4PPP/3R1K1R w - - 4 18")]
    [InlineData("r3r1k1/pp2Bpbp/1qp1b1p1/8/2BP4/Q1n2N2/P4PPP/3R1K1R b - - 5 18")]
    [InlineData("8/8/4k3/8/2pPp3/8/B7/7K b - d3 0 1")]
    [InlineData("7k/8/8/8/pPp5/8/8/7K b - b3 0 1")]
    [InlineData("NBQKRBRN/PPPPPPPP/8/8/8/8/3k4/8 w - - 0 1")]
    [InlineData("8/ppp3p1/8/8/3p2Q1/8/1ppp2K1/brk4n b - - 12 7")]
    [InlineData("R6R/3Q4/1Q4Q1/4Q3/2Q4Q/Q4Q2/pp1Q4/kBNN1KB1 w - - 0 1")]
    [InlineData("B2k4/1B6/2B5/3B4/4B3/1B3B2/5KB1/6BB w - - 0 1")]
    [InlineData("nnnnnnnn/8/8/8/P2P1k2/8/6P1/n1K4n b - - 0 1")]
    public void CreateStateFromFenThenFenFromState(string initialFen) {
        //arrange

        //act
        State parsedState = FenParser.ParseFen(initialFen);
        string returnedFen = FenCreator.GetFen(parsedState);

        //assert
        Assert.Equal(initialFen, returnedFen);
    }
}