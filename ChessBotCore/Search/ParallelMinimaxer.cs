using ChessBotCore.MoveGenerators;

namespace ChessBotCore.Search;

public sealed class ParallelMinimaxer {
    private static readonly IMoveGenerator _generator = new MoveGenerator();
    

    private static SearchScore Eval(State s) {
        int score = Evaluator.Evaluate(s);
        return new SearchScore(score, score is short.MinValue or short.MaxValue);
    }
    private static bool IsTerminal(State s) => Evaluator.IsTerminal(s);


    public Move ChooseBestMove(State state, int maxDepth) {
        return MinimaxSetup(state, maxDepth);
    }

    private Move MinimaxSetup(State state, int maxDepth) {
        bool isMaxing = state.WhiteIsActive;

        using var moves = _generator.GenerateMoves(state).GetLegalMoves().GetEnumerator();
        
        // querying for a move when stalemated is undefined behaviour
        if (!moves.MoveNext()) return default;

        SearchScore bestScore = isMaxing ? new SearchScore(int.MinValue) : new SearchScore(int.MaxValue);
        Move bestMove = default;

        // TODO this code is weird and not even parallel
        do {
            var move = moves.Current;
            var undo = state.ApplyMove(move);
            SearchScore currentScore = Minimax(state, maxDepth - 1);
            state.UndoMove(move, undo);

            if (isMaxing) {
                if (currentScore > bestScore) {
                    bestMove = move;
                    bestScore = currentScore;
                }
            }
            else {
                if (currentScore < bestScore) {
                    bestMove = move;
                    bestScore = currentScore;
                }
            }
        } while (moves.MoveNext());

        return bestMove;
    }

    internal SearchScore Minimax(State state, int depth) {
        if (depth <= 0 || IsTerminal(state)) return Eval(state);

        bool isMaxing = state.WhiteIsActive;
        SearchScore bestScore = isMaxing ? new SearchScore(int.MinValue) : new SearchScore(int.MaxValue);

        // todo sort the moves somehow 
        var moves = _generator.GenerateMoves(state).GetLegalMoves().GetEnumerator();


        // no moves means stalemate, which is loss for both players
        if (!moves.MoveNext()) return new SearchScore(0);

        do {
            var move = moves.Current;
            var undo = state.ApplyMove(move);
            SearchScore currentScore = Minimax(state, depth - 1);
            state.UndoMove(move, undo);

            if (isMaxing) {
                if (currentScore > bestScore) bestScore = currentScore;
            } else {
                if (currentScore < bestScore) bestScore = currentScore;
            }
        } while (moves.MoveNext());

        return bestScore;
    }
}
