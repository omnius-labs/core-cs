using System;
using System.Collections.Generic;

namespace Omnius.Core.Extensions
{
    public static class LinkedListExtensions
    {
        // http://stackoverflow.com/questions/8195242/removing-from-a-linkedlist
        public static int RemoveAll<T>(this LinkedList<T> list, Predicate<T> match)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (match == null) throw new ArgumentNullException(nameof(match));

            int count = 0;
            var node = list.First;

            while (node != null)
            {
                var next = node.Next;

                if (match(node.Value))
                {
                    list.Remove(node);
                    count++;
                }

                node = next;
            }

            return count;
        }
    }
}
