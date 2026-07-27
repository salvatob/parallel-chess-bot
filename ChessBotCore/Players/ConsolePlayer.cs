using ChessBotCore.Game;
using ChessBotCore.Search;

namespace ChessBotCore.Players;

public class ConsolePlayer : IPlayer {
    public SearchHandle ChooseMoveAsync(State state, Timers timers) {
        
        var cts = new CancellationTokenSource();
    
        // Start the input task on a background thread
        var command = Task.Run(() => GetCommand(state, timers));
    
        // Use WaitAsync to link the task to the CancellationToken
        // This returns a new task that completes when 'command' finishes 
        // OR when 'cts.Token' is cancelled.
        Task<SearchResults> cancellableTask = command.WaitAsync(cts.Token);
                
        return new SearchHandle(cts, cancellableTask);
        
    }

    private SearchResults GetCommand(State state, Timers timers) {
        var command = Console.ReadLine();
        if (command is null) throw new InvalidOperationException($"No command for the {nameof(ConsolePlayer)}.");
        var move = Move.Parse(command);
        return new SearchResults {
            BestMove = move
        };
    }
    
    private bool ValidateMove(State state, Move move) {
        var copy = state.Clone();
        try {
            copy.ApplyMoveWithoutMetadata(move);
            return true;
        } catch (ArgumentException) {
            return false;
        }
    }
    
    public void Dispose() { } // its totally okay this is empty

    public Task OnGameStartAsync(bool yourColor, State state) {
        Console.WriteLine($"You are {(yourColor ? "white" : "black")}");
        return Task.CompletedTask;
    }
    
    public Task OnGameGameEndAsync(bool yourColor, GameResult result) {
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
}
