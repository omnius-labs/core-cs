using Omnius.Core.Base;

namespace Omnius.Core.Base.Pipelines;

public sealed partial class BytesPipe : DisposableBase
{
    private readonly BytesState _state;
    private readonly BytesReader _reader;
    private readonly BytesWriter _writer;

    public BytesPipe()
        : this(BytesPool.Shared)
    {
    }

    public BytesPipe(IBytesPool bytesPool)
    {
        _state = new BytesState(bytesPool);
        _reader = new BytesReader(_state);
        _writer = new BytesWriter(_state);
    }

    protected override void OnDispose(bool disposing)
    {
        if (disposing)
        {
            _state.Dispose();
        }
    }

    public BytesReader Reader => _reader;

    public BytesWriter Writer => _writer;

    public void Reset()
    {
        _reader.Reset();
        _writer.Reset();
    }
}
