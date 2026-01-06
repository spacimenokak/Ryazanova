using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lab03.Collections
{
    /// <summary>
    /// Элемент словаря (цепочка для разрешения коллизий)
    /// </summary>
    internal class DictionaryEntry<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public DictionaryEntry<TKey, TValue>? Next { get; set; }

        public DictionaryEntry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    /// <summary>
    /// Простая реализация словаря с цепочками для разрешения коллизий
    /// </summary>
    public class SimpleDictionary<TKey, TValue> : IDictionary<TKey, TValue>, 
                                                  IReadOnlyDictionary<TKey, TValue>,
                                                  ICollection<KeyValuePair<TKey, TValue>>,
                                                  IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        private const int DefaultCapacity = 16;
        private const double LoadFactor = 0.75;
        
        private DictionaryEntry<TKey, TValue>?[] _buckets;
        private int _count;
        private int _version;
        
        private readonly IEqualityComparer<TKey> _comparer;

        public SimpleDictionary() : this(DefaultCapacity, EqualityComparer<TKey>.Default) { }

        public SimpleDictionary(int capacity) : this(capacity, EqualityComparer<TKey>.Default) { }

        public SimpleDictionary(IEqualityComparer<TKey> comparer) : this(DefaultCapacity, comparer) { }

        public SimpleDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            
            _comparer = comparer;
            _buckets = new DictionaryEntry<TKey, TValue>[GetPrime(capacity)];
        }

        public TValue this[TKey key]
        {
            get
            {
                var entry = FindEntry(key);
                if (entry == null)
                    throw new KeyNotFoundException($"Ключ '{key}' не найден в словаре.");
                return entry.Value;
            }
            set
            {
                Insert(key, value, true);
            }
        }

        public ICollection<TKey> Keys => new KeyCollection(this);
        public ICollection<TValue> Values => new ValueCollection(this);

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public int Count => _count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, false);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (_count > 0)
            {
                Array.Clear(_buckets, 0, _buckets.Length);
                _count = 0;
            }
            _version++;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var entry = FindEntry(item.Key);
            return entry != null && EqualityComparer<TValue>.Default.Equals(entry.Value, item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) != null;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < _count)
                throw new ArgumentException("Недостаточно места в целевом массиве");
            
            foreach (var pair in this)
            {
                array[arrayIndex++] = pair;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            int bucketIndex = GetBucketIndex(key);
            var current = _buckets[bucketIndex];
            DictionaryEntry<TKey, TValue>? previous = null;

            while (current != null)
            {
                if (_comparer.Equals(current.Key, key))
                {
                    if (previous == null)
                        _buckets[bucketIndex] = current.Next;
                    else
                        previous.Next = current.Next;
                    
                    _count--;
                    _version++;
                    return true;
                }
                
                previous = current;
                current = current.Next;
            }
            
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var entry = FindEntry(item.Key);
            if (entry != null && EqualityComparer<TValue>.Default.Equals(entry.Value, item.Value))
            {
                return Remove(item.Key);
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var entry = FindEntry(key);
            if (entry != null)
            {
                value = entry.Value;
                return true;
            }
            
            value = default!;
            return false;
        }

        private DictionaryEntry<TKey, TValue>? FindEntry(TKey key)
        {
            int bucketIndex = GetBucketIndex(key);
            var current = _buckets[bucketIndex];
            
            while (current != null)
            {
                if (_comparer.Equals(current.Key, key))
                    return current;
                current = current.Next;
            }
            
            return null;
        }

        private void Insert(TKey key, TValue value, bool overwrite)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            
            // Проверяем, нужно ли увеличить емкость
            if (_count >= _buckets.Length * LoadFactor)
            {
                Resize(_buckets.Length * 2);
            }
            
            int bucketIndex = GetBucketIndex(key);
            var current = _buckets[bucketIndex];
            
            // Проверяем, существует ли уже ключ
            while (current != null)
            {
                if (_comparer.Equals(current.Key, key))
                {
                    if (overwrite)
                    {
                        current.Value = value;
                        _version++;
                    }
                    else
                    {
                        throw new ArgumentException($"Элемент с ключом '{key}' уже существует.");
                    }
                    return;
                }
                current = current.Next;
            }
            
            // Добавляем новый элемент в начало цепочки
            var newEntry = new DictionaryEntry<TKey, TValue>(key, value)
            {
                Next = _buckets[bucketIndex]
            };
            _buckets[bucketIndex] = newEntry;
            _count++;
            _version++;
        }

        private void Resize(int newSize)
        {
            newSize = GetPrime(newSize);
            var oldBuckets = _buckets;
            _buckets = new DictionaryEntry<TKey, TValue>[newSize];
            _count = 0;
            
            foreach (var bucket in oldBuckets)
            {
                var current = bucket;
                while (current != null)
                {
                    // Пересчитываем хеш для нового размера
                    int newBucketIndex = GetBucketIndex(current.Key);
                    var newEntry = new DictionaryEntry<TKey, TValue>(current.Key, current.Value)
                    {
                        Next = _buckets[newBucketIndex]
                    };
                    _buckets[newBucketIndex] = newEntry;
                    _count++;
                    
                    current = current.Next;
                }
            }
        }

        private int GetBucketIndex(TKey key)
        {
            int hashCode = _comparer.GetHashCode(key) & 0x7FFFFFFF;
            return hashCode % _buckets.Length;
        }

        private static int GetPrime(int min)
        {
            // Простые числа для размера хеш-таблицы
            int[] primes = {
                3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
                1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
                17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
                187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
                1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
            };
            
            foreach (int prime in primes)
            {
                if (prime >= min) return prime;
            }
            
            // Если нужно число больше максимального в массиве
            for (int i = min | 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i)) return i;
            }
            
            return min;
        }

        private static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            
            int boundary = (int)Math.Sqrt(number);
            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        // Вложенные классы для коллекций ключей и значений
        private sealed class KeyCollection : ICollection<TKey>
        {
            private readonly SimpleDictionary<TKey, TValue> _dictionary;

            public KeyCollection(SimpleDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public int Count => _dictionary._count;
            public bool IsReadOnly => true;

            public void Add(TKey item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Remove(TKey item) => throw new NotSupportedException();

            public bool Contains(TKey item)
            {
                return _dictionary.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (array.Length - arrayIndex < Count)
                    throw new ArgumentException("Недостаточно места в целевом массиве");
                
                foreach (var pair in _dictionary)
                {
                    array[arrayIndex++] = pair.Key;
                }
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return new KeyEnumerator(_dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private sealed class KeyEnumerator : IEnumerator<TKey>
            {
                private readonly SimpleDictionary<TKey, TValue> _dictionary;
                private int _index;
                private DictionaryEntry<TKey, TValue>? _currentEntry;
                private readonly int _version;

                public KeyEnumerator(SimpleDictionary<TKey, TValue> dictionary)
                {
                    _dictionary = dictionary;
                    _version = dictionary._version;
                    _index = -1;
                }

                public TKey Current
                {
                    get
                    {
                        if (_currentEntry == null)
                            throw new InvalidOperationException();
                        return _currentEntry.Key;
                    }
                }

                object IEnumerator.Current => Current!;

                public bool MoveNext()
                {
                    if (_version != _dictionary._version)
                        throw new InvalidOperationException("Коллекция была изменена во время перебора");
                    
                    while (_currentEntry == null || _currentEntry.Next == null)
                    {
                        _index++;
                        if (_index >= _dictionary._buckets.Length)
                            return false;
                        _currentEntry = _dictionary._buckets[_index];
                        if (_currentEntry != null)
                            return true;
                    }
                    
                    _currentEntry = _currentEntry.Next;
                    return true;
                }

                public void Reset()
                {
                    if (_version != _dictionary._version)
                        throw new InvalidOperationException("Коллекция была изменена во время перебора");
                    
                    _index = -1;
                    _currentEntry = null;
                }

                public void Dispose() { }
            }
        }

        private sealed class ValueCollection : ICollection<TValue>
        {
            private readonly SimpleDictionary<TKey, TValue> _dictionary;

            public ValueCollection(SimpleDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public int Count => _dictionary._count;
            public bool IsReadOnly => true;

            public void Add(TValue item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Remove(TValue item) => throw new NotSupportedException();

            public bool Contains(TValue item)
            {
                foreach (var pair in _dictionary)
                {
                    if (EqualityComparer<TValue>.Default.Equals(pair.Value, item))
                        return true;
                }
                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                if (array.Length - arrayIndex < Count)
                    throw new ArgumentException("Недостаточно места в целевом массиве");
                
                foreach (var pair in _dictionary)
                {
                    array[arrayIndex++] = pair.Value;
                }
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new ValueEnumerator(_dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private sealed class ValueEnumerator : IEnumerator<TValue>
            {
                private readonly SimpleDictionary<TKey, TValue> _dictionary;
                private int _index;
                private DictionaryEntry<TKey, TValue>? _currentEntry;
                private readonly int _version;

                public ValueEnumerator(SimpleDictionary<TKey, TValue> dictionary)
                {
                    _dictionary = dictionary;
                    _version = dictionary._version;
                    _index = -1;
                }

                public TValue Current
                {
                    get
                    {
                        if (_currentEntry == null)
                            throw new InvalidOperationException();
                        return _currentEntry.Value;
                    }
                }

                object IEnumerator.Current => Current!;

                public bool MoveNext()
                {
                    if (_version != _dictionary._version)
                        throw new InvalidOperationException("Коллекция была изменена во время перебора");
                    
                    while (_currentEntry == null || _currentEntry.Next == null)
                    {
                        _index++;
                        if (_index >= _dictionary._buckets.Length)
                            return false;
                        _currentEntry = _dictionary._buckets[_index];
                        if (_currentEntry != null)
                            return true;
                    }
                    
                    _currentEntry = _currentEntry.Next;
                    return true;
                }

                public void Reset()
                {
                    if (_version != _dictionary._version)
                        throw new InvalidOperationException("Коллекция была изменена во время перебора");
                    
                    _index = -1;
                    _currentEntry = null;
                }

                public void Dispose() { }
            }
        }

        // Вложенный класс Enumerator для словаря
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
        {
            private readonly SimpleDictionary<TKey, TValue> _dictionary;
            private int _index;
            private DictionaryEntry<TKey, TValue>? _currentEntry;
            private readonly int _version;

            internal Enumerator(SimpleDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = -1;
                _currentEntry = null;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    if (_currentEntry == null)
                        throw new InvalidOperationException();
                    return new KeyValuePair<TKey, TValue>(_currentEntry.Key, _currentEntry.Value);
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                    throw new InvalidOperationException("Коллекция была изменена во время перебора");
                
                while (_currentEntry == null || _currentEntry.Next == null)
                {
                    _index++;
                    if (_index >= _dictionary._buckets.Length)
                        return false;
                    _currentEntry = _dictionary._buckets[_index];
                    if (_currentEntry != null)
                        return true;
                }
                
                _currentEntry = _currentEntry.Next;
                return true;
            }

            public void Reset()
            {
                if (_version != _dictionary._version)
                    throw new InvalidOperationException("Коллекция была изменена во время перебора");
                
                _index = -1;
                _currentEntry = null;
            }

            public void Dispose() { }
        }
    }
}