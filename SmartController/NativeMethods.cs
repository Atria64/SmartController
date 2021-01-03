using System.Runtime.InteropServices;

namespace SmartController
{
    internal class NativeMethods
    {
        //クリック処理用
        [DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        internal const int MOUSEEVENTF_LEFTDOWN = 0x2;
        internal const int MOUSEEVENTF_LEFTUP = 0x4;
        
        //コンソール表示用
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AllocConsole();
    }
}
