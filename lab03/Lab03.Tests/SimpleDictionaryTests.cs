using NUnit.Framework;
using Lab03.Collections;
using System;
using System.Linq;

namespace Lab03.Tests
{
    [TestFixture]
    public class SimpleDictionaryTests
    {
        [Test]
        public void Add_AddsKeyValuePair()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("one", 1);
            dict.Add("two", 2);
            
            Assert.That(dict.Count, Is.EqualTo(2));
            Assert.That(dict["one"], Is.EqualTo(1));
            Assert.That(dict["two"], Is.EqualTo(2));
        }

        [Test]
        public void Add_DuplicateKey_ThrowsException()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("key", 1);
            
            Assert.Throws<ArgumentException>(() => dict.Add("key", 2));
        }

        [Test]
        public void Indexer_GetAndSet_Works()
        {
            var dict = new SimpleDictionary<string, int>();
            dict["one"] = 1;
            dict["two"] = 2;
            
            Assert.That(dict["one"], Is.EqualTo(1));
            Assert.That(dict["two"], Is.EqualTo(2));
            
            dict["one"] = 11;
            Assert.That(dict["one"], Is.EqualTo(11));
        }

        [Test]
        public void Indexer_GetNonExistentKey_ThrowsException()
        {
            var dict = new SimpleDictionary<string, int>();
            
            Assert.Throws<KeyNotFoundException>(() => _ = dict["nonexistent"]);
        }

        [Test]
        public void ContainsKey_ReturnsCorrectValue()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("key", 1);
            
            Assert.That(dict.ContainsKey("key"), Is.True);
            Assert.That(dict.ContainsKey("other"), Is.False);
        }

        [Test]
        public void Remove_RemovesKey()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("key1", 1);
            dict.Add("key2", 2);
            
            var removed = dict.Remove("key1");
            
            Assert.That(removed, Is.True);
            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(dict.ContainsKey("key1"), Is.False);
            Assert.That(dict.ContainsKey("key2"), Is.True);
        }

        [Test]
        public void TryGetValue_WorksCorrectly()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("key", 42);
            
            Assert.That(dict.TryGetValue("key", out int value), Is.True);
            Assert.That(value, Is.EqualTo(42));
            
            Assert.That(dict.TryGetValue("nonexistent", out _), Is.False);
        }

        [Test]
        public void Keys_CollectionContainsAllKeys()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("c", 3);
            
            var keys = dict.Keys.ToList();
            
            Assert.That(keys.Count, Is.EqualTo(3));
            Assert.That(keys, Contains.Item("a"));
            Assert.That(keys, Contains.Item("b"));
            Assert.That(keys, Contains.Item("c"));
        }

        [Test]
        public void Values_CollectionContainsAllValues()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("c", 3);
            
            var values = dict.Values.ToList();
            
            Assert.That(values.Count, Is.EqualTo(3));
            Assert.That(values, Contains.Item(1));
            Assert.That(values, Contains.Item(2));
            Assert.That(values, Contains.Item(3));
        }

        [Test]
        public void Enumerator_EnumeratesAllPairs()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            
            var pairs = dict.ToList();
            
            Assert.That(pairs.Count, Is.EqualTo(2));
            Assert.That(pairs.Any(p => p.Key == "a" && p.Value == 1), Is.True);
            Assert.That(pairs.Any(p => p.Key == "b" && p.Value == 2), Is.True);
        }

        [Test]
        public void Clear_RemovesAllItems()
        {
            var dict = new SimpleDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            
            dict.Clear();
            
            Assert.That(dict.Count, Is.EqualTo(0));
            Assert.That(dict.ContainsKey("a"), Is.False);
        }

        [Test]
        public void Dictionary_ResizesWhenNeeded()
        {
            var dict = new SimpleDictionary<int, string>(4);
            
            // Добавим больше элементов, чем начальная емкость
            for (int i = 0; i < 10; i++)
            {
                dict.Add(i, $"value{i}");
            }
            
            Assert.That(dict.Count, Is.EqualTo(10));
            for (int i = 0; i < 10; i++)
            {
                Assert.That(dict.ContainsKey(i), Is.True);
            }
        }
    }
}