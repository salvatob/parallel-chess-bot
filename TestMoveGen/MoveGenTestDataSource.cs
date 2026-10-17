using System.Collections;
using System.Text.Json;

namespace TestMoveGen;

public class MoveGenTestDataSource : IEnumerable<object[]> {
    public IEnumerator<object[]> GetEnumerator() {
        string[] files = ["standard", "castling", "famous", "pawns", "promotions", "taxing"];
        foreach (var fileName in files) {
            var path = AppContext.BaseDirectory + Path.Combine("testcases", $"{fileName}.json");

            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                RespectNullableAnnotations = true
            };
            string json = File.ReadAllText(path);
            TestSet cases = JsonSerializer.Deserialize<TestSet>(json, options)
                            ?? throw new JsonException("The JSON root cannot be null.");

            foreach (var c in cases.TestCases) {
                c.TestSet = fileName;
                yield return [c];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
