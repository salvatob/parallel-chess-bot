# ParallelChessBot

A high-performance chess engine core written in C#, focusing on parallel move generation and evaluation.

## Overview

ParallelChessBot is a .NET-based chess library and engine designed to explore parallel processing techniques in chess move generation and search. It includes a core logic library, a console interface for testing, and comprehensive benchmarking and testing suites.

## Features

- **Parallel Move Generation:** Utilizing multiple cores for faster move exploration.
- **Bitboard Representation:** Efficient board representation for performance.
- **Perft Testing:** Built-in performance testing (Perft) to verify move generator correctness and speed.
- **Comprehensive Benchmarking:** Integration with BenchmarkDotNet for precise performance measurements.
- **Extensive Test Suites:** Unit tests and specialized move generation tests against external databases.
- [**Web Application**](https://github.com/salvatob/chess-server) allowing users to play online matches agains this engine.

## Tech Stack

- **Language:** C# 13.0
- **Framework:** .NET 9.0
- **Testing:** xUnit, FluentAssertions
- **Benchmarking:** BenchmarkDotNet
- **Scripts:** Python (for benchmark comparison)

## Project Structure

- `ChessBotCore/`: The main library containing chess logic, move generators, and board state management.
- `ConsoleInterface/`: A CLI application to run Perft tests and interact with the engine.
- `Benchmarks/`: Performance benchmarks for various engine components.
- `UnitTests/`: General unit tests for library logic.
- `TestMoveGen/`: Specialized tests for move generation, including validation against external test cases.
- `helper_scripts/`: Utility scripts, such as branch-to-branch benchmark comparisons.
- `BenchmarkDotNet.Artifacts/`: Generated benchmark results.

## Requirements

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Python 3.x (optional, for helper scripts)

## Setup & Run

### Clone the repository
```bash
git clone https://github.com/yourusername/ParallelChessBot.git
cd ParallelChessBot
```

### Build the solution
```bash
dotnet build
```

### Run the Console Interface
This will run the default Perft tests defined in `Program.cs`.
```bash
dotnet run --project ConsoleInterface
```

### Run Benchmarks
To run performance benchmarks:
```bash
dotnet run -c Release --project Benchmarks
```

### Run Tests
To run all tests in the solution:
```bash
dotnet test
```

## License

TODO: Add license information.
