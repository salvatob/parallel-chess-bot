using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using ChessBotCore;

namespace Benchmarks;

internal static class BenchmarkSettings {
  public static IConfig Config { get; } = ManualConfig
    .Create(DefaultConfig.Instance)
    .AddJob(Job.Default
      .WithToolchain(InProcessNoEmitToolchain.Instance)
      .WithLaunchCount(1)
      .WithWarmupCount(0)
      .WithIterationCount(5));
}

internal static class BenchmarkPositions {
  public const string InitialPosition = "InitialPosition";

  public static IEnumerable<string> PositionCases => new[] {
    InitialPosition
  };

  public static State GetState(string position) {
    return position switch {
      InitialPosition => State.Initial,
      _ => throw new ArgumentOutOfRangeException(nameof(position), position, "Unknown benchmark position")
    };
  }
}
