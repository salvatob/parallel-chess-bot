using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Search;

namespace UnitTests.MockPlayers;

public class ThrowingPlayer : MockPlayer {
    public override Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct = default) {
        throw new InvalidOperationException("Simulated crash");
    }
}