using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;
using Xunit;
using Xunit.Abstractions;

namespace Multithreading
{
    public class MultithreadingTasks
    {
        private readonly ITestOutputHelper _output;

        public MultithreadingTasks(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Task1()
        {
            var tasks = new Task[100];
            Task GetTask() => new Task(() =>
            {
                for (int j = 0; j <= 1000; j++) _output.WriteLine($"Task #{Task.CurrentId} – {j}");
            });

            for (int i = 0; i < 100; i++) tasks[i] = GetTask();

            foreach (var task in tasks) task.Start();

            Task.WaitAll(tasks);
        }

        [Fact]
        public void Task2()
        {
            void ArrayPrint(int[] array, string description) => _output.WriteLine($"{description}: {string.Join(", ", array)}");

            var createArrayTask = new Task<int[]>(() =>
            {
                var sequence = new int[10];
                var random = new Random();

                for (int i = 0; i < 10; i++) sequence[i] = random.Next(0, 10);
                ArrayPrint(sequence, "Randomly generated array");

                return sequence;
            });
            
            int[] MultiplyArray(int[] sequence)
            {
                var randomMultiplier = new Random().Next(10);

                sequence = sequence.Select(item => item * randomMultiplier).ToArray();
                ArrayPrint(sequence, $"Multiplied array ({randomMultiplier})");

                return sequence;
            };

            int[] SortArray(int[] sequence)
            {
                sequence = sequence.OrderBy(item => item).ToArray();
                ArrayPrint(sequence, "Sorted array");

                return sequence;
            };

            void PrintAverage(int[] sequence) => _output.WriteLine($"Average of array: {sequence.Average()}");

            var tasksChain = createArrayTask
                .ContinueWith(task => MultiplyArray(task.Result))
                .ContinueWith(task => SortArray(task.Result))
                .ContinueWith(task => PrintAverage(task.Result));

            createArrayTask.Start();
            tasksChain.Wait();
        }

        [Fact]
        public void Task3()
        {
            int[,] CreateMatrice(int mSize, int nSize)
            {
                var matrice = new int[mSize, nSize];
                var random = new Random();

                for (int m = 0; m < mSize; m++)
                    for (int n = 0; n < nSize; n++)
                        matrice[m, n] = random.Next(10);

                return matrice;
            }

            var matriceFirst = CreateMatrice(5, 5);
            var matriceSecond = CreateMatrice(5, 5);

            Assert.True(false);
        }

        [Fact]
        public void Task4()
        {
            Assert.True(false);
        }

        [Fact]
        public void Task5()
        {
            Assert.True(false);
        }

        [Fact]
        public void Task6()
        {
            var sequence = new List<int>();

            List<int> SafeAccess()
            {
                object locker = new object();

                lock (locker)
                {
                    return sequence;
                }
            }

         

            new Thread((object obj) =>
            {
                int elementCount = sequence.Count;

                while (elementCount <= 10 && elementCount > 0)
                {
                    _output.WriteLine(SafeAccess().Last().ToString());
                }
            }).Start();

            new Thread((object obj) =>
            {
                for (int i = 0; i < 10; i++) SafeAccess().Add(i);
            }).Start();

            Thread.Sleep(5000);
        }
    }
}