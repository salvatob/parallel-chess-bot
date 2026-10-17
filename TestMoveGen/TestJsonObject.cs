// ReSharper disable ClassNeverInstantiated.Global
namespace TestMoveGen;

public class TestSet {
    public required string Description { get; set; }
    public required TestCases[] TestCases { get; set; }
}

public class TestCases {
    public required Start Start { get; set; }
    public required Expected[] Expected { get; set; }
    public string? TestSet { get; set; }
    public override string ToString() => $"{TestSet??"unknown"}: " + Start.Description;

}

public class Start {
    public string? Description { get; set; }

    public required string Fen { get; set; }
}

public class Expected {
    public required string Move { get; set; }
    public required string Fen { get; set; }
}
