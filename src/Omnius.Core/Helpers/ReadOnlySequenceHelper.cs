using System.Buffers;

namespace Omnius.Core.Helpers;

public static class ReadOnlySequenceHelper
{
    public static IEnumerable<ReadOnlyMemory<T>> ToReadOnlyMemories<T>(ReadOnlySequence<T> sequence)
    {
        var memories = new List<ReadOnlyMemory<T>>();
        foreach (var memory in sequence)
        {
            memories.Add(memory);
        }

        return memories;
    }

    public static ReadOnlySequence<T> Create<T>(IEnumerable<ReadOnlyMemory<T>> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (!source.Any()) return ReadOnlySequence<T>.Empty;

        var totalLength = source.Sum(s => s.Length);
        var lastMemory = source.Last();
        var remainMemories = source.Reverse().Skip(1);

        CustomReadOnlySequenceSegment<T> firstSegment, lastSegment;
        firstSegment = new CustomReadOnlySequenceSegment<T>(lastMemory, null, totalLength - lastMemory.Length);
        lastSegment = firstSegment;

        foreach (var currentElement in remainMemories)
        {
            firstSegment = new CustomReadOnlySequenceSegment<T>(currentElement, firstSegment, firstSegment.RunningIndex - currentElement.Length);
        }

        var sequence = new ReadOnlySequence<T>(firstSegment, 0, lastSegment, lastSegment.Memory.Length);
        return sequence;
    }

    private class CustomReadOnlySequenceSegment<T> : ReadOnlySequenceSegment<T>
    {
        public CustomReadOnlySequenceSegment(ReadOnlyMemory<T> memory, CustomReadOnlySequenceSegment<T>? next, long runningIndex)
        {
            this.Memory = memory;
            this.Next = next;
            this.RunningIndex = runningIndex;
        }
    }
}
