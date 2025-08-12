using System.Text.Json;
using ChessBotCore;
using FluentAssertions;
using TestMoveGen.testcases;

namespace TestMoveGen;

public class TestAgainstTestDatabase {
    private string DeleteEnpassantFromFen(string fen) {
        var tokens = fen.Split(" ", 6);
        tokens[3] = "-";
        return string.Join(" ", tokens);
    }
    
    [Theory]
    [InlineData("standard")]
    public void GetAllMoves(string fileName) {
        //arrange
        var path = $@"C:\Users\tobia\RiderProjects\ParallelChessBot\TestMoveGen\testcases\{fileName}.json";

        var cases = JsonSerializer.Deserialize<RootObject>(new StreamReader(path).ReadToEnd());


        foreach (var testCase in cases.testCases) {
            var start = FenLoader.ParseFen(testCase.start.fen);

            IEnumerable<Move> moves = GeneratorWrapper.Default.GetLegalMoves(start);

            var moveFens = moves.Select(m => FenCreator.GetFen(m.StateAfter));
            IEnumerable<string> expected = 
                from c in testCase.expected
                where c.move != "0-0" && c.move != "0-0-0"
                // select c.fen;
                select DeleteEnpassantFromFen(c.fen);

            moveFens.Should().BeEquivalentTo(expected);
        }
        
        //act

        //assert
    }
}