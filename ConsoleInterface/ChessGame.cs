using ChessBotCore;

namespace ConsoleInterface;

public class ChessGame {
    private IPlayer _blackPlayer;
    private IPlayer _whitePlayer;
    private State _state = State.Initial;
    public TimeSpan WhiteTime { get; private set; }
    public TimeSpan BlackTime { get; private set; }
    public TimeSpan Increment { get; private set; }
    
    public ChessGame(IPlayer whitePlayer, IPlayer blackPlayer) {
        _whitePlayer = whitePlayer;
        _blackPlayer = blackPlayer;
    }
    
    private IPlayer ActivePlayer(bool isWhite) => isWhite ? _whitePlayer : _blackPlayer;

    public async Task<List<Move>> Play(int verbosity) {
        while (!_state.IsTerminal()) {
            
        }
        throw new NotImplementedException();
    }
}
