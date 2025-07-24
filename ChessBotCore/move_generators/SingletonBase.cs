namespace ChessBotCore;

public interface ISingletonBase<out T> {
    public static abstract T Instance { get; }
}

public interface IGeneratorSingleton : ISingletonBase<IMoveGenerator>;