using System;
using System.Linq;

namespace Lab02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var benchmark = new CollectionBenchmark();
            var results = benchmark.RunAll();

            // вывод таблички
            Console.WriteLine("{0,-15} | {1,-20} | {2,10}", "Collection", "Operation", "Avg ms");
            Console.WriteLine(new string('-', 55));

            foreach (var r in results.OrderBy(r => r.CollectionName).ThenBy(r => r.Operation))
            {
                Console.WriteLine("{0,-15} | {1,-20} | {2,10:F3}",
                    r.CollectionName, r.Operation, r.AverageMilliseconds);
            }
        }
    }
}
