using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab04.Problems
{
    /// <summary>
    /// Класс для демонстрации проблемы обедающих философов
    /// </summary>
    public class DiningPhilosophers
    {
        private const int PhilosopherCount = 5;
        private readonly object[] _forks = new object[PhilosopherCount];
        private readonly Philosopher[] _philosophers = new Philosopher[PhilosopherCount];
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isRunning = false;

        public DiningPhilosophers()
        {
            for (int i = 0; i < PhilosopherCount; i++)
            {
                _forks[i] = new object();
            }
            
            for (int i = 0; i < PhilosopherCount; i++)
            {
                _philosophers[i] = new Philosopher(i, _forks[i], _forks[(i + 1) % PhilosopherCount]);
            }
        }

        /// <summary>
        /// Версия с возможным deadlock (неправильная реализация)
        /// </summary>
        public void RunWithDeadlock(int durationSeconds = 10)
        {
            Console.WriteLine("\n=== Обедающие философы (версия с DEADLOCK) ===");
            Console.WriteLine("Эта версия может привести к взаимной блокировке!");
            
            _isRunning = true;
            var tasks = new Task[PhilosopherCount];
            
            for (int i = 0; i < PhilosopherCount; i++)
            {
                int philosopherId = i;
                tasks[i] = Task.Run(() => PhilosopherWithDeadlock(_philosophers[philosopherId], _cancellationTokenSource.Token));
            }
            
            // Даем поработать некоторое время
            Task.Delay(TimeSpan.FromSeconds(durationSeconds)).Wait();
            
            Stop();
            Task.WaitAll(tasks);
            Console.WriteLine("Версия с deadlock завершена.");
        }

        /// <summary>
        /// Версия без deadlock (исправленная)
        /// </summary>
        public void RunWithoutDeadlock(int durationSeconds = 10)
        {
            Console.WriteLine("\n=== Обедающие философы (версия БЕЗ deadlock) ===");
            Console.WriteLine("Используется стратегия взятия вилок в определенном порядке.");
            
            _isRunning = true;
            var tasks = new Task[PhilosopherCount];
            
            for (int i = 0; i < PhilosopherCount; i++)
            {
                int philosopherId = i;
                tasks[i] = Task.Run(() => PhilosopherWithoutDeadlock(_philosophers[philosopherId], _cancellationTokenSource.Token));
            }
            
            // Даем поработать некоторое время
            Task.Delay(TimeSpan.FromSeconds(durationSeconds)).Wait();
            
            Stop();
            Task.WaitAll(tasks);
            Console.WriteLine("Версия без deadlock завершена.");
        }

        private void PhilosopherWithDeadlock(Philosopher philosopher, CancellationToken token)
        {
            Random random = new Random(philosopher.Id + Environment.TickCount);
            
            while (_isRunning && !token.IsCancellationRequested)
            {
                // 1. Думаем
                Think(philosopher, random);
                
                // 2. Берем левую вилку (может привести к deadlock)
                lock (philosopher.LeftFork)
                {
                    Console.WriteLine($"Философ {philosopher.Id} взял левую вилку");
                    
                    // Небольшая задержка, увеличивающая вероятность deadlock
                    Thread.Sleep(random.Next(50, 200));
                    
                    // 3. Пытаемся взять правую вилку
                    lock (philosopher.RightFork)
                    {
                        Console.WriteLine($"Философ {philosopher.Id} взял правую вилку и начал есть");
                        Eat(philosopher, random);
                        Console.WriteLine($"Философ {philosopher.Id} закончил есть");
                    }
                }
                
                Console.WriteLine($"Философ {philosopher.Id} положил обе вилки");
            }
        }

        private void PhilosopherWithoutDeadlock(Philosopher philosopher, CancellationToken token)
        {
            Random random = new Random(philosopher.Id + Environment.TickCount);
            
            while (_isRunning && !token.IsCancellationRequested)
            {
                // 1. Думаем
                Think(philosopher, random);
                
                // 2. Определяем порядок взятия вилок (избегаем deadlock)
                object firstFork, secondFork;
                
                // Философы с четным ID берут сначала левую, потом правую
                // Философы с нечетным ID берут сначала правую, потом левую
                if (philosopher.Id % 2 == 0)
                {
                    firstFork = philosopher.LeftFork;
                    secondFork = philosopher.RightFork;
                }
                else
                {
                    firstFork = philosopher.RightFork;
                    secondFork = philosopher.LeftFork;
                }
                
                // 3. Берем вилки в определенном порядке
                lock (firstFork)
                {
                    Console.WriteLine($"Философ {philosopher.Id} взял первую вилку");
                    
                    lock (secondFork)
                    {
                        Console.WriteLine($"Философ {philosopher.Id} взял обе вилки и начал есть");
                        Eat(philosopher, random);
                        Console.WriteLine($"Философ {philosopher.Id} закончил есть");
                    }
                }
                
                Console.WriteLine($"Философ {philosopher.Id} положил обе вилки");
            }
        }

        private void Think(Philosopher philosopher, Random random)
        {
            int thinkTime = random.Next(500, 1500);
            Console.WriteLine($"Философ {philosopher.Id} думает {thinkTime}мс");
            Thread.Sleep(thinkTime);
        }

        private void Eat(Philosopher philosopher, Random random)
        {
            int eatTime = random.Next(300, 1000);
            Thread.Sleep(eatTime);
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
        }

        private class Philosopher
        {
            public int Id { get; }
            public object LeftFork { get; }
            public object RightFork { get; }

            public Philosopher(int id, object leftFork, object rightFork)
            {
                Id = id;
                LeftFork = leftFork;
                RightFork = rightFork;
            }
        }
    }
}