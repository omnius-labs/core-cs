using System.Collections.Generic;

namespace Omnius.Core.Pipelines;

public interface IEventPublicher<TResult>
{
    IEnumerable<TResult> Publish();
}