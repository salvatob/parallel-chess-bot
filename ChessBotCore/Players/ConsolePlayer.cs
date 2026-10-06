using ChessBotCore.Game;
using ChessBotCore.Search;

namespace ChessBotCore.Players;

/// <summary>
/// A chess player implementation that allows a human to play via the console.
/// </summary>
public class ConsolePlayer : IPlayer {
    private TextReader _reader;
    private TextWriter _writer;

    public ConsolePlayer(TextReader? reader = null, TextWriter? writer = null) {
        _reader = reader ?? Console.In;
        _writer = writer ?? Console.Out;
    }
    
    
    /// <inheritdoc/>
    public async Task<SearchResults> ChooseMoveAsync(State state, Timers timers, CancellationToken ct) {
        
        _writer.WriteLine($"You are {(state.WhiteIsActive ? "white" : "black")}");
        _writer.WriteLine($"You have {timers.ActiveTime(state.WhiteIsActive)} time");
        // Start the input task on a background thread
        var command = await GetCommand(state, ct);
    
        return command;
    }
    
    /// <summary>
    /// Waits asynchronously until the user enters a valid move.
    /// </summary>
    /// <param name="state">The current board state.</param>
    /// <param name="ct">Token to stop this operation.</param>
    /// <returns>The search results containing the chosen move.</returns>
    private async Task<SearchResults> GetCommand(State state, CancellationToken ct) {
        while (true) {
            // this should throw if the token is cancelled
            var command = await _reader.ReadLineAsync(ct);
            if (command is null) {
                ct.ThrowIfCancellationRequested();
                throw new MoveException("Move command has failed.");
            }
            if (string.IsNullOrWhiteSpace(command)) continue;
            
            MoveDTO moveDto;
            try {
                moveDto = MoveDTO.Parse(command);
            } catch (ArgumentException ae) {
                _writer.WriteLine(ae.Message);
                continue;
            }
            try {
                var move = Move.FindFullMove(moveDto, state);
                return new SearchResults {
                    BestMove = move
                };
            } catch (ArgumentException ae) {
                _writer.WriteLine($"Move <{moveDto}> is illegal");
                continue;
            }
        }
    }
    
    /// <inheritdoc/>
    public void Dispose() { } // its totally okay this is empty

    /// <inheritdoc/>
    public Task PrepareAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> moveHistory) {
        _writer.WriteLine($"You are {(yourColor ? "white" : "black")}");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnGameStartAsync() {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnGameEndAsync(bool youreWhite, GameResult result) {
        if (result.Outcome == GameOutcome.Draw) {
            _writer.WriteLine("Game was a draw");
            return Task.CompletedTask;
        }

        bool weWon = youreWhite == (result.Outcome == GameOutcome.WhiteWin);

        var res = weWon switch {
            true => "You won!!!",
            false => "You lost!!!"
        };

        _writer.WriteLine(res);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnOpponentsMoveAsync(Move move, State newState) {
        _writer.WriteLine($"Opponent played: {move.PrintLAN()}");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task OnErrorNotifyAsync(Exception error, bool gameEnd) {
        return OnErrorNotifyAsync(error.Message, gameEnd);
    }

    /// <inheritdoc/>
    public Task OnErrorNotifyAsync(string errorMessage, bool gameEnd) {
        _writer.WriteLine("An error has occured:");
        _writer.WriteLine(errorMessage);
        _writer.WriteLine($"Game {(gameEnd ? "cannot" : "can")} continue.");
        return Task.CompletedTask;
    }
}
