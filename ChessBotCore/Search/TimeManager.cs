using ChessBotCore.Game;

namespace ChessBotCore.Search;

/// <summary>
/// Calculates, how much time should an engine allocate to the search of the current move.
/// </summary>
public interface ITimeManager {
    public TimeSpan CalculateAllowedTime(Timers timers, bool isWhite);
}


/// <summary>
/// The default and preferred time manager type that should be used almost everywhere 
/// </summary>
public class TimeManager : ITimeManager {
    public TimeSpan CalculateAllowedTime(Timers timers, bool isWhite) {
        var timeLeft = timers.ActiveTime(isWhite);
        var timePerMoveFraction = timeLeft / 20 + timers.Increment / 2;
        return new[] { timePerMoveFraction, timeLeft / 2 }.Min(); // never should use more than half of remaining time
    }
}