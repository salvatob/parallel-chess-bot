using ChessBotCore;
using ChessBotCore.Players;

namespace ConsoleInterface;



public class ChessGame {
    
    private readonly IPlayer _blackPlayer;
    private readonly IPlayer _whitePlayer;
    private readonly State _state = State.Initial;
    private readonly Timers _timers = new();

    public ChessGame(IPlayer whitePlayer, IPlayer blackPlayer) {
        _whitePlayer = whitePlayer;
        _blackPlayer = blackPlayer;
    }

    private IPlayer ActivePlayer(bool isWhite) {
        return isWhite ? _whitePlayer : _blackPlayer;
    }

    public record class GameResult(GameResult.GameOutcome Outcome, List<Move> Moves) {
        public enum GameOutcome {
            WhiteWin,
            BlackWin,
            Draw
        }
           
        
    }
    
    public async Task<GameResult> Play(int verbosity) {
        List<Move> moveList = new();
        while (!_state.IsTerminal()) {
            var player = ActivePlayer(_state.WhiteIsActive);
            Console.WriteLine($"Player {player.GetType().Name} turn");
            Console.WriteLine(_state.PrettyPrint());
            
            var moveHandle = player.GetBestMove(_state, _timers);
           
            // var cts = new CancellationTokenSource();
            // Task.Run(async () => {
            //     await Task.Delay(TimeSpan.FromSeconds(10));
            //     Console.WriteLine("Time limit reached");
            //     moveHandle.Cancel();
            // }, cts.Token);
            //
            // moveHandle.Register(cts.Cancel);
            
            
            var searchResult = await moveHandle.Result;


            var move = searchResult.BestMove;
            
            Console.WriteLine($"Player {player.GetType().Name} made move {move.PrintUCI()}");
            Console.WriteLine();
            Console.WriteLine();
            
            moveList.Add(move);
            _state.ApplyMove(move);
        }

        return new (GameResult.GameOutcome.Draw ,moveList);
    }
}