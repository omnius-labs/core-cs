using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using System.Linq;

namespace Omnix.Network.Connection.Multiplexer
{
    /*
    public sealed partial class OmniConnectionMultiplexer<TPriority> : ServiceBase
         where TPriority : IComparable<TPriority>
    {
        private readonly IConnection _connection;
        private readonly BufferPool _bufferPool;

        private Random _random = new Random();

        private Dictionary<ulong, SessionInfo> _sessionInfos;

        private TaskManager _sendTaskManager;
        private TaskManager _receiveTaskManager;

        private AsyncLock _asyncLock = new AsyncLock();

        public OmniConnectionMultiplexer(IConnection connection, BufferPool bufferPool)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (bufferPool == null) throw new ArgumentNullException(nameof(bufferPool));

            _connection = connection;
            _bufferPool = bufferPool;
        }

        private ulong CreateId()
        {
            Span<byte> buffer = stackalloc byte[8];

            for (; ; )
            {
                _random.NextBytes(buffer);
                var id = BinaryPrimitives.ReadUInt64BigEndian(buffer);

                if (!_sessionInfos.ContainsKey(id))
                {
                    return id;
                }
            }
        }

        public TPriority GetPriority(IConnection connection)
        {
            if (connection is SessionConnection sessionConnection)
            {
                if (_sessionInfos.TryGetValue(sessionConnection.Id, out var info))
                {
                    return info.Priority;
                }
            }

            throw new ArgumentException(); 
        }

        private void SetPriority(IConnection connection, TPriority priority)
        {
            if (connection is SessionConnection sessionConnection)
            {
                if (_sessionInfos.TryGetValue(sessionConnection.Id, out var info))
                {
                    info.Priority = priority;

                    return;
                }
            }

            throw new ArgumentException();
        }

        public IConnection Connect(TPriority priority)
        {
            var id = this.CreateId();

            var sessionInfo = new SessionInfo();
            sessionInfo.Id = id;
            sessionInfo.Priority = priority;
            sessionInfo.StatusType = SessionStatusType.Connecting;

            return new SessionConnection(id, this);
        }

        public IConnection Accept(TPriority priority)
        {

        }

        internal void CloseConnection(ulong id)
        {

        }

        public IConnection BaseConnection => _connection;

        public override ServiceStateType StateType { get; }

        internal bool TryEnqueue(ulong id, Action<IBufferWriter<byte>> action)
        {
            if (!_sessionInfos.TryGetValue(id, out var sessionInfo))
            {
                // TODO
                throw new Exception();
            }

            if (!sessionInfo.SendSemaphoreSlim.Wait(0))
            {
                return false;
            }

            var hub = new Hub();
            action.Invoke(hub.Writer);
            hub.Writer.Complete();

            sessionInfo.SendHubQueue.Enqueue(hub);

            return true;
        }

        internal async ValueTask EnqueueAsync(ulong id, Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            if (!_sessionInfos.TryGetValue(id, out var sessionInfo))
            {
                // TODO
                throw new Exception();
            }

            await sessionInfo.SendSemaphoreSlim.WaitAsync(token);

            var hub = new Hub();
            action.Invoke(hub.Writer);
            hub.Writer.Complete();

            sessionInfo.SendHubQueue.Enqueue(hub);
        }

        internal bool TryDequeue(ulong id, Action<ReadOnlySequence<byte>> action)
        {
        }

        internal async ValueTask DequeueAsync(ulong id, Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
        }

        private void SendThread(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var sessionInfo = _sessionInfos.Values.OrderByDescending(n => n.Priority).FirstOrDefault();

                if (sessionInfo == null || sessionInfo.SendHubQueue.Count == 0)
                {
                    Task.Delay(300);

                    continue;
                }

                var targetHub = sessionInfo.SendHubQueue.Dequeue();

                _connection.EnqueueAsync((bufferWriter) =>
                {
                    bufferWriter.Write()
                    targetHub.Reader.
                });
            }
        }

        private void InternalStart()
        {
            _sendTaskManager = new TaskManager(this.SendThread);
            _sendTaskManager.Start();
        }

        private void InternalStop()
        {

        }

        public override async ValueTask Start()
        {
            using (await _asyncLock.LockAsync())
            {

            }
        }

        public override ValueTask Stop()
        {
            throw new NotImplementedException();
        }

        public override ValueTask Restart()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                _connection.Dispose();
            }
        }
    }
    */
}
