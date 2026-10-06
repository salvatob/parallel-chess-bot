using ChessBotCore;

namespace UnitTests.StateTests;

public class StateToMatrix_Dense {
    private readonly char[] _emptyRow = new char[8]
        { default, default, default, default, default, default, default, default };

    public IEnumerable<object[]> StateToMatrixDense_Data => new[] {
        new object[] {
            State.Initial, new char[,] {
                // _emptyRow
            }
        }
    };
}
