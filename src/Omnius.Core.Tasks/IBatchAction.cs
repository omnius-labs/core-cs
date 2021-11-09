using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Tasks;

public interface IBatchAction
{
    TimeSpan Interval { get; }

    void Execute();
}