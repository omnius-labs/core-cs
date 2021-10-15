using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public partial class SecureConnection
    {
        internal class ConnectionSender : DisposableBase, IConnectionSender
        {
            private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

            private readonly IConnectionSender _sender;
            private readonly IBytesPool _bytesPool;
            private readonly AesGcmEncrypter _encrypter;

            public ConnectionSender(IConnectionSender sender, CryptoAlgorithmType cryptoAlgorithmType, byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool)
            {
                _sender = sender;

                if (cryptoAlgorithmType == CryptoAlgorithmType.Aes_Gcm_256)
                {
                    _encrypter = new AesGcmEncrypter(cryptoKey, nonce, bytesPool);
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
                    _encrypter.Dispose();
                }
            }

            public long TotalBytesSent => _sender.TotalBytesSent;

            public async ValueTask WaitToSendAsync(CancellationToken cancellationToken = default)
            {
                await _sender.WaitToSendAsync(cancellationToken);
            }

            public bool TrySend(Action<IBufferWriter<byte>> action)
            {
                return _sender.TrySend(bufferWriter => _encrypter.Encrypt(action, bufferWriter));
            }

            public async ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
            {
                await _sender.SendAsync(bufferWriter => _encrypter.Encrypt(action, bufferWriter), cancellationToken);
            }
        }

        internal sealed class AesGcmEncrypter : DisposableBase
        {
            private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

            private readonly AesGcm _aes;
            private readonly byte[] _nonce;
            private readonly IBytesPool _bytesPool;

            public AesGcmEncrypter(byte[] cryptoKey, byte[] nonce, IBytesPool bytesPool)
            {
                _aes = new AesGcm(cryptoKey);
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

            public void Encrypt(Action<IBufferWriter<byte>> action, IBufferWriter<byte> bufferWriter)
            {
                using var bytesPipe = new BytesPipe(_bytesPool);
                action.Invoke(bytesPipe.Writer);

                var sequence = bytesPipe.Reader.GetSequence();
                var buffer = _bytesPool.Array.Rent(MaxPayloadLength);

                try
                {
                    while (sequence.Length > 0)
                    {
                        int payloadLength = (int)Math.Min(sequence.Length, MaxPayloadLength);

                        var payloadLengthBytes = bufferWriter.GetSpan(4).Slice(0, 4);
                        BinaryPrimitives.WriteInt32BigEndian(payloadLengthBytes, payloadLength);
                        bufferWriter.Advance(payloadLengthBytes.Length);

                        var plaintext = buffer.AsSpan().Slice(0, payloadLength);
                        sequence.Slice(0, payloadLength).CopyTo(plaintext);
                        sequence = sequence.Slice(payloadLength);

                        var tagAndCiphertextBytes = bufferWriter.GetSpan(TagLength + payloadLength).Slice(0, TagLength + payloadLength);
                        var tag = tagAndCiphertextBytes.Slice(0, TagLength);
                        var ciphertext = tagAndCiphertextBytes.Slice(TagLength);
                        _aes.Encrypt(_nonce, plaintext, ciphertext, tag);
                        bufferWriter.Advance(tagAndCiphertextBytes.Length);

                        Increment(_nonce);
                    }
                }
                finally
                {
                    _bytesPool.Array.Return(buffer);
                }
            }
        }
    }
}
