namespace Omnius.Core.Serialization
{
    // TODO
    // FileConverter
    // 汎用的なファイルのインポート&エクスポート機能を実装する
    // public static class Converter
    // {
    //     private enum ConvertCompressionAlgorithm
    //     {
    //         None = 0,
    //         Deflate = 1,
    //     }

    //     private static readonly BufferPool _bytesPool = BufferPool.Instance;

    //     private static Stream ToStream<T>(int version, MessageBase<T> item)
    //             where T : MessageBase<T>
    //     {
    //         Stream stream = null;

    //         try
    //         {
    //             stream = new RangeStream(item.Export(_bytesPool));

    //             var dic = new Dictionary<byte, Stream>();

    //             try
    //             {
    //                 stream.Seek(0, SeekOrigin.Begin);

    //                 RecyclableMemoryStream deflateBufferStream = null;

    //                 try
    //                 {
    //                     deflateBufferStream = new RecyclableMemoryStream(_bytesPool);

    //                     using (var deflateStream = new DeflateStream(deflateBufferStream, CompressionMode.Compress, true))
    //                     using (var safeBuffer = _bytesPool.CreateSafeBuffer(1024 * 4))
    //                     {
    //                         int length;

    //                         while ((length = stream.Read(safeBuffer.Value, 0, safeBuffer.Value.Length)) > 0)
    //                         {
    //                             deflateStream.Write(safeBuffer.Value, 0, length);
    //                         }
    //                     }

    //                     deflateBufferStream.Seek(0, SeekOrigin.Begin);

    //                     dic.Add((byte)ConvertCompressionAlgorithm.Deflate, deflateBufferStream);
    //                 }
    //                 catch (Exception)
    //                 {
    //                     if (deflateBufferStream != null)
    //                     {
    //                         deflateBufferStream.Dispose();
    //                     }
    //                 }
    //             }
    //             catch (Exception)
    //             {

    //             }

    //             dic.Add((byte)ConvertCompressionAlgorithm.None, stream);

    //             var list = dic.ToList();

    //             list.Sort((x, y) =>
    //             {
    //                 int c = x.Value.Length.CompareTo(y.Value.Length);
    //                 if (c != 0) return c;

    //                 return x.Key.CompareTo(y.Key);
    //             });

    //             for (int i = 1; i < list.Count; i++)
    //             {
    //                 list[i].Value.Dispose();
    //             }

    //             var headerStream = new RecyclableMemoryStream(_bytesPool);
    //             Varint.SetUInt64((uint)version, headerStream);
    //             Varint.SetUInt64(list[0].Key, headerStream);

    //             var dataStream = new UniteStream(headerStream, list[0].Value);

    //             var crcStream = new MemoryStream(Crc32_Castagnoli.Compute(new NeverCloseStream(dataStream)));
    //             return new UniteStream(dataStream, crcStream);
    //         }
    //         catch (Exception ex)
    //         {
    //             if (stream != null)
    //             {
    //                 stream.Dispose();
    //             }

    //             throw new ArgumentException(ex.Message, ex);
    //         }
    //     }

    //     private static T FromStream<T>(int version, Stream stream)
    //         where T : MessageBase<T>
    //     {
    //         try
    //         {
    //             stream.Seek(0, SeekOrigin.Begin);

    //             // Check
    //             {
    //                 var verifyCrc = Crc32_Castagnoli.Compute(new RangeStream(stream, 0, stream.Length - 4, true));
    //                 var orignalCrc = new byte[4];

    //                 using (var crcStream = new RangeStream(stream, stream.Length - 4, 4, true))
    //                 {
    //                     crcStream.Read(orignalCrc, 0, orignalCrc.Length);
    //                 }

    //                 if (!BytesOperations.SequenceEqual(verifyCrc, orignalCrc)) throw new ArgumentException("Crc Error");
    //             }

    //             stream.Seek(0, SeekOrigin.Begin);

    //             if (version != (int)Varint.GetUInt64(stream)) throw new ArgumentException("version");
    //             int type = (int)Varint.GetUInt64(stream);

    //             using (var dataStream = new RangeStream(stream, stream.Position, stream.Length - stream.Position - 4, true))
    //             {
    //                 if (type == (int)ConvertCompressionAlgorithm.None)
    //                 {
    //                     return MessageBase<T>.Import(dataStream, _bytesPool);
    //                 }
    //                 else if (type == (int)ConvertCompressionAlgorithm.Deflate)
    //                 {
    //                     using (var deflateBufferStream = new RecyclableMemoryStream(_bytesPool))
    //                     {
    //                         using (var deflateStream = new DeflateStream(dataStream, CompressionMode.Decompress, true))
    //                         using (var safeBuffer = _bytesPool.CreateSafeBuffer(1024 * 4))
    //                         {
    //                             int length;

    //                             while ((length = deflateStream.Read(safeBuffer.Value, 0, safeBuffer.Value.Length)) > 0)
    //                             {
    //                                 deflateBufferStream.Write(safeBuffer.Value, 0, length);

    //                                 if (deflateBufferStream.Length > 1024 * 1024 * 32) throw new Exception("too large");
    //                             }
    //                         }

    //                         deflateBufferStream.Seek(0, SeekOrigin.Begin);

    //                         return MessageBase<T>.Import(deflateBufferStream, _bytesPool);
    //                     }
    //                 }
    //                 else
    //                 {
    //                     throw new ArgumentException("ArgumentException");
    //                 }
    //             }
    //         }
    //         catch (Exception e)
    //         {
    //             throw new ArgumentException(e.Message, e);
    //         }
    //         finally
    //         {
    //             if (stream != null)
    //             {
    //                 stream.Dispose();
    //             }
    //         }
    //     }

    //     public static Stream ToDigitalSignatureStream(DigitalSignature item)
    //     {
    //         if (item == null) throw new ArgumentNullException(nameof(item));

    //         try
    //         {
    //             return Converter.ToStream<DigitalSignature>(0, item);
    //         }
    //         catch (Exception)
    //         {
    //             return null;
    //         }
    //     }

    //     public static DigitalSignature FromDigitalSignatureStream(Stream stream)
    //     {
    //         if (stream == null) throw new ArgumentNullException(nameof(stream));

    //         try
    //         {
    //             return Converter.FromStream<DigitalSignature>(0, stream);
    //         }
    //         catch (Exception)
    //         {
    //             return null;
    //         }
    //     }

    //     public static Stream ToCertificateStream(Certificate item)
    //     {
    //         if (item == null) throw new ArgumentNullException(nameof(item));

    //         try
    //         {
    //             return Converter.ToStream<Certificate>(0, item);
    //         }
    //         catch (Exception)
    //         {
    //             return null;
    //         }
    //     }

    //     public static Certificate FromCertificateStream(Stream stream)
    //     {
    //         if (stream == null) throw new ArgumentNullException(nameof(stream));

    //         try
    //         {
    //             return Converter.FromStream<Certificate>(0, stream);
    //         }
    //         catch (Exception)
    //         {
    //             return null;
    //         }
    //     }
    // }
}
