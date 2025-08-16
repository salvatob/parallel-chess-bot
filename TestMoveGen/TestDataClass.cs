using System.Collections;
using System.Text.Json;

namespace TestMoveGen;

public class TestDataClass : IEnumerable<object[]> {

    private static List<TestCases>? _cases;
    public IEnumerator<object[]> GetEnumerator() {
        if (_cases is null)
            _cases = LoadTestCases().ToList();
            
        return  _cases.Select(c=> new object[] {c}).ToList().GetEnumerator();
        
        return (IEnumerator<object[]>)(new object[4]).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
    
    // private static readonly Lazy<List<object[]>> s_all = new(() => LoadTestCases().ToList());
    //
    // public static IEnumerable<object[]> TestCases => s_all.Value;

    
    // public static IEnumerable<object[]> LoadTestCases() {
    public static IEnumerable<TestCases> LoadTestCases() {
        string[] files = ["standard", "castling", "famous", "pawns", "promotions", "taxing"];
        foreach (var fileName in files) {
            
            var path = 
                // AppContext.BaseDirectory 
                AppDomain.CurrentDomain.BaseDirectory
                + $@"\TestMoveGen\testcases\{fileName}.json";
                // $@"C:\Users\tobia\RiderProjects\ParallelChessBot\TestMoveGen\testcases\{fileName}.json";

            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            };
            string json = new StreamReader(path).ReadToEnd();
            RootObject? cases = JsonSerializer.Deserialize<RootObject>(json, options);

            foreach (var c in cases.TestCases ) {
                yield return c;
                // yield return [c];
            }
        }
    }

}