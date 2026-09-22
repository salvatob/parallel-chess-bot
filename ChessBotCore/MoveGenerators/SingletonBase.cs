namespace ChessBotCore.MoveGenerators;

/// <summary>
///     Requires a class to implement a static property of it's own type, making it a singleton.\
///     Sadly interface cannot enforce disallowing public constructors.
/// </summary>
/// <typeparam name="T">The type which should be a Singleton</typeparam>
public interface ISingletonBase<out T> {
    public static abstract T Instance { get; }
}

public interface IGeneratorSingleton : ISingletonBase<IPieceMoveGenerator>;
