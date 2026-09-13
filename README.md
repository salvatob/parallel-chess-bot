# ParallelChessBot

ParallelChessBot is a high-performance C#/.NET chess framework. 
It provides reusable building blocks—such as bitboard representations, move generators, and search algorithms
for building chess engines and applications.


## Install
Clone the repository:
```console
git clone https://github.com/salvatob/parallel-chess-bot.git`
```

Once you have cloned the repository, add a reference to the core library:

```console
dotnet add reference path/to/ParallelChessBot/ChessBotCore/ChessBotCore.csproj
```

## Examples

### Basic Usage
## Using the Library


### Creating a State from FEN

You can easily create a board state by parsing a FEN (Forsyth-Edwards Notation) string using the `State.FromFen` method.

```csharp
using ChessBotCore;

// Initialize a state from any position in a string format
string startFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
State state = State.FromFen(startFen);

// Or from factory static properties
State initial = State.Initial;
State empty = State.Empty;
```

### Generating and Applying Moves

To generate moves, use the `GeneratorWrapper` class. It provides methods for both pseudo-legal and fully legal moves.

```csharp
using ChessBotCore;

State state = State.FromFen("r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3");

// Initialize the generator wrapper for the current state
var generator = new GeneratorWrapper(state);

// Get all legal moves
List<Move> legalMoves = generator.GetLegalMoves();

foreach (var move in legalMoves)
{
    Console.WriteLine($"Legal move: {move.PrintLAN()}");
}

// Apply a move to the state
if (legalMoves.Count > 0)
{
    Move firstMove = legalMoves[0];
    State.UndoInfo undoInfo = state.ApplyMove(firstMove);
    
    // ... do something with the new state ...
    
    // Undo the move to return to the previous state
    state.UndoMove(firstMove, undoInfo);
}
```

### Creating Players and Running a ChessGame

The `ChessGame` class orchestrates a match between two players. You can use the built-in `EnginePlayer` or `ConsolePlayer`, or implement your own `IPlayer`.

```csharp
using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Players;

// 1. Create players
IPlayer whitePlayer = new EnginePlayer(); // AI player
IPlayer blackPlayer = new ConsolePlayer(); // Human player via console

// 2. Configure game timers (optional, uses defaults if not provided)
Timers timers = Timers.Default;

// 3. Initialize the game
using ChessGame game = new ChessGame(whitePlayer, blackPlayer, timers, State.Initial);

// 4. Run the game asynchronously
GameResult result = await game.PlayAsync(verbosity: 1);

// 5. Check the result
Console.WriteLine($"Game Over: {result.Outcome} due to {result.Reason}");
```

### Custom Player Implementation

You can extend the framework by implementing `IPlayer`. This allows for custom AI logic or alternative interfaces:

```csharp
using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Players;
using ChessBotCore.Search;

public class MyCustomPlayer : IPlayer
{
    public SearchHandle ChooseMoveAsync(State state, Timers timers)
    {
        var cts = new CancellationTokenSource();
        var task = Task.Run(() => 
        {
            // Implement move selection logic
            var generator = new GeneratorWrapper(state);
            var move = generator.GetLegalMoves().First();
            return new SearchResults(move);
        }, cts.Token);

        return new SearchHandle(cts, task);
    }
    
    public Task PrepareAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> history) => Task.CompletedTask;
    public Task OnGameStartAsync() => Task.CompletedTask;
    public Task OnGameEndAsync(bool yourColor, GameResult result) => Task.CompletedTask;
    public Task OnOpponentsMoveAsync(Move move, State newState) => Task.CompletedTask;
    public Task OnErrorNotifyAsync(Exception error, bool gameEnd) => Task.CompletedTask;
    public Task OnErrorNotifyAsync(string message, bool gameEnd) => Task.CompletedTask;
    public void Dispose() { }
}
```

## Documentation

For a deep dive into the architecture and design decisions, refer to the [**Developer Documentation**](docs/Developer.md).

- [Solution Structure](docs/Developer.md#solution-structure)
- [Bitboard Representation](docs/Developer.md#bitboard-representation)
- [Move Generation](docs/Developer.md#move-generation-architecture)
- [Search Algorithms](docs/Developer.md#search-and-evaluation)
- [Performance & Benchmarking](docs/Developer.md#testing-and-benchmarking)

## Repository

The project is organized into several modules:

- [`ChessBotCore`](ChessBotCore/): The core library containing bitboard logic, move generators, and search algorithms.
- [`ConsoleInterface`](ConsoleInterface/): A CLI consumer for interactive play and testing.
- [`Benchmarks`](Benchmarks/): Performance testing suites using BenchmarkDotNet.
- [`UnitTests`](UnitTests/): Comprehensive tests for move generation and game rules.

## License

ParallelChessBot is available under the project's default license. See the repository root for more details.

## Support

- [Report a bug or request a feature](https://github.com/salvatob/ParallelChessBot/issues)
- [Read the Developer Documentation](docs/Developer.md)
