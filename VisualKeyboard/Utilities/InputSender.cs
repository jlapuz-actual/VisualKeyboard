namespace VisualKeyboard.Utilities
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    class InputSender
    {
        public enum EINPUT : UInt32
        {
            MOUSE = 0,
            KEYBOARD = 1,
            HARDWARE = 2,
        }
        public enum KEYEVENTF : UInt32
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
            public UInt16 wVk; // 2 byte field
            public UInt16 wScan; // 2 byte field
            public KEYEVENTF dwFlags; // 4 byte field
            public UInt32 time; // 4 byte field
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
            public Int32 dx; // 4 byte signed field
            public Int32 dy; // 4 byte signed field
            public UInt32 mouseData; // 4 byte field
            public UInt32 dwFlags; // 4 byte field
            public UInt32 time; // 4 byte field
            public UIntPtr dwExtraInfo; // filler
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
        }
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public UInt32 type; // input identifier 4 byte field
            [FieldOffset(4)]
            public UNIONINPUT uinput; // input data
            public static int SIZE { get { return Marshal.SizeOf(typeof(INPUT)); } }
        }
        ///<param name="cbSize"></param>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        /**
         * <summary>Wrapper for SendInput user32 API, sends both key down and key up events</summary>
         * 
         * <param name="type">defines type of <c>keys</c> from enum <c>KEYEVENTF</c></param> 
         * <param name="keys"></param>
         * <returns> returns number of sent keys, this should equal double of the length of <paramref name="keys"/> 
         * as keydown and keyup events are triggered for each entry in the array</returns>
         * 
         */
        public uint SendVirtualKeystroke(UInt16[] keys)
        {
            INPUT[] pInputs = keys.Select(key => new INPUT
            {
                type = (uint)EINPUT.KEYBOARD,
                uinput = new UNIONINPUT
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key,
                        wScan = 0,
                        dwFlags = KEYEVENTF.VIRTUALKEY,
                        time = 0,
                        dwExtraInfo = UIntPtr.Zero
                    }
                }
            }).ToArray();
            var sent = SendInput((uint)pInputs.Length, pInputs, INPUT.SIZE);
            pInputs = keys.Select(key => new INPUT
            {
                type = (uint)EINPUT.KEYBOARD,
                uinput = new UNIONINPUT
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key,
                        wScan = 0,
                        dwFlags = KEYEVENTF.KEYUP,
                        time = 0,
                        dwExtraInfo = UIntPtr.Zero
                    }
                }
            }).ToArray();
            sent += SendInput((uint)pInputs.Length, pInputs, INPUT.SIZE);

            return sent;
        }
        public uint SendVirtualKeyDown(UInt16[] keys)
        {
            if (keys.Length == 0) { return 0; }
            INPUT[] pInputs = keys.Select(key => new INPUT
            {
                type = (uint)EINPUT.KEYBOARD,
                uinput = new UNIONINPUT
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key,
                        wScan = 0,
                        dwFlags = KEYEVENTF.VIRTUALKEY,
                        time = 0,
                        dwExtraInfo = UIntPtr.Zero
                    }
                }
            }).ToArray();
            var sent = SendInput((uint)pInputs.Length, pInputs, INPUT.SIZE);
            return sent;
        }
        public uint SendVirtualKeyUp(UInt16[] keys)
        {
            if (keys.Length == 0) { return 0; }
            INPUT[] pInputs = keys.Select(key => new INPUT
            {
                type = (uint)EINPUT.KEYBOARD,
                uinput = new UNIONINPUT
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key,
                        wScan = 0,
                        dwFlags = KEYEVENTF.KEYUP,
                        time = 0,
                        dwExtraInfo = UIntPtr.Zero
                    }
                }
            }).ToArray();
            var sent = SendInput((uint)pInputs.Length, pInputs, INPUT.SIZE);
            return sent;
        }
        public uint SendScanKeyDown(UInt16[] keys)
        {
            if (keys.Length == 0) { return 0; }
            INPUT[] pInputs = keys.Select(key => new INPUT
            {
                type = (uint)EINPUT.KEYBOARD,
                uinput = new UNIONINPUT
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = key,
                        dwFlags = KEYEVENTF.SCANCODE,
                        time = 0,
                        dwExtraInfo = UIntPtr.Zero
                    }
                }
            }).ToArray();
            var sent = SendInput((uint)pInputs.Length, pInputs, INPUT.SIZE);
            return sent;
        }
        public uint SendScanKeyUp(UInt16[] keys)
        {
            if (keys.Length == 0) { return 0; }
            INPUT[] pInputs = keys.Select(key => new INPUT
            {
                type = (uint)EINPUT.KEYBOARD,
                uinput = new UNIONINPUT
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = 0,
                        wScan = key,
                        dwFlags = KEYEVENTF.KEYUP,
                        time = 0,
                        dwExtraInfo = UIntPtr.Zero
                    }
                }
            }).ToArray();
            var sent = SendInput((uint)pInputs.Length, pInputs, INPUT.SIZE);
            return sent;
        }
    }
}
