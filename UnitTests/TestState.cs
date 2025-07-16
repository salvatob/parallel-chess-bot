using ChessBotCore;

namespace TestProject1;

/// <summary>
/// Tests transformation between a string (FEN), and internal game state  representation.
/// </summary>
public class TestState {
    static string WhiteQueenA1FenPrefix = "8/8/8/8/8/8/8/Q7";
    static State WhiteQueenA1State = State.Initial() with { WhiteQueens = 0b1000_0000 };
    static Dictionary<(int row, int col), char> AllBoardWhiteQueens = [];

    static TestState() {
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
    }
    
    public static IEnumerable<object> StateToMatrix_Data => new[] {
        new StateToMatrixDataType {
            Name = "WhiteQueenA1",
            State = State.Initial() with { WhiteQueens = 0b1000_0000 },
            MatrixCreatorDict = new() {[(0,0)] = 'Q'}
        },
        
        new StateToMatrixDataType {
            Name = "WhiteQueensAllBoard",
            State = State.Initial() with {WhiteQueens = 0xFFFF_FFFF_FFFF_FFFF},
            MatrixCreatorDict = AllBoardWhiteQueens
        }
        
    };
    
    
    [Theory]
    // [InlineData(WhiteQueenA1State)]
    [MemberData(nameof(StateToMatrix_Data))]
    public void StateToMatrix_OneQueenA1(StateToMatrixDataType data) {
        //arrange
        Dictionary<(int row, int col), char> nondefaultValues = new() {
            [(0,0)] = 'Q'
        };

        //act
        // var state = WhiteQueenA1State;
        var expected = new char[8 * 8];
        foreach (var ((r,c),val) in nondefaultValues) {
            expected[r * 8 + c] = val;
        }

        char[,] actual = FenCreator.EncodeIntoMatrix(state);
        var flat = actual.Flatten();

        //assert
        Assert.Equal(expected, flat);
    }
    
    
    [Fact]
    public void PrintingStatePiecesOnly_OneQueen() {
        // arrange
        var state = State.Initial() with { WhiteQueens = 0b1000_0000 };
        var expected = "8/8/8/8/8/8/8/Q7";
        
        // act
        // var actual = FenCreator.GetFen();

        // assert
    }
    
    
    
    [Fact]
    public void PrintingStatePiecesOnly_DefaultState() {
        // arrange
        var expected = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        var defaultState = State.Initial();
        var piecesPartOfFen = FenCreator.GetFen(defaultState);
        // act
        // assert
    }
}