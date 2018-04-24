using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NetTraffic
{
    class NetTrafficCore
    {
        private static PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
        private static string[] instance = performanceCounterCategory.GetInstanceNames();
        private static int NetworkInterfaceNum = instance.GetLength(0);
        private static PerformanceCounter[] performanceCounterReceived = new PerformanceCounter[NetworkInterfaceNum];
        private static PerformanceCounter[] performanceCounterSent = new PerformanceCounter[NetworkInterfaceNum];

        static NetTrafficCore()
        {
            Console.WriteLine("Initialize");
            for (int i=0;i<NetworkInterfaceNum;i++)
            {
                performanceCounterReceived[i] = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance[i]);
                performanceCounterSent[i] = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance[i]);
            }
        }

        public static float GetNetReceived()
        {
            float NetReceived = 0;
            for (int i = 0; i < NetworkInterfaceNum; i++)
            {
                NetReceived += performanceCounterReceived[i].NextValue();
            }
            return NetReceived;
        }

        public static float GetNetSent()
        {
            float NetSent = 0;
            for (int i = 0; i < NetworkInterfaceNum; i++)
            {
                NetSent += performanceCounterSent[i].NextValue();
            }
            return NetSent;
        }
    }
}
