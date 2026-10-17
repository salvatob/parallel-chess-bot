using System.Text;
using System.Text.Json;
using ChessBotCore;
using ChessBotCore.MoveGenerators;
using Xunit.Abstractions;

namespace TestMoveGen;

public class TestAgainstTestDatabase {
    private readonly ITestOutputHelper _out;
    private readonly IMoveGenerator _generator = new MoveGenerator();
    
    public TestAgainstTestDatabase(ITestOutputHelper output) {
        _out = output;
    }
    
    [Theory]
    [ClassData(typeof(MoveGenTestDataSource))]
    public void GetAllMoves(TestCases testCase) {
        //arrange
        
        _out.WriteLine($"TestCase fen : {testCase.Start.Fen}");
        var start = State.FromFen(testCase.Start.Fen);

        IEnumerable<string> expectedEnumerable =
            from c in testCase.Expected
            select c.Fen;
        
        //act
        var moves = _generator.GenerateMoves(start).GetLegalMoves();

        var moveFens = moves.Select(m => {
            var nextState = start.Clone();
            nextState.ApplyMove(m);
            return nextState.GetFen();
        });

        var expected = new HashSet<string>(expectedEnumerable);
        var actual = new HashSet<string>(moveFens);

        var missing = expected.Except(actual).OrderBy(x => x).ToArray();
        var extra = actual.Except(expected).OrderBy(x => x).ToArray();

        // only detect false positives (not generating some legal moves is fine, generating illegal is not)
        if (missing.Length == 0 && extra.Length == 0)
            return; // success

        var msg = new StringBuilder();
        msg.AppendLine("Move generation mismatch for origin FEN:");
        msg.AppendLine(testCase.Start.Fen);
        msg.AppendLine();
        msg.AppendLine("Origin position:");
        msg.AppendLine(AsciiBoardFromFen(testCase.Start.Fen));
        msg.AppendLine();
        msg.AppendLine($"Expected count: {expected.Count}, Actual count: {actual.Count}");
        msg.AppendLine($"Missing count: {missing.Length}, Extra count: {extra.Length}");
        msg.AppendLine();

        if (missing.Any()) {
            msg.AppendLine($"MISSING ({missing.Length}) - expected but not generated:");
            foreach (var fen in missing) {
                var moveName = testCase.Expected.First(exp => exp.Fen == fen).Move;
                msg.AppendLine("---- missing result fen ----");
                msg.AppendLine($"Expected: {moveName}");
                msg.AppendLine(fen);
                msg.AppendLine(AsciiBoardFromFen(fen));
            }
        }

        if (extra.Any()) {
            msg.AppendLine($"UNEXPECTED ({extra.Length}) - generated but not expected:");
            foreach (var fen in extra) {
                msg.AppendLine("---- unexpected result fen ----");
                msg.AppendLine(fen);
                msg.AppendLine(AsciiBoardFromFen(fen));
            }
        }

        Assert.Fail(msg.ToString());
    }

    // Convert a FEN -> 8x8 ASCII board (only uses the placement part)
    public static string AsciiBoardFromFen(string fen) {
        var parts = fen.Split(' ');
        if (parts.Length == 0) return fen;

        var rows = parts[0].Split('/');
        if (rows.Length != 8) return fen;

        char[,] board = new char[8, 8];
        for (int r = 0; r < 8; r++)
        for (int f = 0; f < 8; f++)
            board[r, f] = '.';

        for (int rank = 0; rank < 8; rank++) {
            string row = rows[rank];
            int file = 0;
            foreach (char c in row) {
                if (char.IsDigit(c)) {
                    int skip = c - '0';
                    file += skip;
                }
                else {
                    board[rank, file] = c;
                    file++;
                }
            }
        }

        // Build printable string with ranks 8..1
        var sb = new StringBuilder();
        for (int r = 0; r < 8; r++) {
            sb.Append(8 - r).Append("  ");
            for (int f = 0; f < 8; f++) sb.Append(board[r, f]).Append(' ');
            sb.AppendLine();
        }

        sb.AppendLine();
        sb.AppendLine("   a b c d e f g h");
        return sb.ToString();
    }
}
