using System.Diagnostics;

namespace PerformanceMonitor
{
    public class Monitor
    {
        public static PerformanceCounter CpuCounter;
        public static PerformanceCounter RamCounter;
        static Monitor()
        {
            CpuCounter = new PerformanceCounter();

            CpuCounter.CategoryName = "Processor";
            CpuCounter.CounterName = "% Processor Time";
            CpuCounter.InstanceName = "_Total";

            RamCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public static float GetCpuUsage()
        {
            return CpuCounter.NextValue();
        }

        public static float GetAvailableRam()
        {
            return RamCounter.NextValue();
        }
    }
}
