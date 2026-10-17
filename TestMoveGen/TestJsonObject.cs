// ReSharper disable ClassNeverInstantiated.Global
namespace TestMoveGen;

public class TestSet {
    public required string Description { get; init; }
    public required TestCases[] TestCases { get; init; }
}

public class TestCases {
    public required Start Start { get; init; }
    public required Expected[] Expected { get; init; }
    public string? TestSet { get; set; }
    public override string ToString() => $"{TestSet??"unknown"}: " + Start.Description;

}

public class Start {
    public string? Description { get; init; }

    public required string Fen { get; init; }
}

public class Expected {
    public required string Move { get; init; }
    public required string Fen { get; init; }
}
