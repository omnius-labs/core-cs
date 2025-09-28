using System.Diagnostics;
using Xunit.Abstractions;

namespace Omnius.Core.Testkit;

public sealed class XunitTraceListener : TraceListener
{
    private readonly ITestOutputHelper _output;

    public static XunitTraceListener Create(ITestOutputHelper output)
    {
        var traceListener = new XunitTraceListener(output);
        Trace.Listeners.Add(traceListener);
        Trace.AutoFlush = true;
        return traceListener;
    }

    public XunitTraceListener(ITestOutputHelper output) => _output = output;

    public override void Write(string? message) => _output.WriteLine(message ?? "");
    public override void WriteLine(string? message) => _output.WriteLine(message ?? "");

    protected override void Dispose(bool disposing)
    {
        Trace.Listeners.Remove(this);
        base.Dispose(disposing);
    }
}
