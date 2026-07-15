using System.Diagnostics.CodeAnalysis;

namespace ChessBotCore.Players;

public class Timers {
    private TimeSpan _baseWhiteTime = TimeSpan.FromMinutes(5);
    private TimeSpan _baseBlackTime = TimeSpan.FromMinutes(5);

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

    public TimeSpan WhiteTime { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan BlackTime { get; set; } = TimeSpan.FromMinutes(5);
    public required TimeSpan Increment { get; init; } = TimeSpan.FromSeconds(2);

    [SetsRequiredMembers]
    public Timers() {
    }

    
    public TimeSpan ActiveTime(bool color) => color ? WhiteTime : BlackTime;
}
