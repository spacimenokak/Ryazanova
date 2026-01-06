using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab03.Collections
{
    /// <summary>
    /// Простая реализация списка с динамическим массивом
    /// </summary>
    public class SimpleList<T> : IList<T>, ICollection<T>, IEnumerable<T>
    {
        private T[] _items;
        private int _size;
        private int _version;
        
        private const int DefaultCapacity = 4;
        private static readonly T[] EmptyArray = Array.Empty<T>();

        public SimpleList()
        {
            _items = EmptyArray;
        }

        public SimpleList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            
            _items = capacity == 0 ? EmptyArray : new T[capacity];
        }

        public SimpleList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            
            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count == 0)
                {
                    _items = EmptyArray;
                }
                else
                {
                    _items = new T[count];
                    c.CopyTo(_items, 0);
                    _size = count;
                }
            }
            else
            {
                _items = EmptyArray;
                foreach (var item in collection)
                {
                    Add(item);
                }
            }
        }

        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _size)
                    throw new ArgumentOutOfRangeException(nameof(value));
                
                if (value != _items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (_size > 0)
                        {
                            Array.Copy(_items, newItems, _size);
                        }
                        _items = newItems;
                    }
                    else
                    {
                        _items = EmptyArray;
                    }
                }
            }
        }

        public int Count => _size;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _size)
                    throw new ArgumentOutOfRangeException(nameof(index));
                _items[index] = value;
                _version++;
            }
        }

        public void Add(T item)
        {
            if (_size == _items.Length)
                EnsureCapacity(_size + 1);
            
            _items[_size++] = item;
            _version++;
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }
            _version++;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < _size)
                throw new ArgumentException("Недостаточно места в целевом массиве");
            
            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _size);
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > _size)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            if (_size == _items.Length)
                EnsureCapacity(_size + 1);
            
            if (index < _size)
            {
                Array.Copy(_items, index, _items, index + 1, _size - index);
            }
            
            _items[index] = item;
            _size++;
            _version++;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            _size--;
            if (index < _size)
            {
                Array.Copy(_items, index + 1, _items, index, _size - index);
            }
            _items[_size] = default!;
            _version++;
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;
                if (newCapacity < min) newCapacity = min;
                Capacity = newCapacity;
            }
        }

        // Вложенный класс Enumerator для поддержки foreach
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly SimpleList<T> _list;
            private int _index;
            private readonly int _version;
            private T? _current;

            internal Enumerator(SimpleList<T> list)
            {
                _list = list;
                _index = 0;
                _version = list._version;
                _current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Коллекция была изменена во время перебора");
                
                if (_index < _list._size)
                {
                    _current = _list._items[_index];
                    _index++;
                    return true;
                }
                
                _current = default;
                return false;
            }

            public void Reset()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Коллекция была изменена во время перебора");
                
                _index = 0;
                _current = default;
            }

            public T Current => _current!;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index > _list._size)
                        throw new InvalidOperationException();
                    return Current;
                }
            }
        }
    }
}