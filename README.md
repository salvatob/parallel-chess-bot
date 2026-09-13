# ParallelChessBot

ParallelChessBot is a high-performance C#/.NET chess framework providing a suite of reusable building blocks for chess applications and engines. It is designed with a focus on engineering quality, efficient board representations, and high-throughput move generation.

Rather than being a standalone game, this project serves as a robust library for developers to build upon, demonstrating modern .NET practices in a computationally intensive domain.

## Key Engineering Highlights

This project showcases several advanced software engineering and algorithmic implementations:

- **Bitboard Representation**: Employs 64-bit unsigned integers to represent board states, enabling high-performance move generation and position analysis via bitwise operations and hardware intrinsics.
- **Efficient State Management**: Implements a mutable state model with an `Apply`/`Undo` architecture, minimizing memory allocations and pressure on the garbage collector during deep search tree traversals.
- **Modular Move Generation**: Features a decoupled architecture for move generation, supporting sliding pieces, pawns, and specialized king movements while maintaining a clean separation of concerns.
- **Parallel Performance Verification**: Includes a parallelized "Perft" implementation capable of verifying millions of leaf nodes across multiple threads, significantly accelerating the validation of move generation correctness.
- **Advanced Search Algorithms**: Implements a Negamax search engine enhanced with Alpha-Beta pruning, iterative deepening, and sophisticated move ordering to optimize tree exploration.
- **Performance-First Culture**: Integrates `BenchmarkDotNet` for rigorous performance tracking, ensuring that architectural changes maintain or improve the engine's throughput.

## Project Structure

- **ChessBotCore**: The core library containing bitboard logic, move generators, search algorithms, and game management.
- **ConsoleInterface**: A CLI consumer of the framework used for interactive play and performance testing.
- **Benchmarks & MacroBenchmarks**: Performance testing suites for micro-optimizations and full-scale engine throughput.
- **UnitTests**: Comprehensive test suite ensuring correctness of move generation, FEN parsing, and game rules.

## Documentation

For a deep dive into the architecture, design decisions, and extension points, please refer to the [**Developer Documentation**](docs/Developer.md).

The developer documentation includes:
- Architectural overview and component relationships.
- Detailed explanations of bitboard and state management.
- Guidance on implementing custom players and evaluation functions.
- Code examples for framework integration.

## Quick Start

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Using the Library


### 1. Creating a State from FEN

You can easily create a board state by parsing a FEN (Forsyth-Edwards Notation) string using the `State.FromFen` method.

```csharp
using ChessBotCore;

// Initialize a state from the starting position
string startFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
State state = State.FromFen(startFen);

// Or a custom position
State customState = State.FromFen("r1bqkbnr/pppp1ppp/2n5/4p3/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq - 2 3");
```

### 2. Generating and Applying Moves

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

### 3. Creating Players and Running a ChessGame

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
using ChessGame game = new ChessGame(whitePlayer, blackPlayer, timers, State.FromFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));

// 4. Run the game asynchronously
Console.WriteLine("Starting game...");
GameResult result = await game.PlayAsync(verbosity: 1);

// 5. Check the result
Console.WriteLine($"Game Over: {result.Outcome} due to {result.Reason}");
```

## Current Status

ParallelChessBot is currently a functional framework/library. While it includes a `ConsoleInterface` for testing and can be consumed by external applications (such as this [web application](https://github.com/salvatob/chess-server)), it is primarily intended as an extensible backend for chess-related software.
