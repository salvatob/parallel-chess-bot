using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Game;

public class Timers {
    private readonly TimeSpan _baseWhiteTime = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _baseBlackTime = TimeSpan.FromMinutes(5);
    private TimeSpan _whiteTime;
    private TimeSpan _blackTime;

    public required TimeSpan BaseWhiteTime {
        get => _baseWhiteTime;
        init {
            _baseWhiteTime = value;
            WhiteTime = value;
        }
    }

    public required TimeSpan BaseBlackTime {
        get => _baseBlackTime;
        init {
            _baseBlackTime = value;
            BlackTime = value;
        }
    }

    public TimeSpan WhiteTime {
        get => _whiteTime;
        set => _whiteTime = value;
    }

    public TimeSpan BlackTime {
        get => _blackTime;
        set => _blackTime = value;
    }

    public required TimeSpan Increment { get; init; }

    [SetsRequiredMembers]
    public Timers() {}

    public static Timers Default => new Timers() {
        BaseWhiteTime = TimeSpan.FromMinutes(5),
        BaseBlackTime = TimeSpan.FromMinutes(5),
        Increment = TimeSpan.FromSeconds(2)
    };

    /// <summary>
    /// Updates the active player's timer, that is subtracting the elapsed time, and adding the Increment.
    /// </summary>
    /// <param name="elapsedTime">The amount of time the player used for their move.</param>
    /// <param name="white">True means the player is white.</param>
    /// <returns>If the new value is negative, ie the player has lost on time.</returns>
    public bool UpdateTimer(TimeSpan elapsedTime, bool white) {
        ref TimeSpan activeTime = ref ActiveTime(white);
        activeTime -= elapsedTime;
        if (activeTime < TimeSpan.Zero) return false;
        activeTime += Increment;
        return true;
    }
    
    /// <summary>
    /// Returns the active players Time by reference.
    /// </summary>
    /// <param name="color">True means white</param>
    /// <returns>The Time by reference.</returns>
    public ref TimeSpan ActiveTime(bool color) => ref color ? ref _whiteTime : ref _blackTime;
    public long WhiteTimeMs => (long)WhiteTime.TotalMilliseconds;
    public long BlackTimeMs => (long)BlackTime.TotalMilliseconds;
    public long IncrementMs => (long)Increment.TotalMilliseconds;
}
