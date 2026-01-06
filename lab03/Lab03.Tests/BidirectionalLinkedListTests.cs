using NUnit.Framework;
using Lab03.Collections;
using System.Linq;

namespace Lab03.Tests
{
    [TestFixture]
    public class BidirectionalLinkedListTests
    {
        [Test]
        public void Add_AddsItemsToEnd()
        {
            var list = new BidirectionalLinkedList<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(2));
            Assert.That(list[2], Is.EqualTo(3));
        }

        [Test]
        public void Insert_AtBeginning_Works()
        {
            var list = new BidirectionalLinkedList<int> { 2, 3 };
            list.Insert(0, 1);
            
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(2));
            Assert.That(list[2], Is.EqualTo(3));
        }

        [Test]
        public void Insert_InMiddle_Works()
        {
            var list = new BidirectionalLinkedList<int> { 1, 3 };
            list.Insert(1, 2);
            
            Assert.That(list.Count, Is.EqualTo(3));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(2));
            Assert.That(list[2], Is.EqualTo(3));
        }

        [Test]
        public void Remove_RemovesItem()
        {
            var list = new BidirectionalLinkedList<int> { 1, 2, 3 };
            
            var removed = list.Remove(2);
            
            Assert.That(removed, Is.True);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.EqualTo(1));
            Assert.That(list[1], Is.EqualTo(3));
        }

        [Test]
        public void RemoveAt_Beginning_Works()
        {
            var list = new BidirectionalLinkedList<int> { 1, 2, 3 };
            
            list.RemoveAt(0);
            
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.EqualTo(2));
            Assert.That(list[1], Is.EqualTo(3));
        }

        [Test]
        public void Enumerator_WorksCorrectly()
        {
            var list = new BidirectionalLinkedList<int> { 1, 2, 3 };
            var result = new System.Collections.Generic.List<int>();
            
            foreach (var item in list)
            {
                result.Add(item);
            }
            
            Assert.That(result, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void IndexOf_ReturnsCorrectIndex()
        {
            var list = new BidirectionalLinkedList<int> { 1, 2, 3, 2 };
            
            Assert.That(list.IndexOf(2), Is.EqualTo(1));
            Assert.That(list.IndexOf(4), Is.EqualTo(-1));
        }
    }
}