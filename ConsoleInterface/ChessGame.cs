using ChessBotCore;
using ChessBotCore.Players;

namespace ConsoleInterface;



public class ChessGame {
    
    private readonly IPlayer _blackPlayer;
    private readonly IPlayer _whitePlayer;
    private readonly State _state = State.Initial;
    private Timers _timers = new();

    public ChessGame(IPlayer whitePlayer, IPlayer blackPlayer) {
        _whitePlayer = whitePlayer;
        _blackPlayer = blackPlayer;
    }

    private IPlayer ActivePlayer(bool isWhite) {
        return isWhite ? _whitePlayer : _blackPlayer;
    }

    public async Task<List<Move>> Play(int verbosity) {
        while (!_state.IsTerminal()) { }

        throw new NotImplementedException();
    }
}