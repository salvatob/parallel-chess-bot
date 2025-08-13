using System.Collections;
using System.Text.Json;
using ChessBotCore;
using FluentAssertions;
using TestMoveGen.testcases;

namespace TestMoveGen;

public class TestAgainstTestDatabase {
    public static IEnumerable<object[]> TestCases => LoadTestCases();

    
    public static IEnumerable<object[]> LoadTestCases() {
        string[] files = ["standard", "castling", "famous", "pawns", "promotions", "taxing"];
        foreach (var fileName in files) {
            
            var path = $@"C:\Users\tobia\RiderProjects\ParallelChessBot\TestMoveGen\testcases\{fileName}.json";

            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            };
            string json = new StreamReader(path).ReadToEnd();
            RootObject? cases = JsonSerializer.Deserialize<RootObject>(json, options);

            foreach (var c in cases.TestCases ) {
                yield return [c];
            }

        }
    }
    
    
    private string DeleteEnpassantFromFen(string fen) {
        var tokens = fen.Split(" ", 6);
        tokens[3] = "-";
        return string.Join(" ", tokens);
    }
    
    [Theory]
    // [InlineData("standard")]
    [MemberData(nameof(TestCases))]
    public void GetAllMoves(TestCases testCase) {
        //arrange
        var start = FenLoader.ParseFen(testCase.Start.Fen);

        IEnumerable<string> expected = 
            from c in testCase.Expected
            where c.Move != "0-0" && c.Move != "0-0-0"
            // select c.fen;
            select DeleteEnpassantFromFen(c.Fen);
        
        //act
        IEnumerable<Move> moves = GeneratorWrapper.Default.GetLegalMoves(start);
        
        var moveFens = moves.Select(m => FenCreator.GetFen(m.StateAfter));
        
        //assert
        moveFens.Should().BeEquivalentTo(expected);
    }
}