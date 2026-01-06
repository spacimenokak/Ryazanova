using System;
using System.Collections;
using System.Collections.Generic;

namespace Lab03.Collections
{
    /// <summary>
    /// Узел двунаправленного связного списка
    /// </summary>
    public class ListNode<T>
    {
        public T Value { get; set; }
        public ListNode<T>? Next { get; set; }
        public ListNode<T>? Previous { get; set; }

        public ListNode(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Двунаправленный связный список, реализующий IList
    /// </summary>
    public class BidirectionalLinkedList<T> : IList<T>
    {
        private ListNode<T>? _head;
        private ListNode<T>? _tail;
        private int _count;
        private int _version;

        public int Count => _count;
        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => GetNodeAtIndex(index).Value;
            set => GetNodeAtIndex(index).Value = value;
        }

        public void Add(T item)
        {
            var newNode = new ListNode<T>(item);
            
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail!.Next = newNode;
                newNode.Previous = _tail;
                _tail = newNode;
            }
            
            _count++;
            _version++;
        }

        public void Clear()
        {
            _head = null;
            _tail = null;
            _count = 0;
            _version++;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < _count)
                throw new ArgumentException("Недостаточно места в целевом массиве");
            
            var current = _head;
            int i = arrayIndex;
            while (current != null)
            {
                array[i++] = current.Value;
                current = current.Next;
            }
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
            var current = _head;
            int index = 0;
            
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, item))
                    return index;
                
                current = current.Next;
                index++;
            }
            
            return -1;
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            if (index == _count)
            {
                Add(item);
                return;
            }
            
            var newNode = new ListNode<T>(item);
            
            if (index == 0)
            {
                newNode.Next = _head;
                _head!.Previous = newNode;
                _head = newNode;
            }
            else
            {
                var nodeAtIndex = GetNodeAtIndex(index);
                var previousNode = nodeAtIndex.Previous!;
                
                previousNode.Next = newNode;
                newNode.Previous = previousNode;
                newNode.Next = nodeAtIndex;
                nodeAtIndex.Previous = newNode;
            }
            
            _count++;
            _version++;
        }

        public bool Remove(T item)
        {
            var node = FindNode(item);
            if (node == null)
                return false;
            
            RemoveNode(node);
            return true;
        }

        public void RemoveAt(int index)
        {
            var node = GetNodeAtIndex(index);
            RemoveNode(node);
        }

        private ListNode<T> GetNodeAtIndex(int index)
        {
            if (index < 0 || index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));
            
            ListNode<T> current;
            if (index < _count / 2)
            {
                // Ищем с начала
                current = _head!;
                for (int i = 0; i < index; i++)
                    current = current.Next!;
            }
            else
            {
                // Ищем с конца
                current = _tail!;
                for (int i = _count - 1; i > index; i--)
                    current = current.Previous!;
            }
            
            return current;
        }

        private ListNode<T>? FindNode(T item)
        {
            var current = _head;
            while (current != null)
            {
                if (EqualityComparer<T>.Default.Equals(current.Value, item))
                    return current;
                current = current.Next;
            }
            return null;
        }

        private void RemoveNode(ListNode<T> node)
        {
            if (node.Previous != null)
                node.Previous.Next = node.Next;
            else
                _head = node.Next;
            
            if (node.Next != null)
                node.Next.Previous = node.Previous;
            else
                _tail = node.Previous;
            
            _count--;
            _version++;
        }

        // Вложенный класс Enumerator
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly BidirectionalLinkedList<T> _list;
            private ListNode<T>? _current;
            private int _index;
            private readonly int _version;

            internal Enumerator(BidirectionalLinkedList<T> list)
            {
                _list = list;
                _current = null;
                _index = -1;
                _version = list._version;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Коллекция была изменена во время перебора");
                
                if (_index == -1)
                {
                    _current = _list._head;
                    _index = 0;
                }
                else
                {
                    _current = _current?.Next;
                    _index++;
                }
                
                return _current != null;
            }

            public void Reset()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Коллекция была изменена во время перебора");
                
                _current = null;
                _index = -1;
            }

            public T Current
            {
                get
                {
                    if (_current == null)
                        throw new InvalidOperationException();
                    return _current.Value;
                }
            }

            object? IEnumerator.Current => Current;
        }
    }
}