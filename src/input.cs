using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Warehouse
{
    public enum ScanCodes : int
    {
        NotAssigned = -1,
        Escape = 0x01,
        One = 0x02,
        Two = 0x03,
        Three = 0x04,
        Four = 0x05,
        Five = 0x06,
        Six = 0x07,
        Seven = 0x08,
        Eight = 0x09,
        Nine = 0x0A,
        Zero = 0x0B,
        Minus = 0x0C,
        Equals = 0x0D,
        Backspace = 0x0E,
        Tab = 0x0F,
        Q = 0x10,
        W = 0x11,
        E = 0x12,
        R = 0x13,
        T = 0x14,
        Y = 0x15,
        U = 0x16,
        I = 0x17,
        O = 0x18,
        P = 0x19,
        LBracket = 0x1A,
        RBracket = 0x1B,
        Return = 0x1C,
        LControl = 0x1D,
        A = 0x1E,
        S = 0x1F,
        D = 0x20,
        F = 0x21,
        G = 0x22,
        H = 0x23,
        J = 0x24,
        K = 0x25,
        L = 0x26,
        Semicolon = 0x27,
        Apostrophe = 0x28,
        Grave = 0x29,
        LShift = 0x2A,
        Backslash = 0x2B,
        Z = 0x2C,
        X = 0x2D,
        C = 0x2E,
        V = 0x2F,
        B = 0x30,
        N = 0x31,
        M = 0x32,
        Comma = 0x33,
        Period = 0x34,
        Slash = 0x35,
        RShift = 0x36,
        Multiply = 0x37,
        LMenu = 0x38,
        Space = 0x39,
        CapsLock = 0x3A,
        F1 = 0x3B,
        F2 = 0x3C,
        F3 = 0x3D,
        F4 = 0x3E,
        F5 = 0x3F,
        F6 = 0x40,
        F7 = 0x41,
        F8 = 0x42,
        F9 = 0x43,
        F10 = 0x44,
        NumLock = 0x45,
        ScrollLock = 0x46,
        NumPad7 = 0x47,
        NumPad8 = 0x48,
        NumPad9 = 0x49,
        Subtract = 0x4A,
        NumPad4 = 0x4B,
        NumPad5 = 0x4C,
        NumPad6 = 0x4D,
        Add = 0x4E,
        NumPad1 = 0x4F,
        NumPad2 = 0x50,
        NumPad3 = 0x51,
        NumPad0 = 0x52,
        Decimal = 0x53,
        F11 = 0x57,
        F12 = 0x58,
        F13 = 0x64,
        F14 = 0x65,
        F15 = 0x66,
        Kana = 0x70,
        Convert = 0x79,
        NoConvert = 0x7B,
        Yen = 0x7D,
        NumPadEquals = 0x8D,
        Circumflex = 0x90,
        At = 0x91,
        Colon = 0x92,
        Underline = 0x93,
        Kanji = 0x94,
        Stop = 0x95,
        Ax = 0x96,
        Unlabeled = 0x97,
        NumPadEnter = 0x9C,
        RControl = 0x9D,
        NumPadComma = 0xB3,
        Divide = 0xB5,
        SysRq = 0xB7,
        RMenu = 0xB8,
        Home = 0xC7,
        Up = 0xC8,
        Prior = 0xC9,
        Left = 0xCB,
        Right = 0xCD,
        End = 0xCF,
        Down = 0xD0,
        Next = 0xD1,
        Insert = 0xD2,
        Delete = 0xD3,
        LWin = 0xDB,
        RWin = 0xDC,
        Apps = 0xDD
    }
    public class Input
    {
        internal enum INPUT_TYPE : uint
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2,
        }
        [Flags()]
        internal enum KEYEVENTF : uint
        {
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            UNICODE = 0x0004,
            SCANCODE = 0x0008,
        }
        internal enum MAPVK_MAPTYPES : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
            MAPVK_VK_TO_VSC_EX = 0x4,
        }
        [Flags()]
        internal enum MOUSEEVENTF : uint
        {
            MOVE = 0x0001,  // mouse move 
            LEFTDOWN = 0x0002,  // left button down
            LEFTUP = 0x0004,  // left button up
            RIGHTDOWN = 0x0008,  // right button down
            RIGHTUP = 0x0010,  // right button up
            MIDDLEDOWN = 0x0020,  // middle button down
            MIDDLEUP = 0x0040,  // middle button up
            XDOWN = 0x0080,  // x button down 
            XUP = 0x0100,  // x button down
            WHEEL = 0x0800,  // wheel button rolled
            VIRTUALDESK = 0x4000,  // map to entire virtual desktop
            ABSOLUTE = 0x8000,  // absolute move
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MOUSEEVENTF dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public KEYEVENTF dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Explicit)]
        internal struct INPUT
        {
            [FieldOffset(0)]
            public INPUT_TYPE type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "MapVirtualKey", SetLastError = false)]
        internal static extern uint MapVirtualKey(uint uCode, MAPVK_MAPTYPES uMapType);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetMessageExtraInfo();
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        [DllImportAttribute("User32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string ClassName, string WindowName);
        [DllImportAttribute("User32.dll", SetLastError = true)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        internal static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
        /// <summary>
        ///     Retrieves a handle to the foreground window (the window with which the user is currently working). The system
        ///     assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
        ///     <para>See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633505%28v=vs.85%29.aspx for more information.</para>
        /// </summary>
        /// <returns>
        ///     C++ ( Type: Type: HWND )<br /> The return value is a handle to the foreground window. The foreground window
        ///     can be NULL in certain circumstances, such as when a window is losing activation.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
        internal static void SendMouseInput(MOUSEEVENTF dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo)
        {
            INPUT input = new INPUT
            {
                mi = new MOUSEINPUT(),
                type = INPUT_TYPE.INPUT_MOUSE
            };
            input.mi.dwFlags = dwFlags;
            input.mi.dx = (int)dx;
            input.mi.dy = (int)dy;
            input.mi.mouseData = dwData;
            input.mi.time = 0;
            input.mi.dwExtraInfo = dwExtraInfo;
            SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }
        internal static void SendKeyInput(ushort scanCode, bool press, bool release)
        {
            if (!press && !release)
            {
                return;
            }
            int numInputs = 0;
            if (press && release)
            {
                numInputs = 2;
            }
            else
            {
                numInputs = 1;
            }
            INPUT[] inputs = new INPUT[numInputs];
            int curInput = 0;
            if (press)
            {
                INPUT input = new INPUT
                {
                    ki = new KEYBDINPUT
                    {
                        wScan = scanCode,
                        time = 0,
                        dwFlags = KEYEVENTF.SCANCODE
                    }
                };
                if ((scanCode & 0x80) > 0)
                {
                    input.ki.dwFlags |= KEYEVENTF.EXTENDEDKEY;
                }
                System.Diagnostics.Debug.WriteLine(input.ki.wScan);
                input.ki.dwExtraInfo = GetMessageExtraInfo();
                input.type = INPUT_TYPE.INPUT_KEYBOARD;
                inputs[curInput] = input;
                curInput++;
            }
            if (release)
            {
                INPUT input = new INPUT
                {
                    ki = new KEYBDINPUT
                    {
                        wScan = scanCode,
                        time = 0,
                        dwFlags = (KEYEVENTF.KEYUP | KEYEVENTF.SCANCODE)
                    }
                };
                if ((scanCode & 0x80) > 0)
                {
                    input.ki.dwFlags |= KEYEVENTF.EXTENDEDKEY;
                }
                input.ki.dwExtraInfo = GetMessageExtraInfo();
                input.type = INPUT_TYPE.INPUT_KEYBOARD;
                inputs[curInput] = input;
            }
            SendInput((uint)numInputs, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        internal static void SendStringInput(string p, IntPtr hWnd)
        {
            Mapper mapper = new Mapper();
            ushort shiftScanCode = Mapper.GetScanCode("Left Shift").ScanCode;
            for (int i = 0; i < p.Length; i++)
            {
                Mapper.KeyInfo ki = Mapper.GetScanCode(p.Substring(i, 1));
                if (ki.ShiftKey)
                {
                    SendKeyInput(shiftScanCode, true, false);
                }

                SendKeyInput(ki.ScanCode, true, false);
                SendKeyInput(ki.ScanCode, false, true);
                if (ki.ShiftKey)
                {
                    SendKeyInput(shiftScanCode, false, true);
                }
            }
        }
        /// <summary>
        ///Moves the mouse to the given relative (x,y) coordinates.
        /// </summary>
        public static void MouseMoveRelative(int x, int y)
        {
            int cur_x = Cursor.Position.X;
            int cur_y = Cursor.Position.Y;
            int new_x = cur_x + x;
            int new_y = cur_y + y;
            MouseMoveAbsolute(new_x, new_y);
        }
        /// <summary>
        ///Moves the mouse to the given absolute (x,y) coordinates.
        /// </summary>
        public static void MouseMoveAbsolute(int x, int y)
        {
            x = x * 65535 / Screen.PrimaryScreen.Bounds.Width;
            y = y * 65535 / Screen.PrimaryScreen.Bounds.Height;
            SendMouseInput((MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.MOVE), (uint)x, (uint)y, 0, UIntPtr.Zero);
        }
        /// <summary>
        ///Moves the mouse wheel by given amount.
        /// </summary>
        public static void MouseWheelMove(int amount)
        {
            SendMouseInput(MOUSEEVENTF.WHEEL, 0, 0, (uint)amount, UIntPtr.Zero);
        }
        /// <summary>
        ///Sends a left mouse button up event at the current cursor position.
        /// </summary>
        public static void LeftUp()
        {
            SendMouseInput(MOUSEEVENTF.LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }
        /// <summary>
        ///Sends a right mouse button up event at the current cursor position.
        /// </summary>
        public static void RightUp()
        {
            SendMouseInput(MOUSEEVENTF.RIGHTUP, 0, 0, 0, UIntPtr.Zero);
        }
        /// <summary>
        ///Sends a middle mouse button up event at the current cursor position.
        /// </summary>
        public static void MiddleUp()
        {
            SendMouseInput(MOUSEEVENTF.MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
        }
        /// <summary>
        ///Sends a middle mouse button down event at the current cursor position.
        /// </summary>
        public static void MiddleDown()
        {
            SendMouseInput(MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
        }
        /// <summary>
        ///Sends a right mouse button down event at the current cursor position.
        /// </summary>
        public static void RightDown()
        {
            SendMouseInput(MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
        }
        /// <summary>
        ///Sends a left mouse button down event at the current cursor position.
        /// </summary>
        public static void LeftDown()
        {
            SendMouseInput(MOUSEEVENTF.LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        }
        /// <summary>
        ///Sends a middle mouse button double click at the current cursor position.
        /// </summary>
        public static void MiddleDoubleClick()
        {
            SendMouseInput(MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }
        /// <summary>
        ///Sends a right mouse button double click at the current cursor position.
        /// </summary>
        public static void RightDoubleClick()
        {
            SendMouseInput(MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.RIGHTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.RIGHTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }
        /// <summary>
        ///Sends a left mouse button double click at the current cursor position.
        /// </summary>
        public static void LeftDoubleClick()
        {
            SendMouseInput(MOUSEEVENTF.LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.LEFTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.LEFTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }
        /// <summary>
        ///Sends a right mouse button click at the current cursor position.
        /// </summary>
        public static void RightClick()
        {
            SendMouseInput(MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.RIGHTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }
        /// <summary>
        ///Sends a left mouse button click at the current cursor position.
        /// </summary>
        public static void LeftClick()
        {
            SendMouseInput(MOUSEEVENTF.LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.LEFTUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }
        /// <summary>
        ///Sends a middle mouse button click at the current cursor position.
        /// </summary>
        public static void MiddleClick()
        {
            SendMouseInput(MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
            SendMouseInput(MOUSEEVENTF.MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(100);
        }
    }
}