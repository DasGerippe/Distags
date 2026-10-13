using System.Collections;

namespace Distags.WebApp.Collections;

public class FeedBuffer<T> : IEnumerable<T>
{
    private readonly T[] _Array;

    private int _Offset;

    private int _Version;

    public int Capacity { get; }

    public int Count { get; private set; }

    public FeedBuffer(int capacity)
    {
        _Array = new T[capacity];
        Capacity = capacity;
    }

    public void Push(T item)
    {
        if (Count < Capacity)
        {
            _Array[GetCorrectedIndex(_Offset + Count)] = item;
            Count++;
        }
        else
        {
            _Array[_Offset] = item;
            _Offset = GetCorrectedIndex(_Offset + 1);
        }

        _Version++;
    }

    public T? FirstOrDefault()
    {
        return Count > 0
            ? _Array[GetCorrectedIndex(_Offset + Count - 1)]
            : default;
    }

    public T? LastOrDefault()
    {
        return Count > 0
            ? _Array[_Offset]
            : default;
    }

    private int GetCorrectedIndex(int index)
    {
        if (index >= Capacity)
            return index - Capacity;
        if (index < 0)
            return index + Capacity;
        return index;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private struct Enumerator : IEnumerator<T>
    {
        private readonly FeedBuffer<T> _StackBuffer;
        private readonly int _Version;

        private int _CurrentIndex;

        public T Current { get; private set; } = default!;

        object? IEnumerator.Current => Current;

        public Enumerator(FeedBuffer<T> stackBuffer)
        {
            _StackBuffer = stackBuffer;
            _Version = stackBuffer._Version;
            _CurrentIndex = stackBuffer.Count;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_StackBuffer._Version != _Version)
                ThrowModifiedException();

            _CurrentIndex--;
            if (_CurrentIndex < 0)
                return false;

            Current = _StackBuffer._Array[_StackBuffer.GetCorrectedIndex(_StackBuffer._Offset + _CurrentIndex)];
            return true;
        }

        public void Reset()
        {
            if (_StackBuffer._Version != _Version)
                ThrowModifiedException();

            _CurrentIndex = _StackBuffer.Count;
            Current = default!;
        }

        private void ThrowModifiedException()
        {
            throw new InvalidOperationException($"The enumerated {nameof(FeedBuffer<T>)} has been modified externally.");
        }
    }
}
