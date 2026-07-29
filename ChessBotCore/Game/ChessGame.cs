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

    public ChessGame(IPlayer whitePlayer, IPlayer blackPlayer, Timers timers, State state) : this(whitePlayer, blackPlayer) {
        _state = state;
        _timers = timers;
    }
    
    private IPlayer ActivePlayer(bool isWhite) {
        return isWhite ? _whitePlayer : _blackPlayer;
    }


    
    public async Task<GameResult> Play(int verbosity=0) {
        List<Move> moveList = new();
        var wps = _whitePlayer.OnGameStartAsync(true, _state, _timers);
        var bps = _blackPlayer.OnGameStartAsync(false, _state, _timers);
        Task.WaitAll(wps, bps);

        while (!_state.IsTerminal()) {
            var player = ActivePlayer(_state.WhiteIsActive);
            
            var moveHandle = player.ChooseMoveAsync(_state, _timers);
           
            // TODO handle timers, add some stopwatches etc.
            
            var searchResult = await moveHandle.Result;


            var move = searchResult.BestMove;
            
            moveList.Add(move);
            _state.ApplyMove(move);
        }
        
        var gameResult = new GameResult(GameOutcome.Draw ,moveList);
        var wpe = _whitePlayer.OnGameGameEndAsync(true, gameResult);
        var bpe = _blackPlayer.OnGameGameEndAsync(false, gameResult);
        Task.WaitAll(wpe, bpe);
        
        return gameResult;
    }

    public void Dispose() {
        _blackPlayer.Dispose();
        _whitePlayer.Dispose();
    }
}