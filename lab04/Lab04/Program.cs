using System;
using System.Threading.Tasks;
using Lab04.Problems;

namespace Lab04
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Лабораторная работа 4: Многопоточность ===");
            Console.WriteLine("Изучение проблем синхронизации потоков\n");
            
            try
            {
                // 1. Обедающие философы
                await RunDiningPhilosophers();
                
                // 2. Спящий парикмахер
                await RunSleepingBarber();
                
                // 3. Производитель-Потребитель
                await RunProducerConsumer();
                
                Console.WriteLine("\n=== Все задачи завершены ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static async Task RunDiningPhilosophers()
        {
            var philosophers = new DiningPhilosophers();
            
            Console.WriteLine("1. Задача 'Обедающие философы'");
            Console.WriteLine("====================================");
            
            // Версия с deadlock (может зависнуть!)
            Console.WriteLine("\nЗапуск версии С deadlock (10 секунд)...");
            philosophers.RunWithDeadlock(10);
            
            Console.WriteLine("\nОжидание 2 секунды перед следующей версией...");
            await Task.Delay(2000);
            
            // Версия без deadlock
            Console.WriteLine("\nЗапуск версии БЕЗ deadlock (10 секунд)...");
            philosophers.RunWithoutDeadlock(10);
            
            Console.WriteLine("\nЗадача 'Обедающие философы' завершена.\n");
        }

        static async Task RunSleepingBarber()
        {
            var barber = new SleepingBarber();
            
            Console.WriteLine("2. Задача 'Спящий парикмахер'");
            Console.WriteLine("====================================");
            
            Console.WriteLine("\nЗапуск задачи (15 секунд)...");
            barber.Run(15);
            
            Console.WriteLine("\nЗадача 'Спящий парикмахер' завершена.\n");
            
            // Пауза между задачами
            await Task.Delay(2000);
        }

        static async Task RunProducerConsumer()
        {
            var producerConsumer = new ProducerConsumer();
            
            Console.WriteLine("3. Задача 'Производитель-Потребитель'");
            Console.WriteLine("==========================================");
            
            // Версия с BlockingCollection
            Console.WriteLine("\nЗапуск версии с BlockingCollection (10 секунд)...");
            producerConsumer.RunWithBlockingCollection(10);
            
            Console.WriteLine("\nОжидание 2 секунды перед следующей версией...");
            await Task.Delay(2000);
            
            // Версия с SemaphoreSlim
            Console.WriteLine("\nЗапуск версии с SemaphoreSlim (10 секунд)...");
            producerConsumer.RunWithSemaphore(10);
            
            Console.WriteLine("\nЗадача 'Производитель-Потребитель' завершена.");
        }
    }
}