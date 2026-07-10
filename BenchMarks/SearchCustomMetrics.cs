using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using ChessBotCore;

namespace Benchmarks;

public class SearchCustomMetrics : IColumn {
    private static int EvaluateTestcase(MethodInfo caseMethod, State state, int depth) {
        var benchmarkCase = new Search {
            Depth = depth,
            state = state
        };
        
        return caseMethod.Name switch {
            nameof(Search.SimpleMinimax) => benchmarkCase.SimpleMinimax(),
            nameof(Search.SimpleNegamax) => benchmarkCase.SimpleNegamax(),
            nameof(Search.ABNegamax) => benchmarkCase.ABNegamax(),
            nameof(Search.SmartABNegamax) => benchmarkCase.SmartABNegamax(),
            _ => new Lazy<int>(() => {
                Console.Error.WriteLine($"Method {caseMethod.Name} not found in benchmark so it's node count is unknown.");
                return 0;
            }).Value
        };
    }
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase) {
        int depth = (int)benchmarkCase.Parameters.Items
            .Single(p => p.Name == nameof(Search.Depth))
            .Value;

        State state = (State)benchmarkCase.Parameters.Items
            .Single(p => p.Name == nameof(Search.state))
            .Value;
        
        var value = EvaluateTestcase(benchmarkCase.Descriptor.WorkloadMethod, state, depth);
        return value.ToString();
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