using System;
using System.IO;

namespace SocketServer
{
    public class ServerHardware
    {
        public string GPUName { get; set; }
        public string GPUDeviceID { get; set; }
        public string GPUAdapterRAM { get; set; }
        public string GPUArchitecture { get; set; }
        public string GPUProcessor { get; set; }
        public string ProcessorName { get; set; }
        public string ProcessorManufacturer { get; set; }
        public string ProcessorCurrentClockSpeed { get; set; }
        public string ProcessorNumberOfCores { get; set; }
        public string ProcessorNumberOfEnabledCores { get; set; }
        public string NumberOfLogicalProcessors { get; set; }
        public string ProcessorArchitecture { get; set; }


        public ServerHardware()
        {

        }

        public ServerHardware(string gPUName, string gPUDeviceID, string gPUAdapterRAM, string gPUArchitecture, string gPUProcessor, string processorName, string processorManufacturer, string processorCurrentClockSpeed, string processorNumberOfCores, string processorNumberOfEnabledCores, string numberOfLogicalProcessors, string processorArchitecture)
        {
            GPUName = gPUName;
            GPUDeviceID = gPUDeviceID;
            GPUAdapterRAM = gPUAdapterRAM;
            GPUArchitecture = gPUArchitecture;
            GPUProcessor = gPUProcessor;
            ProcessorName = processorName;
            ProcessorManufacturer = processorManufacturer;
            ProcessorCurrentClockSpeed = processorCurrentClockSpeed;
            ProcessorNumberOfCores = processorNumberOfCores;
            ProcessorNumberOfEnabledCores = processorNumberOfEnabledCores;
            NumberOfLogicalProcessors = numberOfLogicalProcessors;
            ProcessorArchitecture = processorArchitecture;
        }
    }
}
