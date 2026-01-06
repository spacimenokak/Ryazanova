using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Lab02
{
    public class OperationResult
    {
        public string CollectionName { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public double AverageMilliseconds { get; set; }
    }

    public class CollectionBenchmark
    {
        private const int N = 100_000;
        private const int Iterations = 5;
        private readonly Random _random = new Random(42);

        public List<OperationResult> RunAll()
        {
            var results = new List<OperationResult>();

            results.AddRange(BenchmarkList());
            results.AddRange(BenchmarkLinkedList());
            results.AddRange(BenchmarkQueue());
            results.AddRange(BenchmarkStack());
            results.AddRange(BenchmarkImmutableList());

            return results;
        }

        private double Measure(Action action)
        {
            var sw = new Stopwatch();
            long totalTicks = 0;

            for (int i = 0; i < Iterations; i++)
            {
                sw.Restart();
                action();
                sw.Stop();
                totalTicks += sw.ElapsedTicks;
            }

            return totalTicks / (double)Iterations / TimeSpan.TicksPerMillisecond;
        }

        private IEnumerable<OperationResult> BenchmarkList()
        {
            var results = new List<OperationResult>();
            var list = new List<int>();

            // заполнение
            for (int i = 0; i < N; i++)
                list.Add(i);

            results.Add(new OperationResult
            {
                CollectionName = "List",
                Operation = "Add(end)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new List<int>(list);
                    local.Add(N + 1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "List",
                Operation = "Insert(begin)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new List<int>(list);
                    local.Insert(0, -1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "List",
                Operation = "Insert(middle)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new List<int>(list);
                    local.Insert(local.Count / 2, -1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "List",
                Operation = "RemoveAt(begin)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new List<int>(list);
                    local.RemoveAt(0);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "List",
                Operation = "RemoveAt(end)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new List<int>(list);
                    local.RemoveAt(local.Count - 1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "List",
                Operation = "RemoveAt(middle)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new List<int>(list);
                    local.RemoveAt(local.Count / 2);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "List",
                Operation = "Index access",
                AverageMilliseconds = Measure(() =>
                {
                    int sum = 0;
                    for (int i = 0; i < list.Count; i++)
                        sum += list[i];
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "List",
                Operation = "Search(value)",
                AverageMilliseconds = Measure(() =>
                {
                    list.Contains(N - 1);
                })
            });

            return results;
        }

        private IEnumerable<OperationResult> BenchmarkLinkedList()
        {
            var results = new List<OperationResult>();
            var list = new LinkedList<int>();

            for (int i = 0; i < N; i++)
                list.AddLast(i);

            results.Add(new OperationResult
            {
                CollectionName = "LinkedList",
                Operation = "AddLast",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new LinkedList<int>(list);
                    local.AddLast(N + 1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "LinkedList",
                Operation = "AddFirst",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new LinkedList<int>(list);
                    local.AddFirst(-1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "LinkedList",
                Operation = "Add(middle)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new LinkedList<int>(list);
                    var node = GetMiddleNode(local);
                    local.AddAfter(node, -1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "LinkedList",
                Operation = "RemoveFirst",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new LinkedList<int>(list);
                    local.RemoveFirst();
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "LinkedList",
                Operation = "RemoveLast",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new LinkedList<int>(list);
                    local.RemoveLast();
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "LinkedList",
                Operation = "Remove(middle)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new LinkedList<int>(list);
                    var node = GetMiddleNode(local);
                    local.Remove(node);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "LinkedList",
                Operation = "Search(value)",
                AverageMilliseconds = Measure(() =>
                {
                    list.Contains(N - 1);
                })
            });

            return results;
        }

        private LinkedListNode<int> GetMiddleNode(LinkedList<int> list)
        {
            var node = list.First;
            for (int i = 0; i < list.Count / 2; i++)
                node = node!.Next;
            return node!;
        }

        private IEnumerable<OperationResult> BenchmarkQueue()
        {
            var results = new List<OperationResult>();
            var queue = new Queue<int>();

            for (int i = 0; i < N; i++)
                queue.Enqueue(i);

            results.Add(new OperationResult
            {
                CollectionName = "Queue",
                Operation = "Enqueue",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new Queue<int>(queue);
                    local.Enqueue(N + 1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "Queue",
                Operation = "Dequeue",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new Queue<int>(queue);
                    local.Dequeue();
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "Queue",
                Operation = "Search(value)",
                AverageMilliseconds = Measure(() =>
                {
                    queue.Contains(N - 1);
                })
            });

            return results;
        }

        private IEnumerable<OperationResult> BenchmarkStack()
        {
            var results = new List<OperationResult>();
            var stack = new Stack<int>();

            for (int i = 0; i < N; i++)
                stack.Push(i);

            results.Add(new OperationResult
            {
                CollectionName = "Stack",
                Operation = "Push",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new Stack<int>(stack);
                    local.Push(N + 1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "Stack",
                Operation = "Pop",
                AverageMilliseconds = Measure(() =>
                {
                    var local = new Stack<int>(stack);
                    local.Pop();
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "Stack",
                Operation = "Search(value)",
                AverageMilliseconds = Measure(() =>
                {
                    stack.Contains(N - 1);
                })
            });

            return results;
        }

        private IEnumerable<OperationResult> BenchmarkImmutableList()
        {
            var results = new List<OperationResult>();
            var list = ImmutableList<int>.Empty;

            for (int i = 0; i < N; i++)
                list = list.Add(i);

            results.Add(new OperationResult
            {
                CollectionName = "ImmutableList",
                Operation = "Add(end)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = list;
                    local = local.Add(N + 1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "ImmutableList",
                Operation = "Insert(begin)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = list;
                    local = local.Insert(0, -1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "ImmutableList",
                Operation = "Insert(middle)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = list;
                    local = local.Insert(local.Count / 2, -1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "ImmutableList",
                Operation = "Remove(begin)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = list;
                    local = local.RemoveAt(0);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "ImmutableList",
                Operation = "Remove(end)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = list;
                    local = local.RemoveAt(local.Count - 1);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "ImmutableList",
                Operation = "Remove(middle)",
                AverageMilliseconds = Measure(() =>
                {
                    var local = list;
                    local = local.RemoveAt(local.Count / 2);
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "ImmutableList",
                Operation = "Index access",
                AverageMilliseconds = Measure(() =>
                {
                    int sum = 0;
                    for (int i = 0; i < list.Count; i++)
                        sum += list[i];
                })
            });

            results.Add(new OperationResult
            {
                CollectionName = "ImmutableList",
                Operation = "Search(value)",
                AverageMilliseconds = Measure(() =>
                {
                    list.Contains(N - 1);
                })
            });

            return results;
        }
    }
}
