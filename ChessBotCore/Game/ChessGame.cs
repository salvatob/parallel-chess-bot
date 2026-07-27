using ChessBotCore.Players;

namespace ChessBotCore.Game;
public enum GameOutcome {
    WhiteWin,
    BlackWin,
    Draw
}

public record GameResult(GameOutcome Outcome, List<Move> Moves);

public class ChessGame : IDisposable {
    
    private readonly IPlayer _blackPlayer;
    private readonly IPlayer _whitePlayer;
    private readonly State _state = State.Initial;
    private readonly Timers _timers = new() {
        BaseWhiteTime = TimeSpan.FromMinutes(5),
        BaseBlackTime = TimeSpan.FromMinutes(5),
        Increment = TimeSpan.FromSeconds(2)
    };

    public ChessGame(IPlayer whitePlayer, IPlayer blackPlayer) {
        _whitePlayer = whitePlayer;
        _blackPlayer = blackPlayer;
    }

    private IPlayer ActivePlayer(bool isWhite) {
        return isWhite ? _whitePlayer : _blackPlayer;
    }


    
    public async Task<GameResult> Play(int verbosity) {
        List<Move> moveList = new();
        while (!_state.IsTerminal()) {
            var player = ActivePlayer(_state.WhiteIsActive);
            Console.WriteLine($"Player {player.GetType().Name} turn");
            Console.WriteLine(_state.PrettyPrint());
            
            var moveHandle = player.ChooseMoveAsync(_state, _timers);
           
            // TODO handle timers, add some stopwatches etc.
            
            var searchResult = await moveHandle.Result;


            var move = searchResult.BestMove;
            
            Console.WriteLine($"Player {player.GetType().Name} made move {move.PrintUCI()}");
            Console.WriteLine();
            Console.WriteLine();
            
            moveList.Add(move);
            _state.ApplyMove(move);
        }

        return new (GameOutcome.Draw ,moveList);
    }

    public void Dispose() {
        _blackPlayer.Dispose();
        _whitePlayer.Dispose();
    }
}