using System;
using Lab03.Collections;

namespace Lab03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Тестирование SimpleList ===");
            TestSimpleList();
            
            Console.WriteLine("\n=== Тестирование BidirectionalLinkedList ===");
            TestBidirectionalLinkedList();
            
            Console.WriteLine("\n=== Тестирование SimpleDictionary ===");
            TestSimpleDictionary();
        }

        static void TestSimpleList()
        {
            var list = new SimpleList<int>();
            
            Console.WriteLine("Добавляем элементы 1, 2, 3:");
            list.Add(1);
            list.Add(2);
            list.Add(3);
            
            Console.WriteLine($"Количество элементов: {list.Count}");
            Console.WriteLine($"Емкость: {list.Capacity}");
            
            Console.WriteLine("Элементы списка:");
            foreach (var item in list)
            {
                Console.WriteLine($"  {item}");
            }
            
            Console.WriteLine($"Содержит 2? {list.Contains(2)}");
            Console.WriteLine($"Индекс элемента 2: {list.IndexOf(2)}");
            
            Console.WriteLine("Вставляем 0 в начало:");
            list.Insert(0, 0);
            Console.WriteLine($"Теперь первый элемент: {list[0]}");
            
            Console.WriteLine("Удаляем элемент 2:");
            list.Remove(2);
            Console.WriteLine($"Теперь количество элементов: {list.Count}");
        }

        static void TestBidirectionalLinkedList()
        {
            var list = new BidirectionalLinkedList<string>();
            
            Console.WriteLine("Добавляем элементы:");
            list.Add("первый");
            list.Add("второй");
            list.Add("третий");
            
            Console.WriteLine($"Количество элементов: {list.Count}");
            
            Console.WriteLine("Элементы списка:");
            foreach (var item in list)
            {
                Console.WriteLine($"  {item}");
            }
            
            Console.WriteLine("Вставляем 'новый' в середину:");
            list.Insert(1, "новый");
            Console.WriteLine($"Теперь второй элемент: {list[1]}");
            
            Console.WriteLine($"Индекс 'третий': {list.IndexOf("третий")}");
        }

        static void TestSimpleDictionary()
        {
            var dict = new SimpleDictionary<string, int>();
            
            Console.WriteLine("Добавляем пары ключ-значение:");
            dict.Add("яблоко", 5);
            dict.Add("банан", 3);
            dict["апельсин"] = 7;
            
            Console.WriteLine($"Количество элементов: {dict.Count}");
            
            Console.WriteLine("Содержимое словаря:");
            foreach (var pair in dict)
            {
                Console.WriteLine($"  {pair.Key}: {pair.Value}");
            }
            
            Console.WriteLine($"Значение для 'яблоко': {dict["яблоко"]}");
            Console.WriteLine($"Содержит ключ 'банан'? {dict.ContainsKey("банан")}");
            
            Console.WriteLine("Пытаемся получить значение несуществующего ключа:");
            if (dict.TryGetValue("груша", out int value))
            {
                Console.WriteLine($"  Груша: {value}");
            }
            else
            {
                Console.WriteLine("  Ключ 'груша' не найден");
            }
            
            Console.WriteLine("Удаляем 'банан':");
            dict.Remove("банан");
            Console.WriteLine($"Теперь количество элементов: {dict.Count}");
            
            Console.WriteLine("Все ключи:");
            foreach (var key in dict.Keys)
            {
                Console.WriteLine($"  {key}");
            }
        }
    }
}