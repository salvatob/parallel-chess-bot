using ChessBotCore;

namespace TestProject1;

public class TestStateToFen {
    public class StateToFenDataType {
        public string? Name { get; init; }
        public State State { get; init; }
        public string Fen { get; init; }
        public override string ToString() {
            return Name ?? "";
        }
    }
    
    public static IEnumerable<object[]> StateToFenPieces_Data => new[] {
        new StateToFenDataType() {
            Name = "DefaultBeginningState",
            State = State.Initial,
            Fen = State.DefaultFen
        }
    }.Select(i=> new object[]{i});
    
    [Theory]
    [MemberData(nameof(StateToFenPieces_Data))]
    public void TestStateToFenPieces(StateToFenDataType stateData) {
        // arrange
        var expected = stateData.Fen;
        // act
        var actual = FenCreator.GetFen(stateData.State);

        // assert
        Assert.StartsWith(actual, expected);
    }
    
}