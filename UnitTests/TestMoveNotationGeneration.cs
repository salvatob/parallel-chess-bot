using ChessBotCore;
using Xunit.Abstractions;

namespace TestProject1;

public class TestMoveNotationGeneration {
  
  private readonly ITestOutputHelper _output;

  public TestMoveNotationGeneration(ITestOutputHelper output) {
    _output = output;
  }
  
  
  [Fact]
  public void Simple_1() {
    string expected = "e2e4";
    var before = State.Initial;
    
    var wPawnsAfter1 =
      before.WhitePawns &
      ~Bitboard.FromCoords("e2");
      
    var wPawnsAfter =
      wPawnsAfter1
      | Bitboard.FromCoords("e4");

    
    _output.WriteLine(before.WhitePawns.PrettyPrint());
    _output.WriteLine(wPawnsAfter1.PrettyPrint());
    _output.WriteLine(wPawnsAfter.PrettyPrint());
    
    var after = before.Next() with { WhitePawns = wPawnsAfter };

    var notation = Move.TryGetNotation(before, after);
    
    Assert.Equivalent(expected, notation);

  }
  
  
}