using System.Reflection;
using ChessBotCore;

namespace TestProject1;

public class CompileTimeTests {
    [Theory]
    [InlineData(typeof(RookMoveGenerator))]
    [InlineData(typeof(QueenMoveGenerator))]
    [InlineData(typeof(PawnMoveGenerator))]
    [InlineData(typeof(BishopMoveGenerator))]
    [InlineData(typeof(KingMoveGenerator))]
    [InlineData(typeof(KnightMoveGenerator))]
    public void BitboardMoveGenerator_HasNoPublicConstructors(Type moveGenerator) {
        var publicCtors = moveGenerator.GetConstructors(
            BindingFlags.Public | BindingFlags.Instance);

        Assert.Empty(publicCtors);
    }
}