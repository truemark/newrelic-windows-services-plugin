using System;
using System.Runtime.InteropServices;

namespace newrelic_windows_services_plugin
{
    static class UNMANAGED
    {
        internal const int ERROR_INSUFFICIENT_BUFFER = 0x7A;
        internal const int SC_STATUS_PROCESS_INFO = 0;

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool QueryServiceStatusEx(SafeHandle hService, int infoLevel, IntPtr lpBuffer, uint cbBufSize, out uint pcbBytesNeeded);

        public static int findProcessId(SafeHandle serviceHandle)
        {
            IntPtr ptrZero = IntPtr.Zero;
            try
            {
                UInt32 statusInfoBytesRequired;
                QueryServiceStatusEx(serviceHandle, SC_STATUS_PROCESS_INFO, ptrZero, 0, out statusInfoBytesRequired);

                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    ptrZero = Marshal.AllocHGlobal((int)statusInfoBytesRequired);
                    if (QueryServiceStatusEx(serviceHandle, SC_STATUS_PROCESS_INFO, ptrZero, statusInfoBytesRequired, out statusInfoBytesRequired))
                    {
                        var serviceStatus = new SERVICE_STATUS_PROCESS();
                        Marshal.PtrToStructure(ptrZero, serviceStatus);
                        return (int)serviceStatus.dwProcessId;
                    }
                }
            }
            finally
            {
                if (ptrZero != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrZero);
                }
            }
            return -1;
        }
    }
}