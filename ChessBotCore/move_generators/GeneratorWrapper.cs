namespace ChessBotCore;

public sealed class GeneratorWrapper : IMoveGenerator {

    private IMoveGenerator[] _generators;

    public GeneratorWrapper(params IMoveGenerator[] generators) {
        _generators = generators;
    }
    
    public IEnumerable<Move> GenerateMoves(State state) {
        foreach (IMoveGenerator generator in _generators) {
            foreach (Move move in generator.GenerateMoves(state)) {
                yield return move;
            }
        }
    }
}