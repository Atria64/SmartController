using System.Drawing;
using System.Runtime.InteropServices;

namespace SmartController
{
    internal class NativeMethods
    {
        #region Win32API
        //クリック処理用
        [DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        internal const int MOUSEEVENTF_LEFTDOWN = 0x2;
        internal const int MOUSEEVENTF_LEFTUP = 0x4;
        internal const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        internal const int MOUSEEVENTF_RIGHTUP = 0x10;

        //マウスポインタ移動用
        [DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern void SetCursorPos(int X, int Y);

        //マウスポインタ座標取得用
        [DllImport("User32.dll")]
        static extern bool GetCursorPos(out POINT lppoint);
        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int X { get; set; }
            public int Y { get; set; }
            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        //コンソール表示用
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AllocConsole();
        #endregion
    }
}
