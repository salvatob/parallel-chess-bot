namespace ChessBotCore.Players;

public class Timers {
    public TimeSpan BaseWhiteTime { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan BaseBlackTime { get; init; } = TimeSpan.FromMinutes(5);
    public TimeSpan WhiteTime { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan BlackTime { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan Increment { get; init; } = TimeSpan.FromSeconds(2);

    public TimeSpan ActiveTime(bool color) => color ? WhiteTime : BlackTime;
}
