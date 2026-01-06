using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;

namespace Lab02.Tests
{
    [TestFixture]
    public class CollectionsTests
    {
        private const int N = 1000;

        [Test]
        public void List_Add_Remove_Search_Works()
        {
            var list = new List<int>();

            // Добавление
            for (int i = 0; i < N; i++)
                list.Add(i);

            Assert.That(list.Count, Is.EqualTo(N));

            // Поиск и доступ по индексу
            Assert.That(list.Contains(10), Is.True);
            Assert.That(list[10], Is.EqualTo(10));

            // Удаление из начала
            list.RemoveAt(0);
            Assert.That(list.Count, Is.EqualTo(N - 1));

            // Удаление из конца
            list.RemoveAt(list.Count - 1);
            Assert.That(list.Count, Is.EqualTo(N - 2));
        }

        [Test]
        public void LinkedList_Add_Remove_Search_Works()
        {
            var list = new LinkedList<int>();

            for (int i = 0; i < N; i++)
                list.AddLast(i);

            Assert.That(list.Count, Is.EqualTo(N));
            Assert.That(list.Find(10), Is.Not.Null);

            list.RemoveFirst();
            Assert.That(list.Count, Is.EqualTo(N - 1));

            list.RemoveLast();
            Assert.That(list.Count, Is.EqualTo(N - 2));
        }

        [Test]
        public void Queue_Enqueue_Dequeue_Search_Works()
        {
            var q = new Queue<int>();

            for (int i = 0; i < N; i++)
                q.Enqueue(i);

            Assert.That(q.Count, Is.EqualTo(N));
            Assert.That(q.Contains(10), Is.True);

            var first = q.Dequeue();
            Assert.That(first, Is.EqualTo(0));
            Assert.That(q.Count, Is.EqualTo(N - 1));
        }

        [Test]
        public void Stack_Push_Pop_Search_Works()
        {
            var s = new Stack<int>();

            for (int i = 0; i < N; i++)
                s.Push(i);

            Assert.That(s.Count, Is.EqualTo(N));
            Assert.That(s.Contains(10), Is.True);

            var top = s.Pop();
            Assert.That(top, Is.EqualTo(N - 1));
            Assert.That(s.Count, Is.EqualTo(N - 1));
        }

        [Test]
        public void ImmutableList_Add_Remove_Search_Works()
        {
            var list = ImmutableList<int>.Empty;

            for (int i = 0; i < N; i++)
                list = list.Add(i);

            Assert.That(list.Count, Is.EqualTo(N));
            Assert.That(list.Contains(10), Is.True);
            Assert.That(list[10], Is.EqualTo(10));

            list = list.RemoveAt(0);
            Assert.That(list.Count, Is.EqualTo(N - 1));

            list = list.RemoveAt(list.Count - 1);
            Assert.That(list.Count, Is.EqualTo(N - 2));
        }
    }
}