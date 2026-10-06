using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Players;
using ChessBotCore.Search;

namespace UnitTests.MockPlayers;

public class SequencePlayer : MockPlayer {
    private readonly List<MoveDTO> _moves;
    private int _moveIndex = 0;

    public SequencePlayer(IEnumerable<string> moves) {
        _moves = moves.Select(MoveDTO.Parse).ToList();
    }

    public override Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct) {
        if (_moveIndex >= _moves.Count) {
            throw new MoveException("No more moves in sequence");
        }

        var moveDto = _moves[_moveIndex++];
        var move = Move.FindFullMove(moveDto, state);
            
        var result = new SearchResults {
            BestMove = move
        };
            
        return Task.FromResult(result);
    }
}