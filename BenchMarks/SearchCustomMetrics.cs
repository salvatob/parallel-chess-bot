using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using ChessBotCore;

namespace Benchmarks;

public class SearchCustomMetrics : IColumn {
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase) {
        // Console.WriteLine(benchmarkCase.DisplayInfo);
        // Console.WriteLine(benchmarkCase.Parameters);
        // Console.WriteLine(benchmarkCase.Descriptor.WorkloadMethod.Name);
        int depth = (int)benchmarkCase.Parameters.Items
            .Single(p => p.Name == nameof(Search.Depth))
            .Value;

        State state = (State)benchmarkCase.Parameters.Items
            .Single(p => p.Name == nameof(Search.state))
            .Value;
        
        Console.WriteLine(depth);
        Console.WriteLine(state.PrettyPrint());
        return "69";
    }
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) {
        return GetValue(summary, benchmarkCase);
    }
    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) {
        return false;
    }
    public bool IsAvailable(Summary summary) {
        return true;
    }
    public string Id => nameof(SearchCustomMetrics);
    public string ColumnName => "Nodes Searched";
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Custom;
    public int PriorityInCategory => 0;
    public bool IsNumeric => true;
    public UnitType UnitType => UnitType.Dimensionless;
    public string Legend => "I am not sure what to write here :(";
}