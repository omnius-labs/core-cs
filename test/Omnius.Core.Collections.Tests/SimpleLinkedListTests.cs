using System;
using System.Collections.Generic;
using System.Linq;
using Omnius.Core.Helpers;
using Omnius.Core.Collections;
using Xunit;

#pragma warning disable xUnit2017

namespace Omnius.Core.DataStructures
{
    public class SimpleLinkedListTests
    {
        private readonly Random _random = new Random();

        [Fact]
        public void SimpleLinkedListTest()
        {
            var linkedList = new SimpleLinkedList<int>
            {
                0
            };
            Assert.True(linkedList.Count == 1);
            Assert.True(linkedList.Contains(0));
            Assert.False(linkedList.Contains(1));

            linkedList.Remove(0);
            Assert.True(linkedList.Count == 0);
            linkedList.Add(1);
            Assert.True(linkedList.Count == 1);
            Assert.True(linkedList.Contains(1));
            Assert.False(linkedList.Contains(0));

            linkedList.Clear();

            var tempList = new List<int>();

            for (int i = 0; i < 1024 * 32; i++)
            {
                int v = _random.Next(0, 256);

                if (_random.Next(0, 2) == 0)
                {
                    linkedList.Add(v);
                    tempList.Add(v);
                }
                else
                {
                    linkedList.Remove(v);
                    tempList.Remove(v);
                }
            }

            {
                tempList.Sort();

                var tempList2 = linkedList.ToList();
                tempList2.Sort();

                Assert.True(CollectionHelper.Equals(tempList, tempList2));
            }
        }
    }
}
