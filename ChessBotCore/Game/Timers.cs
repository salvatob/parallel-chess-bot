using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Game;

/// <summary>
/// Manages the game timers for both white and black players.
/// </summary>
public class Timers {
    private readonly TimeSpan _baseBlackTime = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _baseWhiteTime = TimeSpan.FromMinutes(5);
    private TimeSpan _blackTime;
    private TimeSpan _whiteTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="Timers"/> class.
    /// </summary>
    [SetsRequiredMembers]
    public Timers() { }

    /// <summary> The starting time for white. </summary>
    public required TimeSpan BaseWhiteTime {
        get => _baseWhiteTime;
        init {
            _baseWhiteTime = value;
            WhiteTime = value;
        }
    }

    /// <summary> The starting time for black. </summary>
    public required TimeSpan BaseBlackTime {
        get => _baseBlackTime;
        init {
            _baseBlackTime = value;
            BlackTime = value;
        }
    }

    /// <summary> The current time remaining for white. </summary>
    public TimeSpan WhiteTime {
        get => _whiteTime;
        set => _whiteTime = value;
    }

    /// <summary> The current time remaining for black. </summary>
    public TimeSpan BlackTime {
        get => _blackTime;
        set => _blackTime = value;
    }

    /// <summary> The time increment added to a player's clock after each move. </summary>
    public required TimeSpan Increment { get; init; }

    /// <summary>
    /// Gets a default timer configuration (5 minutes base, 2 seconds increment).
    /// </summary>
    public static Timers Default => new() {
        BaseWhiteTime = TimeSpan.FromMinutes(5),
        BaseBlackTime = TimeSpan.FromMinutes(5),
        Increment = TimeSpan.FromSeconds(2)
    };

    /// <summary> White's remaining time in milliseconds. </summary>
    public long WhiteTimeMs => (long)WhiteTime.TotalMilliseconds;
    /// <summary> Black's remaining time in milliseconds. </summary>
    public long BlackTimeMs => (long)BlackTime.TotalMilliseconds;
    /// <summary> The increment in milliseconds. </summary>
    public long IncrementMs => (long)Increment.TotalMilliseconds;

    /// <summary>
    ///     Updates the active player's timer, that is subtracting the elapsed time, and adding the Increment.
    /// </summary>
    /// <param name="elapsedTime">The amount of time the player used for their move.</param>
    /// <param name="white">True means the player is white.</param>
    /// <returns>If the new value is negative, ie the player has lost on time.</returns>
    public bool UpdateTimer(TimeSpan elapsedTime, bool white) {
        ref var activeTime = ref ActiveTime(white);
        activeTime -= elapsedTime;
        if (activeTime < TimeSpan.Zero) return false;
        activeTime += Increment;
        return true;
    }

    /// <summary>
    ///     Returns the active players Time by reference.
    /// </summary>
    /// <param name="color">True means white</param>
    /// <returns>The Time by reference.</returns>
    public ref TimeSpan ActiveTime(bool color) {
        return ref color ? ref _whiteTime : ref _blackTime;
    }
}
