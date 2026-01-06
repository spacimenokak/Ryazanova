using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lab04.Problems
{
    /// <summary>
    /// Класс для демонстрации задачи производитель-потребитель
    /// </summary>
    public class ProducerConsumer
    {
        private const int BufferCapacity = 5; // Размер буфера
        private readonly BlockingCollection<int> _buffer;
        private bool _isRunning = false;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public ProducerConsumer()
        {
            _buffer = new BlockingCollection<int>(BufferCapacity);
        }

        /// <summary>
        /// Запуск с использованием BlockingCollection (готовое решение)
        /// </summary>
        public void RunWithBlockingCollection(int durationSeconds = 10)
        {
            Console.WriteLine("\n=== Производитель-Потребитель (BlockingCollection) ===");
            Console.WriteLine($"Размер буфера: {BufferCapacity}");
            
            _isRunning = true;
            
            // Создаем производителей
            var producerTasks = new Task[2];
            for (int i = 0; i < 2; i++)
            {
                int producerId = i;
                producerTasks[i] = Task.Run(() => ProducerWithBlockingCollection(producerId, _cancellationTokenSource.Token));
            }
            
            // Создаем потребителей
            var consumerTasks = new Task[2];
            for (int i = 0; i < 2; i++)
            {
                int consumerId = i;
                consumerTasks[i] = Task.Run(() => ConsumerWithBlockingCollection(consumerId, _cancellationTokenSource.Token));
            }
            
            // Ждем указанное время
            Task.Delay(TimeSpan.FromSeconds(durationSeconds)).Wait();
            
            // Останавливаем
            Stop();
            Task.WaitAll(producerTasks);
            _buffer.CompleteAdding(); // Сообщаем, что добавление завершено
            Task.WaitAll(consumerTasks);
            
            Console.WriteLine($"Осталось элементов в буфере: {_buffer.Count}");
            Console.WriteLine("BlockingCollection версия завершена.");
        }

        /// <summary>
        /// Запуск с использованием SemaphoreSlim и lock (самописное решение)
        /// </summary>
        public void RunWithSemaphore(int durationSeconds = 10)
        {
            Console.WriteLine("\n=== Производитель-Потребитель (SemaphoreSlim + lock) ===");
            Console.WriteLine($"Размер буфера: {BufferCapacity}");
            
            var buffer = new ConcurrentQueue<int>();
            var mutex = new object();
            var emptySemaphore = new SemaphoreSlim(BufferCapacity, BufferCapacity); // Свободные места
            var fullSemaphore = new SemaphoreSlim(0, BufferCapacity); // Заполненные места
            
            _isRunning = true;
            
            // Создаем производителей
            var producerTasks = new Task[2];
            for (int i = 0; i < 2; i++)
            {
                int producerId = i;
                producerTasks[i] = Task.Run(() => ProducerWithSemaphore(producerId, buffer, mutex, emptySemaphore, fullSemaphore, _cancellationTokenSource.Token));
            }
            
            // Создаем потребителей
            var consumerTasks = new Task[2];
            for (int i = 0; i < 2; i++)
            {
                int consumerId = i;
                consumerTasks[i] = Task.Run(() => ConsumerWithSemaphore(consumerId, buffer, mutex, emptySemaphore, fullSemaphore, _cancellationTokenSource.Token));
            }
            
            // Ждем указанное время
            Task.Delay(TimeSpan.FromSeconds(durationSeconds)).Wait();
            
            // Останавливаем
            Stop();
            Task.WaitAll(producerTasks);
            Task.WaitAll(consumerTasks);
            
            Console.WriteLine($"Осталось элементов в буфере: {buffer.Count}");
            Console.WriteLine("Semaphore версия завершена.");
        }

        private void ProducerWithBlockingCollection(int producerId, CancellationToken token)
        {
            Random random = new Random(producerId + Environment.TickCount);
            int itemId = 0;
            
            while (_isRunning && !token.IsCancellationRequested)
            {
                int item = producerId * 1000 + itemId++;
                int productionTime = random.Next(500, 1500);
                
                Thread.Sleep(productionTime);
                
                if (token.IsCancellationRequested) break;
                
                _buffer.Add(item);
                Console.WriteLine($"Производитель {producerId} произвел: {item} (в буфере: {_buffer.Count})");
            }
        }

        private void ConsumerWithBlockingCollection(int consumerId, CancellationToken token)
        {
            Random random = new Random(consumerId + Environment.TickCount);
            
            while (_isRunning && !token.IsCancellationRequested)
            {
                try
                {
                    // Пытаемся взять элемент из буфера
                    if (_buffer.TryTake(out int item, 1000, token))
                    {
                        int consumptionTime = random.Next(500, 1500);
                        Thread.Sleep(consumptionTime);
                        Console.WriteLine($"Потребитель {consumerId} потребил: {item} (в буфере: {_buffer.Count})");
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private void ProducerWithSemaphore(int producerId, ConcurrentQueue<int> buffer, object mutex, 
                                          SemaphoreSlim emptySemaphore, SemaphoreSlim fullSemaphore, 
                                          CancellationToken token)
        {
            Random random = new Random(producerId + Environment.TickCount);
            int itemId = 0;
            
            while (_isRunning && !token.IsCancellationRequested)
            {
                int item = producerId * 1000 + itemId++;
                int productionTime = random.Next(500, 1500);
                
                Thread.Sleep(productionTime);
                
                if (token.IsCancellationRequested) break;
                
                // Ждем свободного места в буфере
                emptySemaphore.Wait(token);
                
                lock (mutex)
                {
                    buffer.Enqueue(item);
                    Console.WriteLine($"Производитель {producerId} произвел: {item} (в буфере: {buffer.Count})");
                }
                
                // Сигнализируем, что появился элемент
                fullSemaphore.Release();
            }
        }

        private void ConsumerWithSemaphore(int consumerId, ConcurrentQueue<int> buffer, object mutex,
                                          SemaphoreSlim emptySemaphore, SemaphoreSlim fullSemaphore,
                                          CancellationToken token)
        {
            Random random = new Random(consumerId + Environment.TickCount);
            
            while (_isRunning && !token.IsCancellationRequested)
            {
                // Ждем заполненного места в буфере
                fullSemaphore.Wait(token);
                
                if (token.IsCancellationRequested) break;
                
                int item = 0;
                lock (mutex)
                {
                    if (buffer.TryDequeue(out item))
                    {
                        int consumptionTime = random.Next(500, 1500);
                        Thread.Sleep(consumptionTime);
                        Console.WriteLine($"Потребитель {consumerId} потребил: {item} (в буфере: {buffer.Count})");
                    }
                }
                
                // Сигнализируем, что освободилось место
                emptySemaphore.Release();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
        }
    }
}