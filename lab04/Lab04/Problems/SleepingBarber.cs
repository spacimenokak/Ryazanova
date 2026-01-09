using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lab04.Problems
{
    /// <summary>
    /// Класс для демонстрации задачи о спящем парикмахере
    /// </summary>
    public class SleepingBarber
    {
        private const int WaitingChairs = 3; // Количество стульев в зоне ожидания
        private readonly SemaphoreSlim _barberSemaphore = new SemaphoreSlim(0, 1);
        private readonly SemaphoreSlim _customerSemaphore = new SemaphoreSlim(0);
        private readonly SemaphoreSlim _accessSeatsSemaphore = new SemaphoreSlim(1);
        private int _freeSeats = WaitingChairs;
        private bool _isRunning = false;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public void Run(int durationSeconds = 15)
        {
            Console.WriteLine("\n=== Спящий парикмахер ===");
            Console.WriteLine($"Парикмахерская открыта! Стульев в зоне ожидания: {WaitingChairs}");
            
            _isRunning = true;
            
            // Запускаем парикмахера
            var barberTask = Task.Run(() => BarberWork(_cancellationTokenSource.Token));
            
            // Запускаем генератор клиентов
            var customerGeneratorTask = Task.Run(() => GenerateCustomers(_cancellationTokenSource.Token));
            
            // Ждем указанное время
            Task.Delay(TimeSpan.FromSeconds(durationSeconds)).Wait();
            
            // Останавливаем
            Stop();
            Task.WaitAll(barberTask, customerGeneratorTask);
            Console.WriteLine("Парикмахерская закрыта.");
        }

        private void BarberWork(CancellationToken token)
        {
            Random random = new Random();
            
            while (_isRunning && !token.IsCancellationRequested)
            {
                Console.WriteLine("Парикмахер спит...");
                
                // Ждем клиента
                _customerSemaphore.Wait(token);
                
                if (token.IsCancellationRequested) break;
                
                // Освобождаем место для следующего клиента
                _accessSeatsSemaphore.Wait(token);
                _freeSeats++; // Клиент встает со стула ожидания
                Console.WriteLine($"Клиент сел в кресло парикмахера. Свободных мест: {_freeSeats}");
                _accessSeatsSemaphore.Release();
                
                // Парикмахер работает
                _barberSemaphore.Release();
                
                Console.WriteLine("Парикмахер стрижет клиента...");
                Thread.Sleep(random.Next(1000, 3000)); // Время стрижки
                Console.WriteLine("Парикмахер закончил стрижку.");
            }
        }

        private void GenerateCustomers(CancellationToken token)
        {
            Random random = new Random();
            int customerId = 0;
            
            while (_isRunning && !token.IsCancellationRequested)
            {
                // Случайный интервал между приходом клиентов
                int interval = random.Next(500, 2000);
                Thread.Sleep(interval);
                
                if (token.IsCancellationRequested) break;
                
                customerId++;
                Task.Run(() => CustomerArrives(customerId, token));
            }
        }

        private void CustomerArrives(int customerId, CancellationToken token)
        {
            _accessSeatsSemaphore.Wait(token);
            
            if (token.IsCancellationRequested)
            {
                _accessSeatsSemaphore.Release();
                return;
            }
            
            if (_freeSeats > 0)
            {
                _freeSeats--;
                Console.WriteLine($"Клиент {customerId} пришел. Свободных мест: {_freeSeats}");
                _accessSeatsSemaphore.Release();
                
                // Будим парикмахера или садимся ждать
                _customerSemaphore.Release();
                
                // Ждем своей очереди к парикмахеру
                _barberSemaphore.Wait(token);
            }
            else
            {
                Console.WriteLine($"Клиент {customerId} пришел, но все места заняты! Уходит.");
                _accessSeatsSemaphore.Release();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource.Cancel();
        }
    }
}