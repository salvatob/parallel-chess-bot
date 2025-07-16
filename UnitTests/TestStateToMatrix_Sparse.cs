using ChessBotCore;

namespace TestProject1;

/// <summary>
/// Tests transformation between a string (FEN), and internal game state  representation.
/// </summary>
public class TestStateToMatrix_Sparse {
    static string WhiteQueenA1FenPrefix = "8/8/8/8/8/8/8/Q7";
    static State WhiteQueenA1State = State.Initial() with { WhiteQueens = 0b1000_0000 };
    static Dictionary<(int row, int col), char> AllBoardWhiteQueens = [];

    static TestStateToMatrix_Sparse() {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                AllBoardWhiteQueens[(i, j)] = 'Q';
            }
        }
    } 

    
    public class StateToMatrixDataType {
        public string? Name { get; init; }
        public required State State { get; init; }
        public required Dictionary<(int row, int col), char> MatrixCreatorDict { get; init; }
        public override string ToString() {
            return Name ?? "xddx";
        }
    }
    
    public static IEnumerable<object[]> StateToMatrix_Data => new[] {
        new StateToMatrixDataType {
            Name = "WhiteQueenA1",
            State = State.Empty() with { WhiteQueens = 0b1000_0000 },
            MatrixCreatorDict = new() {[(0,0)] = 'Q'}
        },
        
        new StateToMatrixDataType {
            Name = nameof(AllBoardWhiteQueens),
            State = State.Empty() with {WhiteQueens = 0xFFFF_FFFF_FFFF_FFFF},
            MatrixCreatorDict = AllBoardWhiteQueens
        },
        new StateToMatrixDataType {
            Name = "BlackRooksInMiddle",
            State = State.Empty() with{
                BlackRooks = (ulong) 0b_0001_1000_0001_1000 << (8*3)
            },
            MatrixCreatorDict = new() {
                [(3,3)] = 'r',
                [(3,4)] = 'r',
                [(4,3)] = 'r',
                [(4,4)] = 'r'
            }
        }
        
    }.Select(i=> new object[] {i});
    
    
    [Theory]
    // [InlineData(WhiteQueenA1State)]
    [MemberData(nameof(StateToMatrix_Data))]
    
    public void StateToMatrix_SparseBoard(StateToMatrixDataType data) {
        //arrange
        
        //act
        // var state = WhiteQueenA1State;
        var expected = new char[8 * 8];
        foreach (var ((r,c),val) in data.MatrixCreatorDict) {
            expected[r * 8 + c] = val;
        }

        char[,] actual = FenCreator.EncodeIntoMatrix(data.State);
        var flat = actual.Flatten();

        //assert
        Assert.Equal(expected, flat);
    }
}