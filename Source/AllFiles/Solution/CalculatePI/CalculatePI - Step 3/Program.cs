using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CalculatePI
{
    class Program
    {
        // Tuning constants:
        // If you have lots of memory, increase NUMPOINTS to improve the accuracy
        private const int NUMPOINTS = 10000000;
        private const int RADIUS = 10000;

        // Value to seed the random number generator for each calculation.
        // Using the same seed value ensures that the same results should be generated each time
        private const int SEED = 269222;

        // If you have a very fast processor, increase SPINWAITS to show the effects of parallelization
        private const int SPINWAITS = 1000;

        static double ParallelTasksPI()
        {
            List<double> pointsList = new List<double>();
            Random random = new Random(SEED);
            int numPointsInCircle = 0;
            Stopwatch timer = new Stopwatch();
            timer.Start();

            try
            {
                SemaphoreSlim semaphore = new SemaphoreSlim(1); 

                Parallel.For(0, NUMPOINTS, (x) =>
                {
                    semaphore.Wait(-1); 
                    int xCoord = random.Next(RADIUS);
                    int yCoord = random.Next(RADIUS);
                    double distanceFromOrigin =
                        Math.Sqrt(xCoord * xCoord + yCoord * yCoord);
                    pointsList.Add(distanceFromOrigin);
                    semaphore.Release(); 
                    doAdditionalProcessing();
                });

                foreach (double datum in pointsList)
                {
                    if (datum <= RADIUS)
                    {
                        numPointsInCircle++;
                    }
                }

                double pi = 4.0 * numPointsInCircle / NUMPOINTS;
                return pi;
            }
            finally
            {
                long milliseconds = timer.ElapsedMilliseconds;
                Console.WriteLine("ParallelTasksPI complete: Duration: {0} ms", milliseconds);
                Console.WriteLine("Points in pointsList: {0}. Points within circle: {1}", pointsList.Count, numPointsInCircle);
            }
        }

        private static void doAdditionalProcessing()
        {
            Thread.SpinWait(SPINWAITS);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Geometric approximation of PI calculated in parallel with TPL");
            double pi = ParallelTasksPI();
            Console.WriteLine("PI = {0}", pi);
        }
    }
}
