using System;

namespace Omnius.Core.Pipelines;

public interface IActionSubscriber
{
    IDisposable Subscribe(Action action);
}