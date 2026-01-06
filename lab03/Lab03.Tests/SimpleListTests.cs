using NUnit.Framework;
using Lab03.Collections;
using System;
using System.Linq;

namespace Lab03.Tests
{
    [TestFixture]
    public class SimpleListTests
    {
        [Test]
        public void Constructor_WithCapacity_CreatesListWithSpecifiedCapacity()
        {
            var list = new SimpleList<int>(10);
            Assert.That(list.Capacity, Is.EqualTo(10));
        }

        [Test]
        public void Add_AddsItemToEnd()
        {
            var list = new SimpleList<int>();
            list.Add(1);
            list.Add(2);
            
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(2));
        }

        [Test]
        public void Indexer_GetAndSet_WorksCorrectly()
        {
            var list = new SimpleList<int> { 1, 2, 3 };
            
            Assert.That(list[1], Is.EqualTo(2));
            
            list[1] = 99;
            Assert.That(list[1], Is.EqualTo(99));
        }

        [Test]
        public void Indexer_Set_OutOfRange_ThrowsException()
        {
            var list = new SimpleList<int> { 1, 2, 3 };
            
            Assert.Throws<ArgumentOutOfRangeException>(() => 
            {
                var _ = list[10];
            });
            
            Assert.Throws<ArgumentOutOfRangeException>(() => 
            {
                list[10] = 99;
            });
        }

        [Test]
        public void Contains_ReturnsTrueForExistingItem()
        {
            var list = new SimpleList<int> { 1, 2, 3 };
            
            Assert.That(list.Contains(2), Is.True);
            Assert.That(list.Contains(99), Is.False);
        }

        [Test]
        public void IndexOf_ReturnsCorrectIndex()
        {
            var list = new SimpleList<int> { 1, 2, 3, 2 };
            
            Assert.That(list.IndexOf(2), Is.EqualTo(1));
            Assert.That(list.IndexOf(99), Is.EqualTo(-1));
        }

        [Test]
        public void Insert_InsertsItemAtSpecifiedPosition()
        {
            var list = new SimpleList<int> { 1, 3 };
            list.Insert(1, 2);
            
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(2));
            Assert.That(list[2], Is.EqualTo(3));
        }

        [Test]
        public void Remove_RemovesFirstOccurrence()
        {
            var list = new SimpleList<int> { 1, 2, 3, 2 };
            
            var removed = list.Remove(2);
            
            Assert.That(removed, Is.True);
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(3));
            Assert.That(list[2], Is.EqualTo(2));
        }

        [Test]
        public void RemoveAt_RemovesItemAtIndex()
        {
            var list = new SimpleList<int> { 1, 2, 3 };
            
            list.RemoveAt(1);
            
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(3));
        }

        [Test]
        public void Clear_RemovesAllItems()
        {
            var list = new SimpleList<int> { 1, 2, 3 };
            
            list.Clear();
            
            Assert.That(list.Count, Is.EqualTo(0));
            Assert.That(list.Capacity, Is.GreaterThan(0));
        }

        [Test]
        public void CopyTo_CopiesElementsToArray()
        {
            var list = new SimpleList<int> { 1, 2, 3 };
            var array = new int[5];
            
            list.CopyTo(array, 1);
            
            Assert.That(array[0], Is.EqualTo(0));
            Assert.That(array[1], Is.EqualTo(1));
            Assert.That(array[2], Is.EqualTo(2));
            Assert.That(array[3], Is.EqualTo(3));
            Assert.That(array[4], Is.EqualTo(0));
        }

        [Test]
        public void Enumerator_EnumeratesAllItems()
        {
            var list = new SimpleList<int> { 1, 2, 3 };
            var result = new System.Collections.Generic.List<int>();
            
            foreach (var item in list)
            {
                result.Add(item);
            }
            
            Assert.That(result, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void Capacity_GrowsWhenNeeded()
        {
            var list = new SimpleList<int>(2);
            
            Assert.That(list.Capacity, Is.EqualTo(2));
            
            list.Add(1);
            list.Add(2);
            list.Add(3); // Должно вызвать увеличение емкости
            
            Assert.That(list.Capacity, Is.GreaterThanOrEqualTo(3));
        }
    }
}