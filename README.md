# ParallelChessBot

ParallelChessBot is a high-performance C#/.NET chess framework providing a suite of reusable building blocks for chess applications and engines. It is designed with a focus on engineering quality, efficient board representations, and high-throughput move generation.

Rather than being a standalone game, this project serves as a robust library for developers to build upon, demonstrating modern .NET practices in a computationally intensive domain.

## Install

To use ParallelChessBot in your .NET project, reference the `ChessBotCore` project or assembly.

```console
dotnet add reference ChessBotCore/ChessBotCore.csproj
```

## Example

### Basic Usage

Orchestrate a game between an AI engine and a human player using the built-in `ChessGame` class:

```csharp
using ChessBotCore;
using ChessBotCore.Game;
using ChessBotCore.Players;

// Initialize players
IPlayer white = new EnginePlayer();
IPlayer black = new ConsolePlayer();

// Configure and run the game
using var game = new ChessGame(white, black, Timers.Default, State.Default);
var result = await game.PlayAsync(verbosity: 1);

Console.WriteLine($"Result: {result.Outcome} due to {result.Reason}");
```

### Custom Player Implementation

Extend the framework by implementing your own `IPlayer`. This allows you to integrate custom AI logic, remote players, or alternative interfaces:

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
            // Implement your move selection logic here
            var generator = new GeneratorWrapper(state);
            var move = generator.GetLegalMoves().First();
            return new SearchResults(move);
        }, cts.Token);

        return new SearchHandle(cts, task);
    }
    
    // it is totally okay that these methods do not do any work
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

For a deep dive into the architecture, design decisions, and extension points, please refer to the [**Developer Documentation**](docs/Developer.md).

- [Architecture & Main Components](docs/Developer.md#architecture)
- [Bitboard Representation](docs/Developer.md#board-representation)
- [Move Generation](docs/Developer.md#move-generation)
- [Search Algorithms](docs/Developer.md#search)
- [Performance Benchmarking](docs/Developer.md#performance-and-benchmarks)

## Repository

The project is organized into several key modules:

- [`ChessBotCore`](ChessBotCore/): The core library containing bitboard logic, move generators, search algorithms, and game management.
- [`ConsoleInterface`](ConsoleInterface/): A CLI consumer of the framework used for interactive play and performance testing.
- [`Benchmarks`](Benchmarks/): Performance testing suites for micro-optimizations and full-scale engine throughput.
- [`UnitTests`](UnitTests/): Comprehensive test suite ensuring correctness of move generation and game rules.

Read the [Developer Documentation](docs/Developer.md) before contributing or extending the framework.

## License

ParallelChessBot is available under the project's default license. See the repository root for more details.

## Support

- [Report a bug or request a feature](https://github.com/salvatob/ParallelChessBot/issues)
- [Read the Developer Documentation](docs/Developer.md)
