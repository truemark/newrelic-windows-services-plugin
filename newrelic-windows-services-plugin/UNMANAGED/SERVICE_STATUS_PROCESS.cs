using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace newrelic_windows_services_plugin
{
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class SERVICE_STATUS_PROCESS
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint dwServiceType;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwCurrentState;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwControlsAccepted;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwWin32ExitCode;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwServiceSpecificExitCode;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwCheckPoint;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwWaitHint;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwProcessId;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwServiceFlags;
    }
}
