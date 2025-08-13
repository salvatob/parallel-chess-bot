namespace TestMoveGen.testcases;
public class RootObject {
    public string Description { get; set; }
    public TestCases[] TestCases { get; set; }
}

public class TestCases {
    public Start Start { get; set; }
    public Expected[] Expected { get; set; } 
    public override string ToString() => Start.Description;

}

public class Start {
    public string Description { get; set; }
    public string Fen { get; set; }
    // public override string ToString() => Description;
}

public class Expected {
    public string Move { get; set; }
    public string Fen { get; set; }
}
