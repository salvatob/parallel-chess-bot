# Developer Documentation

## Overview
ParallelChessBot is a high-performance chess engine framework written in C#.
Its primary focus is on efficient board representation, move generation, and parallel search algorithms.
It is designed to be extensible, allowing developers to implement their own players, evaluation functions, or search strategies.
In future, an UCI interface is planned to be added to allow for easy integration with chess engines/GUIs.

## Solution Structure

The solution is divided into several projects, each with a clear responsibility:

*   **ChessBotCore**: The heart of the application. It contains the board representation, move generation logic, search engines, and game management.
*   **ConsoleInterface**: A simple console-based application to interact with the engine, play games, and run perft tests.
*   **Benchmarks & MacroBenchmarks**: Projects using `BenchmarkDotNet` to measure the performance of bitboard operations, move generation, and search.
*   **UnitTests**: A suite of tests ensuring the correctness of move generation, game logic, and FEN parsing.
*   **TestMoveGen**: A utility project for specialized move generation testing.

## Core Design Concepts

### Bitboard Representation
The engine uses a **Bitboard** representation, where the state of the board for each piece type and color is stored as a 64-bit unsigned integer (`ulong`). Each bit corresponds to a square on the 8x8 chess grid.

*   **`Bitboard.cs`**: A lightweight `readonly struct` wrapping the `ulong`. It provides bitwise operations and helper methods like `PopCount()` and `TrailingZeroCount()`.
*   **`BitMask.cs`**: Contains pre-calculated masks for rows, columns, and specific squares to speed up calculations.

### State Management
The **`State`** class represents a complete chess position. It includes:
- Bitboards for all pieces (WhitePawns, BlackKnights, etc.).
- Metadata: Active player, castling rights, en passant square, and the half-move clock for the 50-move rule.
- **Apply/Undo Logic**: The engine supports an efficient `ApplyMove` and `UndoMove` pattern, which is crucial for deep tree searches without constantly cloning the entire state.
- 

### Applying a move
The **`Move`** is a small struct that encapsulates the move details, 
such as positions from and where to move the piece, and optional info,
like if the move is a promotion, castle, or a capture.
This metatada is very useful in the search process, since ordering moves 
before searching them allows for increased alpha-beta hits, with tiny overhead.

I have decided on a approach, where the state is mutable, 
and during search, the moves are applied in place. That is significantly faster that generating 
a new state on each move, however is a bit more complex and requires the UndoInfo architecture.

## Move Generation Architecture

Move generation is designed using a modular approach:

1.  **`IMoveGenerator`**: An interface defining a `GenerateMoves(State state, List<Move> buffer)` method.
2.  **`MoveGeneratorBase`**: An abstract class providing common utilities like `CreateMove`.
3.  **Piece-Specific Generators**:
    *   **`RayMoveGenerator`**: A base for sliding pieces (Rook, Bishop, Queen) using bitboard shifts to calculate paths.
    *   **`PawnMoveGenerator`**, **`KnightMoveGenerator`**, **`KingMoveGenerator`**: Handle the unique movement rules for these pieces.
4.  **Pseudo-Legal Generation**: By default, the generators produce pseudo-legal moves (moves that follow piece movement rules but might leave the king in check). The search engine or game logic is responsible for filtering out illegal moves.

## Game loop

The process of playing a game is managed with the **`ChessGame`** class.
It handles creation, game flow, and correct disposal of a chess game.
Introduces safe API to limit invalid states (only one player connected, game stuck in a loop or a deadlock).

Just set players using **`RegisterPlayer`**, run the **`PlayAsync`** method in a separate thread, and collect 
the **`GameResult`** type to get the final outcome.

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

## Search and Evaluation

### Minimax Engine
The **`MinimaxEvaluator`** implements the core search logic:
*   **Negamax**: A variant of Minimax that simplifies the implementation for zero-sum games.
*   **Alpha-Beta Pruning**: Significantly reduces the number of nodes evaluated by "pruning" branches that cannot possibly influence the final decision.
*   **Iterative Deepening**: Searches progressively deeper (depth 1, then 2, then 3...) to ensure a move is always available and to improve move ordering for alpha-beta pruning.

### Evaluation Function
The **`Evaluator.cs`** provides a static `Evaluate` method. It currently uses:
*   **Material Weighting**: Assigning values to pieces (e.g., Queen = 900, Pawn = 40).
*   **Positional Bonus**: Simple heuristics like pawn advancement bonuses.

## Extensibility

ParallelChessBot is designed to be extended. Possible such ideas include:

*   **Custom Players**: Virtually anything, that is able to return a move could be put inside a custom **`IPlayer`** class.
*   **Smarter Search Algorithm**:Implement a custom SearchEvaluator and inject it into the **`EnginePlayer`**  (e.g., an agent that uses a different search algorithm or a neural network).
*   **New Evaluation Logic**: Modify or replace the `Evaluator` class to implement more complex scoring (like piece-square tables, king safety, or pawn structure analysis).
*   **Parallelism**: The `Performance` class contains experiments with parallelizing the search. Developers can look into `ParallelPerft` as a starting point for implementing parallel search in `MinimaxEvaluator`.

## User Interface & Interaction

The engine communicates primarily through the **`IPlayer`** abstraction. 
*   **`ConsolePlayer`**: Handles user input from the terminal.
*   **`EnginePlayer`**: Wraps the `MinimaxEvaluator` to act as an automated opponent.
*   **`SearchHandle`**: Wraps the asynchronous search task, allowing callers to monitor progress or trigger cancellation.

The **`ChessGame`** class orchestrates the game flow, alternating between players and checking for game-over conditions like checkmate, stalemate, or three-fold repetition.

## Testing and Benchmarking

*   **Perft (Performance Test)**: Used to verify move generation correctness by counting all possible moves up to a certain depth. See `Performance.Perft`.
*   **Unit Tests**: Located in the `UnitTests` project, covering everything from basic bitboard manipulation to complex game rules.
*   **Benchmarks**: Use the `Benchmarks` project to verify that changes don't negatively impact the performance of critical paths.
