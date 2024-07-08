using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal;

public partial class SecureConnection
{
    internal class ConnectionReceiver : DisposableBase, IConnectionReceiver
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConnectionReceiver _receiver;
        private readonly int _maxReceiveByteCount;
        private readonly IBytesPool _bytesPool;
        private readonly AesGcmDecrypter _decrypter;

        public ConnectionReceiver(IConnectionReceiver receiver, CryptoAlgorithmType cryptoAlgorithmType, int maxReceiveByteCount, byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool)
        {
            _receiver = receiver;
            _maxReceiveByteCount = maxReceiveByteCount;

            if (cryptoAlgorithmType == CryptoAlgorithmType.Aes_Gcm_256)
            {
                _decrypter = new AesGcmDecrypter(cryptoKey, nonce, bytesPool);
            }
            else
            {
                throw new NotSupportedException(nameof(cryptoAlgorithmType));
            }

            _bytesPool = bytesPool;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _decrypter.Dispose();
            }
        }

        public long TotalBytesReceived => _receiver.TotalBytesReceived;

        public async ValueTask WaitToReceiveAsync(CancellationToken cancellationToken = default)
        {
            await _receiver.WaitToReceiveAsync(cancellationToken);
        }

        public bool TryReceive(Action<ReadOnlySequence<byte>> action)
        {
            return _receiver.TryReceive(sequence => _decrypter.Decrypt(sequence, action));
        }

        public async ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default)
        {
            await _receiver.ReceiveAsync(sequence => _decrypter.Decrypt(sequence, action), cancellationToken);
        }
    }

    internal sealed class AesGcmDecrypter : DisposableBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly AesGcm _aes;
        private readonly byte[] _nonce;
        private readonly IBytesPool _bytesPool;

        public AesGcmDecrypter(byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool)
        {
            _aes = new AesGcm(cryptoKey, TagLength);
            _nonce = nonce;
            _bytesPool = bytesPool;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _aes.Dispose();
            }
        }

        public void Decrypt(ReadOnlySequence<byte> sequence, Action<ReadOnlySequence<byte>> action)
        {
            var bytesPipe = new BytesPipe(_bytesPool);

            Span<byte> payloadLengthBytes = stackalloc byte[4];
            Span<byte> tag = stackalloc byte[TagLength];
            var buffer = _bytesPool.Array.Rent(MaxPayloadLength);

            try
            {
                while (sequence.Length > 4 + TagLength)
                {
                    sequence.Slice(0, payloadLengthBytes.Length).CopyTo(payloadLengthBytes);
                    sequence = sequence.Slice(payloadLengthBytes.Length);
                    var payloadLength = BinaryPrimitives.ReadInt32BigEndian(payloadLengthBytes);

                    if (payloadLength > MaxPayloadLength) throw new OmniSecureConnectionException("payload too large");

                    sequence.Slice(0, tag.Length).CopyTo(tag);
                    sequence = sequence.Slice(tag.Length);

                    var ciphertext = buffer.AsSpan()[..payloadLength];
                    sequence.Slice(0, ciphertext.Length).CopyTo(ciphertext);
                    sequence = sequence.Slice(ciphertext.Length);

                    var plaintext = bytesPipe.Writer.GetSpan(payloadLength)[..payloadLength];
                    _aes.Decrypt(_nonce, ciphertext, tag, plaintext);
                    bytesPipe.Writer.Advance(plaintext.Length);

                    Increment(_nonce);
                }
            }
            finally
            {
                _bytesPool.Array.Return(buffer);
            }

            action.Invoke(bytesPipe.Reader.GetSequence());
        }
    }
}
