using ChessBotCore.Game;
using ChessBotCore.MoveGenerators;
using ChessBotCore.Search;

namespace ChessBotCore.Players;

public class RandomPlayer : IPlayer {
    // ReSharper disable once InconsistentNaming
    private static readonly Random _random = new();
    public void Dispose() { }

    public SearchHandle ChooseMoveAsync(State state, Timers timers) {
        List<Move> moves = new GeneratorWrapper(state).GetLegalMoves();
        Move move = moves[_random.Next(moves.Count)];

        var result = new SearchResults {
            BestMove = move
        };
        
        return new SearchHandle(new CancellationTokenSource(), Task.FromResult(result));
    }

    public Task OnGameStartAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> _) {
        return Task.CompletedTask;
    }

    public Task OnGameGameEndAsync(bool yourColor, GameResult result) {
        return Task.CompletedTask;
    }

    public Task OnErrorNotifyAsync(Exception error, bool gameEnd) {
        return Task.CompletedTask;
    }

    public Task OnErrorNotifyAsync(string errorMessage, bool gameEnd) {
        return Task.CompletedTask;
    }
}
