namespace TestMoveGen.testcases;
public class RootObject {
    public string description { get; set; }
    public TestCases[] testCases { get; set; }
}

public class TestCases {
    public Start start { get; set; }
    public Expected[] expected { get; set; }
}

public class Start {
    public string description { get; set; }
    public string fen { get; set; }
}

public class Expected {
    public string move { get; set; }
    public string fen { get; set; }
}
