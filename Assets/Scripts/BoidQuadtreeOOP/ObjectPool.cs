using System;
using System.Collections.Generic;
public class ObjectPool<T> where T : new()
{
    private Stack<T> _pool;
    private int _maxSize;
    private Func<T> _createFunc;

    public ObjectPool() { }
    public ObjectPool(Func<T> createFunc, int defautCapacity = 50, int maxSize = 500)
    {
        _pool = new Stack<T>(defautCapacity);
        _maxSize = maxSize;
        _createFunc = createFunc;

        for (int i = 0; i < defautCapacity; ++i)
            _pool.Push(_createFunc());
    }

    public ObjectPool<T> Init(Func<T> createFunc, int defautCapacity = 50, int maxSize = 500)
    {
        _pool = new Stack<T>(defautCapacity);
        _maxSize = maxSize;
        _createFunc = createFunc;

        for (int i = 0; i < defautCapacity; ++i)
            _pool.Push(_createFunc());
        return this;
    }

    public T Get()
    {
        T node;
        if (_pool.Count > 0)
        {
            node = _pool.Pop();
        }
        else
        {
            node = _createFunc();
        }
        return node;
    }

    public void Release(T node)
    {
        if (_pool.Count < _maxSize)
            _pool.Push(node);
    }

    public void ReleaseAll(IList<T> values)
    {
        foreach (T i in values)
        {
            if (_pool.Count < _maxSize)
                _pool.Push(i);
        }
    }
}