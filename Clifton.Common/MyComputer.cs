using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Clifton.Common {
    /// <summary>
    /// Helper class for dealing with the machine you're on.
    /// </summary>
    public static class MyComputer {
        /// <summary>
        /// Note for shared hosting usage: this could throw a Security Exception if you don't have privileges or trust level isn't set properly.
        /// </summary>
        public static UInt64 AvailablePhysicalMemoryInBytes {
            get { return new ComputerInfo().AvailablePhysicalMemory; }
        }

        /// <summary>
        /// Note for shared hosting usage: this could throw a Security Exception if you don't have privileges or trust level isn't set properly.
        /// </summary>
        public static UInt64 AvailableVirtualMemoryInBytes {
            get { return new ComputerInfo().AvailableVirtualMemory; }
        }

        public static string CLRVersion {
            get { return Environment.Version.ToString(); }
        }

        #region Open Process / Shell Execute

        /// <summary>
        /// Used to shell execute a process.
        /// </summary>
        /// <param name="strWorkingDirectory">Directory you want to execute from</param>
        /// <param name="strFileName">File you want to execute</param>
        /// <param name="strArguments">Arguments to pass to the file to execute</param>
        /// <param name="iTimeoutMS">Timeout in milliseconds for the process to execute</param>
        /// <param name="bUseShell">If true, will launch using Windows shell. Otherwise, opens process directly. Read the notes inside this function for more detail.</param>
        /// <param name="strStandardOut">The text the file writes to standard out</param>
        /// <returns>System exit code</returns>
        private static int Execute(string strWorkingDirectory, string strFileName, string strArguments, Int32 iTimeoutMS, bool bUseShell, out string strStandardOut) {
            Process myProcess = new System.Diagnostics.Process();

            myProcess.StartInfo.WorkingDirectory = strWorkingDirectory;
            myProcess.StartInfo.FileName = strFileName;
            myProcess.StartInfo.Verb = "open";
            myProcess.StartInfo.Arguments = strArguments;
            myProcess.StartInfo.CreateNoWindow = true;

            // Setting UseShellExecute to FALSE is required to capture standard output.
            // However, it's set to FALSE, the process you launch might hang if you launch other processes (in my experience).
            // For more info, see https://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.useshellexecute.aspx
            myProcess.StartInfo.UseShellExecute = bUseShell;
            if (!bUseShell) {
                myProcess.StartInfo.RedirectStandardOutput = true;
            }

            //Open the process and wait for exit. Timeout is provided in case the user is prompted for input, etc.
            try {
                myProcess.Start();
                if (!myProcess.WaitForExit(iTimeoutMS)) {
                    throw new System.TimeoutException("Timed out while executing!");
                }
            } catch {
                myProcess.Close();
                throw;
            }

            //Read the command line output if requested.
            try {
                if (!bUseShell) {
                    StreamReader sr = myProcess.StandardOutput;
                    strStandardOut = sr.ReadToEnd();
                } else {
                    strStandardOut = string.Empty;
                }
                return myProcess.ExitCode;
            } finally {
                myProcess.Close();
            }
        }

        public static int Execute(string strWorkingDirectory, string strFileName, string strArguments, out string strStandardOut) {
            return Execute(strWorkingDirectory, strFileName, strArguments, Int32.MaxValue, false, out strStandardOut);
        }

        public static int Execute(string strWorkingDirectory, string strFileName, string strArguments, Int32 iTimeoutMS) {
            string strStandardOut;
            return Execute(strWorkingDirectory, strFileName, strArguments, iTimeoutMS, true, out strStandardOut);
        }

        public static int Execute(string strWorkingDirectory, string strFileName, string strArguments) {
            string strStandardOut;
            return Execute(strWorkingDirectory, strFileName, strArguments, Int32.MaxValue, true, out strStandardOut);
        }

        #endregion

        public static string Hostname {
            get { return Dns.GetHostName(); }
        }

        /// <summary>
        /// Note for shared hosting usage: this could throw a Security Exception if you don't have privileges or trust level isn't set properly.
        /// </summary>
        public static bool is64Bit {
            get {
                string pa = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                return ((String.IsNullOrWhiteSpace(pa) || String.Compare(pa, 0, "x86", 0, 3, true) == 0) ? false : true);
            }
        }

        public static bool is64BitProcess {
            get { return (IntPtr.Size == 8); }
        }

        #region Time

        public static DateTime LocalTime {
            get { return DateTime.Now; }
        }

        public static string TimeZone {
            get { return System.TimeZone.CurrentTimeZone.StandardName; }
        }

        #endregion

        public static UInt64 ManagedHeapSizeInBytes {
            get {
                // For more info, see...
                // http://msdn.microsoft.com/en-us/library/system.gc.gettotalmemory.aspx
                // http://dotnetperls.com/gc-gettotalmemory

                return (UInt64)GC.GetTotalMemory(true);
            }
        }

        public static int ProcessorCount {
            get { return Environment.ProcessorCount; }
        }

        /// <summary>
        /// Note for shared hosting usage: this could throw a Security Exception if you don't have privileges or trust level isn't set properly.
        /// </summary>
        public static UInt64 TotalPhysicalMemoryInBytes {
            get { return new ComputerInfo().TotalPhysicalMemory; }
        }
        /// <summary>
        /// Note for shared hosting usage: this could throw a Security Exception if you don't have privileges or trust level isn't set properly.
        /// </summary>
        public static UInt64 TotalVirtualMemoryInBytes {
            get { return new ComputerInfo().TotalVirtualMemory; }
        }

        /// <summary>
        /// Download a file from the internet.
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strDestinationPath"></param>
        public static void WebGet(string strUrl, string strDestinationPath) {
            HttpWebRequest request;
            HttpWebResponse response = null;

            try {
                //make web request, get response
                request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Timeout = 10000;
                request.AllowWriteStreamBuffering = false;
                response = (HttpWebResponse)request.GetResponse();
                Stream s = response.GetResponseStream();

                //write response to disk
                FileStream fs = new FileStream(strDestinationPath, FileMode.Create);
                byte[] read = new byte[256];
                int count = s.Read(read, 0, read.Length);

                while (count > 0) {
                    fs.Write(read, 0, count);
                    count = s.Read(read, 0, read.Length);
                }
                fs.Close();
                s.Close();

            } finally {
                if (response != null) {
                    response.Close();
                }
            }
        }

        #region Windows version

        /// <summary>
        /// Used for getting a friendly name for the operating system.
        /// partially based off https://msdn.microsoft.com/en-us/library/ms724832(v=VS.85).aspx
        /// </summary>
        public static string WindowsVersion {
            get {
                switch (Environment.OSVersion.Platform) {
                    case PlatformID.Win32S:
                        return "Windows 3.1";

                    case PlatformID.Win32Windows:
                        switch (Environment.OSVersion.Version.Minor) {
                            case 0: return "Windows 95";
                            case 10: return "Windows 98";
                            case 90: return "Windows ME";
                        }
                        break;

                    case PlatformID.Win32NT:
                        switch (Environment.OSVersion.Version.Major) {
                            case 3: return "Windows NT 3.51";
                            case 4: return "Windows NT 4.0";
                            case 5:
                                switch (Environment.OSVersion.Version.Minor) {
                                    case 0: return "Windows 2000";
                                    case 1: return "Windows XP";
                                    case 2: return "Windows 2003";
                                }
                                break;
                            case 6:
                                switch (Environment.OSVersion.Version.Minor) {
                                    case 0: return "Windows Vista / Windows 2008 Server";
                                    case 1: return "Windows 7 / Windows Server 2008 R2";
                                    case 2: return "Windows 8 / Windows Server 2012";
                                    case 3: return "Windows 8.1 / Windows Server 2012 R2";
                                }
                                break;
                        }
                        break;

                    case PlatformID.WinCE:
                        return "Win CE";
                }

                return "Unknown";
            }
        }

        #endregion
    }
}