using NUnit.Framework;
using Lab04.Problems;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Lab04.Tests
{
    [TestFixture]
    public class MultithreadingTests
    {
        [Test]
        [Timeout(5000)] // Максимальное время выполнения теста
        public void DiningPhilosophers_WithoutDeadlock_ShouldNotDeadlock()
        {
            var philosophers = new DiningPhilosophers();
            
            // Запускаем на короткое время - не должно быть deadlock
            Assert.DoesNotThrow(() => 
            {
                philosophers.RunWithoutDeadlock(2);
            });
        }

        [Test]
        [Timeout(5000)]
        public void SleepingBarber_ShouldHandleCustomers()
        {
            var barber = new SleepingBarber();
            
            // Запускаем на короткое время
            Assert.DoesNotThrow(() => 
            {
                barber.Run(2);
            });
        }

        [Test]
        [Timeout(5000)]
        public void ProducerConsumer_BlockingCollection_ShouldWork()
        {
            var pc = new ProducerConsumer();
            
            // Запускаем на короткое время
            Assert.DoesNotThrow(() => 
            {
                pc.RunWithBlockingCollection(2);
            });
        }

        [Test]
        [Timeout(5000)]
        public void ProducerConsumer_Semaphore_ShouldWork()
        {
            var pc = new ProducerConsumer();
            
            // Запускаем на короткое время
            Assert.DoesNotThrow(() => 
            {
                pc.RunWithSemaphore(2);
            });
        }

        [Test]
        public void ThreadSafety_CheckNoRaceConditions()
        {
            // Тест на безопасность потоков
            int sharedCounter = 0;
            object lockObject = new object();
            
            var tasks = new Task[10];
            
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        lock (lockObject)
                        {
                            sharedCounter++;
                        }
                    }
                });
            }
            
            Task.WaitAll(tasks);
            
            // Если нет состояния гонки, счетчик должен быть 10000
            Assert.That(sharedCounter, Is.EqualTo(10000));
        }
        
        [Test]
        public void Lock_BasicFunctionality_Works()
        {
            // Проверка базовой функциональности lock
            object lockObj = new object();
            bool wasLocked = false;
            
            lock (lockObj)
            {
                wasLocked = true;
            }
            
            Assert.That(wasLocked, Is.True);
        }
        
        [Test]
        public void SemaphoreSlim_BasicFunctionality_Works()
        {
            // Проверка базовой функциональности SemaphoreSlim
            var semaphore = new SemaphoreSlim(1, 1);
            bool entered = false;
            
            entered = semaphore.Wait(100); // Пытаемся войти с таймаутом
            
            Assert.That(entered, Is.True);
            
            if (entered)
            {
                semaphore.Release();
            }
        }
    }
}