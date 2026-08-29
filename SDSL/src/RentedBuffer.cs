using System.Buffers;

namespace SDSL;

public struct RentedBuffer<T> : IDisposable
{
    private T[] _buffer;
    private readonly int _length;

    public RentedBuffer(int length)
    {
        _buffer = ArrayPool<T>.Shared.Rent(length);
        _length = length;
    }
    
    public int Length => _length;

    public Span<T> AsSpan()
    {
        ThrowIfDisposed();
        return new Span<T>(_buffer, 0, _length);
    }
    
    public void Dispose()
    {
        if (_buffer == null)
            return;
        
        ArrayPool<T>.Shared.Return(_buffer);

        _buffer = null;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_buffer == null, this);
    }
}