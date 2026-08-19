using ChessBotCore;
using ChessBotCore.Game;
using Xunit;

namespace UnitTests;

public class GameOutcomeTests {
    [Fact]
    public void InitialState_IsNonTerminal() {
        var state = State.Initial;
        Assert.Equal(GameOutcome.NonTerminal, state.GetOutcome());
    }

    [Fact]
    public void Checkmate_BlackWins() {
        // Fool's mate: 1. f3 e5 2. g4 Qh4#
        var state = State.FromFen("rnb1kbnr/pppp1ppp/8/4p3/6Pq/5P2/PPPPP2P/RNBQKBNR w KQkq - 1 3");
        Assert.Equal(GameOutcome.BlackWin, state.GetOutcome());
    }

    [Fact]
    public void Checkmate_WhiteWins() {
        // Scholar's mate: 1. e4 e5 2. Bc4 Nc6 3. Qh5 Nf6 4. Qxf7#
        var state = State.FromFen("r1bqkb1r/pppp1Qpp/2n2n2/4p3/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 4");
        Assert.Equal(GameOutcome.WhiteWin, state.GetOutcome());
    }

    [Fact]
    public void Stalemate_IsDraw() {
        var state = State.FromFen("k7/8/1Q6/8/8/8/8/K7 b - - 0 1");
        Assert.Equal(GameOutcome.Draw, state.GetOutcome());
    }

    [Fact]
    public void InsufficientMaterial_KK_IsDraw() {
        var state = State.FromFen("k7/8/8/8/8/8/8/K7 w - - 0 1");
        Assert.Equal(GameOutcome.Draw, state.GetOutcome());
    }

    [Fact]
    public void InsufficientMaterial_KNK_IsDraw() {
        var state = State.FromFen("k7/8/8/8/8/8/8/K1N5 w - - 0 1");
        Assert.Equal(GameOutcome.Draw, state.GetOutcome());
    }

    [Fact]
    public void FiftyMoveRule_IsDraw() {
        var state = State.Initial;
        // Manually set half moves to 100 (50 full moves)
        state.HalfMovesSincePawnMoveOrCapture = 100;
        Assert.Equal(GameOutcome.Draw, state.GetOutcome());
    }
}
