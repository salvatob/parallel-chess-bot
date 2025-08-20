namespace ChessBotCore;

public interface IChessWrapper {
    public long Perft(State state, int depth);

    public long EvalPerft(State state, int depth);
}

public abstract class ChessWrapperBase : IChessWrapper {
    public TextWriter Writer { get; set; }

    public ChessWrapperBase(TextWriter writer) {
        Writer = writer;
    }

    public ChessWrapperBase() : this(Console.Out) {}

    public abstract long Perft(State state, int depth) ;
    public abstract long EvalPerft(State state, int depth) ;
}