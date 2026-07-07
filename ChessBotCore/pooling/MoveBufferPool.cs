namespace ChessBotCore.pooling;

// public sealed class ListPool<T>
public class MoveBufferPool {

    private readonly Stack<List<Move>> _pool = new();

    public List<Move> Get()
    {
        return _pool.Count > 0
            ? _pool.Pop()
            : new List<Move>();
    }

    public void Return(List<Move> list)
    {
        list.Clear();
        _pool.Push(list);
    }
}