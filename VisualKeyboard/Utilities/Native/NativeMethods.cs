namespace VisualKeyboard.Utilities.Native
{
    using System;
    using System.Runtime.InteropServices;

    static class NativeMethods
    {
        #region SendInput constants

        public enum EINPUT : uint
        {
            MOUSE = 0,
            KEYBOARD = 1,
            HARDWARE = 2,
        }
        public enum KEYEVENTF : uint
        {
            VIRTUALKEY = 0x0000,
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            UNICODE = 0x0004,
            SCANCODE = 0x0008,
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk; // 2 byte field
            public ushort wScan; // 2 byte field
            public KEYEVENTF dwFlags; // 4 byte field
            public uint time; // 4 byte field
            public UIntPtr dwExtraInfo; // filler
        }
        /**
         * the structure of the datatype is ordered in memory 
         * by the way they are defined
         * i.e. dy will always reference the 4 bytes after dx
         */
        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx; // 4 byte signed field
            public int dy; // 4 byte signed field
            public uint mouseData; // 4 byte field
            public uint dwFlags; // 4 byte field
            public uint time; // 4 byte field
            public UIntPtr dwExtraInfo; // filler
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }
        /**
         * requires that the fields are explicitly layed out 
         * inside the struct, in this case it is made such 
         * that the first bit of input data starts at the 
         * same place in memory
         */
        [StructLayout(LayoutKind.Explicit)]
        public struct UNIONINPUT
        {
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public EINPUT type; // input identifier 4 byte field
            public UNIONINPUT uinput; // input data
            public static int SIZE { get { return Marshal.SizeOf(typeof(INPUT)); } }
        }

        #endregion
        #region SendInput members

        ///<param name="cbSize"></param>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        #endregion

        #region WindowStyle members
        public enum GWL : int
        {
            WNDPROC = (-4),
            HINSTANCE = (-6),
            HWNDPARENT = (-8),
            STYLE = (-16),
            EXSTYLE = (-20),
            USERDATA = (-21),
            ID = (-12)
        }
        public const int GWL_EXSTYLE = (-20);
        public const int WS_EX_NOACTIVATE = 0x08000000;

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
            }
        }
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong32(IntPtr hWnd, GWL nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);


        public static IntPtr GetWindowLongPtr(IntPtr hWnd, GWL nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return GetWindowLongPtr32(hWnd, nIndex);
        }
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, GWL nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, GWL nIndex);


        #endregion
    }
}
