using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Clifton.Common;

namespace Clifton.Common.Tests {
    [TestClass]
    public class MyComputerTests {
        [TestMethod]
        public void MyComputer_AvailablePhysicalMemoryInBytes() {
            ulong availablePhysicalMemoryInBytes = MyComputer.AvailablePhysicalMemoryInBytes;
            Assert.AreNotEqual(0, availablePhysicalMemoryInBytes);
        }

        [TestMethod]
        public void MyComputer_CLRVersion() {
            string clrVersion = MyComputer.CLRVersion;
            Assert.IsNotNull(clrVersion);
        }

        [TestMethod]
        public void MyComputer_Hostname() {
            string hostname = MyComputer.Hostname;
            Assert.IsNotNull(hostname);
        }

        [TestMethod]
        public void MyComputer_ManagedHeapSizeInBytes() {
            ulong managedHeapSizeInBytes = MyComputer.ManagedHeapSizeInBytes;
            Assert.AreNotEqual(0, managedHeapSizeInBytes);
        }

        [TestMethod]
        public void MyComputer_ProcessorCount() {
            int processorCount = MyComputer.ProcessorCount;
            Assert.AreNotEqual(0, processorCount);
        }

        [TestMethod]
        public void MyComputer_TimeZone() {
            string timeZone = MyComputer.TimeZone;
            Assert.IsNotNull(timeZone);
        }

        [TestMethod]
        public void MyComputer_TotalPhysicalMemoryInBytes() {
            ulong totalPhysicalMemoryInBytes = MyComputer.TotalPhysicalMemoryInBytes;
            Assert.AreNotEqual(0, totalPhysicalMemoryInBytes);
        }

        [TestMethod]
        public void MyComputer_TotalVirtualMemoryInBytes() {
            ulong totalVirtualMemoryInBytes = MyComputer.TotalVirtualMemoryInBytes;
            Assert.AreNotEqual(0, totalVirtualMemoryInBytes);
        }
    }
}