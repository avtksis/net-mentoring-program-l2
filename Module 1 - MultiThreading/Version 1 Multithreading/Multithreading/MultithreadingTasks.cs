using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async void Task2()
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
                var random = new Random();

                sequence = sequence.Select(item => item * random.Next(10)).ToArray();
                ArrayPrint(sequence, "Multiplied array:");

                return sequence;
            };

            int[] SortArray(int[] sequence)
            {
                sequence = sequence.OrderBy(item => item).ToArray();
                ArrayPrint(sequence, "Sorted array:");

                return sequence;
            };

            void PrintAverage(int[] sequence) => _output.WriteLine($"Average of array: {sequence.Average()}");

           createArrayTask
                .ContinueWith(task => MultiplyArray(task.Result))
                .ContinueWith(task => SortArray(task.Result))
                ;//.ContinueWith(task => PrintAverage(task.Result));
            
            //createArrayTask.Start();
            //var createArrayTask = new Task(() =>
            //{
            //    for (int i = 0; i < 10; i++) sequence[i] = random.Next(0, 10);
            //    ArrayPrint(sequence, "Randomly generated array");
            //});

            //var multiplyArrayTask = new Task(() =>
            //{
            //    sequence = sequence.Select(item => item * random.Next(10)).ToArray();
            //    ArrayPrint(sequence, "Multiplied array:");
            //});

            //var sortArrayTask = new Task(() =>
            //{
            //    sequence = sequence.OrderBy(item => item).ToArray();
            //    ArrayPrint(sequence, "Sorted array:");
            //});

            //var printAverageTask = new Task(() => _output.WriteLine($"Average of array: {sequence.Average()}") );

            //var res = CreateArrayTask.ContinueWith(task1 => 
            //        multiplyArrayTask.ContinueWith(task2 => 
            //            sortArrayTask.ContinueWith(task3 => printAverageTask))).Result;


        }
    }
}