namespace ThreadSyncTest
{
    internal class Program
    {
        static int[] container = new int[10];
        static int index = 0;

        private readonly static object _lockObj = new object();

        static void Add()
        {
            lock (_lockObj)
            {
                for (int i = 0; i < 10; i++)
                {
                    int sum = 0;
                    for (int j = 0; j < 10; j++)
                    {
                        sum += j;
                    }
                    container[index] = sum;

                    Monitor.PulseAll(_lockObj); // remove lock from MoveForward
                    Monitor.Wait(_lockObj); // lock MoveForward
                }

                Monitor.PulseAll(_lockObj);
            }
        }

        static void MoveForward()
        {
            lock (_lockObj)
            {
                for (int i = 0; i < 9; i++)
                {
                    index++;

                    Monitor.PulseAll(_lockObj); // remove lock from Add
                    Monitor.Wait(_lockObj); // lock Add
                }

                Monitor.PulseAll(_lockObj);
            }
        }

        static void Main(string[] args)
        {
            Thread t1 = new Thread(Add);
            t1.Name = "Add";

            Thread t2 = new Thread(MoveForward);
            t2.Name = "MoveForward";

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            container.ToList().ForEach(Console.WriteLine);
        }
    }
}