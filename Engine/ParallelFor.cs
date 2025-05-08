using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    /*CPU 1

    Core 1: Affinity: 0 Available: No

    Core 2: Affinity: 1 Available: Yes

    

CPU 2

    Core 1: Affinity: 2 Available: No

    Core 2: Affinity: 3 Available: Yes

    

CPU 3

    Core 1: Affinity: 4 Available: Yes

    Core 2: Affinity: 5 Available: Yes

*/

    public delegate void ForDelegate(int i);
    public delegate void ThreadDelegate();

    public class Parallel
    {
        /// <summary>
        /// Parallel for loop. Invokes given action, passing arguments 
        /// fromInclusive - toExclusive on multiple threads.
        /// Returns when loop finished.
        /// </summary>

        public static void For(int fromInclusive, int toExclusive, ForDelegate action)
        {
            // ChunkSize = 1 makes items to be processed in order.
            // Bigger chunk size should reduce lock waiting time and thus
            // increase paralelism.
            int chunkSize = 64;
            int threadCount = Environment.ProcessorCount;
            int cnt = fromInclusive - chunkSize;

            // processing function
            // takes next chunk and processes it using action
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            ThreadDelegate process = delegate()
            {
#if XBOX

                    t.SetProcessorAffinity(new[] { 5 });

#endif
                System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
                while (true)
                {
                    int cntMem = 0;
                    lock (typeof(Parallel))
                    {
                        // take next chunk
                        cnt += chunkSize;
                        cntMem = cnt;
                    }

                    // process chunk
                    // here items can come out of order if chunkSize > 1
                    for (int i = cntMem; i < cntMem + chunkSize; ++i)
                    {
                        if (i >= toExclusive) return;
                        action(i);
                    }
                }
            };
            // launch process() threads
            IAsyncResult[] asyncResults = new IAsyncResult[threadCount];

            for (int i = 0; i < threadCount; ++i)
            {
                asyncResults[i] = process.BeginInvoke(null, null);
            }
            // wait for all threads to complete
            for (int i = 0; i < threadCount; ++i)
            {
                process.EndInvoke(asyncResults[i]);
            }
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Normal;
        }
    }
}
