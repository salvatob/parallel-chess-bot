using ChessBotCore;

namespace TestProject1;

public class StateToMatrix_Dense {
    private readonly char[] _emptyRow = new char[8] { default,default,default,default,default,default,default,default};
    public IEnumerable<object[]> StateToMatrixDense_Data => new[] {
        new object []{State.Initial, new char[,] {
            // _emptyRow
        }}
    };
    
    // [Theory]
    // [MemberData()]
    
    // public void StateToMatrix_DenseBoard(State state, char[,] expectedBoard) {
    //     //arrange
    //     
    //     //act
    //     // var state = WhiteQueenA1State;
    //     var expected = new char[8 * 8];
    //     foreach (var ((r,c),val) in data.MatrixCreatorDict) {
    //         expected[r * 8 + c] = val;
    //     }
    //
    //     char[,] actual = FenCreator.EncodeIntoMatrix(data.State);
    //     var flat = actual.Flatten();
    //
    //     //assert
    //     Assert.Equal(expected, flat);
    // }
}