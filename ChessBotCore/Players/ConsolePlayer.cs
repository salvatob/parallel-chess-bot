using ChessBotCore.Game;
using ChessBotCore.Search;

namespace ChessBotCore.Players;

public class ConsolePlayer : IPlayer {
    public SearchHandle ChooseMoveAsync(State state, Timers timers) {
        var cts = new CancellationTokenSource();

        Console.WriteLine($"You are {(state.WhiteIsActive ? "white" : "black")}");
        Console.WriteLine($"You have {timers.ActiveTime(state.WhiteIsActive)} time");
        // Start the input task on a background thread
        var command = Task.Run(() => GetCommand(state, timers));

        // Use WaitAsync to link the task to the CancellationToken
        // This returns a new task that completes when 'command' finishes 
        // OR when 'cts.Token' is cancelled.
        Task<SearchResults> cancellableTask = command.WaitAsync(cts.Token);

        return new SearchHandle(cts, cancellableTask);
    }

    public void Dispose() { } // its totally okay this is empty

    public Task OnGameStartAsync(bool yourColor, State state, Timers timers, IReadOnlyList<Move> _) {
        Console.WriteLine($"You are {(yourColor ? "white" : "black")}");
        return Task.CompletedTask;
    }

    public Task OnGameEndAsync(bool yourColor, GameResult result) {
        if (result.Outcome == GameOutcome.Draw) {
            Console.WriteLine("Game was a draw");
            return Task.CompletedTask;
        }

        bool weWon = yourColor ^ (result.Outcome == GameOutcome.WhiteWin);

        var res = weWon switch {
            true => "You won!!!",
            false => "You lost!!!"
        };

        Console.WriteLine(res);
        return Task.CompletedTask;
    }

    public Task OnErrorNotifyAsync(Exception error, bool gameEnd) {
        return OnErrorNotifyAsync(error.Message, gameEnd);
    }

    public Task OnErrorNotifyAsync(string errorMessage, bool gameEnd) {
        Console.WriteLine("An error has occured:");
        Console.WriteLine(errorMessage);
        Console.WriteLine($"Game {(gameEnd ? "cannot" : "can")} continue.");
        return Task.CompletedTask;
    }

    private SearchResults GetCommand(State state, Timers timers) {
        while (true) {
            string? command = null;
            while (command is null) {
                command = Console.ReadLine();
            }

            MoveDTO moveDto;
            try {
                moveDto = MoveDTO.Parse(command);
            } catch (ArgumentException ae) {
                Console.WriteLine(ae.Message);
                continue;
            }
            try {
                var move = Move.FindFullMove(moveDto, state);
                return new SearchResults {
                    BestMove = move
                };
            } catch (ArgumentException ae) {
                Console.WriteLine($"Move <{moveDto}> is illegal");
                continue;
            }
        }
    }

    private bool ValidateMove(State state, MoveDTO move) {
        var copy = state.Clone();
        try {
            copy.ApplyMove(move);
            return true;
        }
        catch (ArgumentException) {
            return false;
        }
    }
}
