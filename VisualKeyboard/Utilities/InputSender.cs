namespace VisualKeyboard.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;

    class InputSender
    {

        public static ReadOnlyDictionary<string, ushort[]> PlainTextToScanCodes
        {
            get
            {
                return new ReadOnlyDictionary<string, ushort[]>
                    (
                    new Dictionary<string, ushort[]>()
                    {
                        { "ESC", new ushort[] { 0x01 } },
                        { "1", new ushort[] { 0x02 } },
                        { "2", new ushort[] { 0x03 } },
                        { "3", new ushort[] { 0x04 } },
                        { "4", new ushort[] { 0x05 } },
                        { "5", new ushort[] { 0x06 } },
                        { "6", new ushort[] { 0x07 } },
                        { "7", new ushort[] { 0x08 } },
                        { "8", new ushort[] { 0x09 } },
                        { "9", new ushort[] { 0x0a } },
                        { "0", new ushort[] { 0x0b } },
                        { @"-", new ushort[] { 0x0c } },
                        { @"=", new ushort[] { 0x0d } },
                        { "BACKSPACE", new ushort[] { 0x0e } },
                        { "TAB", new ushort[] { 0x0F } },
                        { "Q", new ushort[] { 0x10 } },
                        { "W", new ushort[] { 0x11 } },
                        { "E", new ushort[] { 0x12 } },
                        { "R", new ushort[] { 0x13 } },
                        { "T", new ushort[] { 0x14 } },
                        { "Y", new ushort[] { 0x15 } },
                        { "U", new ushort[] { 0x16 } },
                        { "I", new ushort[] { 0x17 } },
                        { "O", new ushort[] { 0x18 } },
                        { "P", new ushort[] { 0x19 } },
                        { @"[", new ushort[] { 0x1a } },
                        { @"]", new ushort[] { 0x1b } },
                        { "ENTER", new ushort[] { 0x1c } },
                        { "LCTRL", new ushort[] { 0x1d } },
                        { "A", new ushort[] { 0x1e } },
                        { "S", new ushort[] { 0x1f } },
                        { "D", new ushort[] { 0x20 } },
                        { "F", new ushort[] { 0x21 } },
                        { "G", new ushort[] { 0x22 } },
                        { "H", new ushort[] { 0x23 } },
                        { "J", new ushort[] { 0x24 } },
                        { "K", new ushort[] { 0x25 } },
                        { "L", new ushort[] { 0x26 } },
                        { ";", new ushort[] { 0x27 } },
                        { @"'", new ushort[] { 0x28 } },
                        { @"`", new ushort[] { 0x29 } },
                        { "LSHIFT", new ushort[] { 0x2a } },
                        { @"\", new ushort[] { 0x2b } },
                        { "Z", new ushort[] { 0x2c } },
                        { "X", new ushort[] { 0x2d } },
                        { "C", new ushort[] { 0x2e } },
                        { "V", new ushort[] { 0x2f } },
                        { "B", new ushort[] { 0x30 } },
                        { "N", new ushort[] { 0x31 } },
                        { "M", new ushort[] { 0x32 } },
                        { @",", new ushort[] { 0x33 } },
                        { @".", new ushort[] { 0x34 } },
                        { @"/", new ushort[] { 0x35 } },
                        { "RSHIFT", new ushort[] { 0x36 } },
                        { "PTSCR", new ushort[] { 0x37 } },
                        { "LALT", new ushort[] { 0x38 } },
                        { "SPACE", new ushort[] { 0x39 } },
                        { "CAPSLOCK", new ushort[] { 0x3a } },
                        { "F1", new ushort[] { 0x3b } },
                        { "F2", new ushort[] { 0x3c } },
                        { "F3", new ushort[] { 0x3d } },
                        { "F4", new ushort[] { 0x3e } },
                        { "F5", new ushort[] { 0x3f } },
                        { "F6", new ushort[] { 0x40 } },
                        { "F7", new ushort[] { 0x41 } },
                        { "F8", new ushort[] { 0x42 } },
                        { "F9", new ushort[] { 0x43 } },
                        { "F10", new ushort[] { 0x44 } },
                        { "NUMLOCK", new ushort[] { 0x45 } },
                        { "SCROLLLOCK", new ushort[] { 0x46 } },                   
                        { "UPARROW", new ushort[] { 0x48 } },                   
                        { "LEFTARROW", new ushort[] { 0x4b } },                   
                        { "RIGHTARROW", new ushort[] { 0x4d } },                   
                        { "DOWNARROW", new ushort[] { 0x50 } },                   
                    }
                    );
                
            }

        }

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
