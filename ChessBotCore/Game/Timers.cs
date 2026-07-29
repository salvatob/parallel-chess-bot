using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Game;

public class Timers {
    private readonly TimeSpan _baseWhiteTime = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _baseBlackTime = TimeSpan.FromMinutes(5);

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

    public TimeSpan WhiteTime { get; set; }
    public TimeSpan BlackTime { get; set; }
    public required TimeSpan Increment { get; init; }

    [SetsRequiredMembers]
    public Timers() {}

    public static Timers Default => new Timers() {
        BaseWhiteTime = TimeSpan.FromMinutes(5),
        BaseBlackTime = TimeSpan.FromMinutes(5),
        Increment = TimeSpan.FromSeconds(2)
    };
    
    public TimeSpan ActiveTime(bool color) => color ? WhiteTime : BlackTime;
    public long WhiteTimeMs => (long)WhiteTime.TotalMilliseconds;
    public long BlackTimeMs => (long)BlackTime.TotalMilliseconds;
    public long IncrementMs => (long)Increment.TotalMilliseconds;
}
