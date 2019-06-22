using System;
using System.Collections;

namespace Warehouse
{
    internal class Mapper
    {
        public struct KeyInfo
        {
            public ushort ScanCode;
            public bool ShiftKey;
        }
        public static KeyInfo GetScanCode(string s)
        {
            KeyInfo ki = new KeyInfo();
            if (s.Length < 1)
            {
                return ki;
            }
            #region map
            ArrayList map = new ArrayList
            {
                new string[] { "N", "01 00", "ESC" },
                new string[] { "N", "02 00", "1" },
                new string[] { "Y", "02 00", "!" },
                new string[] { "Y", "03 00", "@" },
                new string[] { "N", "03 00", "2" },
                new string[] { "Y", "04 00", "#" },
                new string[] { "N", "04 00", "3" },
                new string[] { "Y", "05 00", "$" },
                new string[] { "N", "05 00", "4" },
                new string[] { "Y", "06 00", "%" },
                new string[] { "N", "06 00", "5" },
                new string[] { "N", "07 00", "6" },
                new string[] { "Y", "07 00", "^" },
                new string[] { "Y", "08 00", "&" },
                new string[] { "N", "08 00", "7" },
                new string[] { "Y", "09 00", "*" },
                new string[] { "N", "09 00", "8" },
                new string[] { "Y", "0A 00", "(" },
                new string[] { "N", "0A 00", "9" },
                new string[] { "Y", "0B 00", ")" },
                new string[] { "N", "0B 00", "0" },
                new string[] { "Y", "0C 00", "_" },
                new string[] { "N", "0C 00", "-" },
                new string[] { "Y", "0D 00", "+" },
                new string[] { "N", "0D 00", "=" },
                new string[] { "N", "0E 00", "Backspace" },
                new string[] { "N", "0F 00", "Tab" },
                new string[] { "Y", "10 00", "Q" },
                new string[] { "N", "10 00", "q" },
                new string[] { "Y", "11 00", "W" },
                new string[] { "N", "11 00", "w" },
                new string[] { "Y", "12 00", "E" },
                new string[] { "N", "12 00", "e" },
                new string[] { "Y", "13 00", "R" },
                new string[] { "N", "13 00", "r" },
                new string[] { "Y", "14 00", "T" },
                new string[] { "N", "14 00", "t" },
                new string[] { "T", "15 00", "Y" },
                new string[] { "N", "15 00", "y" },
                new string[] { "Y", "16 00", "U" },
                new string[] { "N", "16 00", "u" },
                new string[] { "Y", "17 00", "I" },
                new string[] { "N", "17 00", "i" },
                new string[] { "Y", "18 00", "O" },
                new string[] { "N", "18 00", "o" },
                new string[] { "Y", "19 00", "P" },
                new string[] { "N", "19 00", "p" },
                new string[] { "Y", "1A 00", "{" },
                new string[] { "N", "1A 00", "[" },
                new string[] { "Y", "1B 00", "}" },
                new string[] { "N", "1B 00", "]" },
                new string[] { "N", "1C 00", "Enter" },
                new string[] { "N", "1D 00", "Left Control" },
                new string[] { "N", "1D E0", "Right Control" },
                new string[] { "Y", "1E 00", "A" },
                new string[] { "N", "1E 00", "a" },
                new string[] { "Y", "1F 00", "S" },
                new string[] { "N", "1F 00", "s" },
                new string[] { "Y", "20 00", "D" },
                new string[] { "N", "20 00", "d" },
                new string[] { "N", "20 E0", "Mute" },
                new string[] { "Y", "21 00", "F" },
                new string[] { "N", "21 00", "f" },
                new string[] { "Y", "22 00", "G" },
                new string[] { "N", "22 00", "g" },
                new string[] { "N", "22 E0", "Play" },
                new string[] { "Y", "23 00", "H" },
                new string[] { "N", "23 00", "h" },
                new string[] { "Y", "24 00", "J" },
                new string[] { "N", "24 00", "j" },
                new string[] { "N", "24 E0", "Stop" },
                new string[] { "Y", "25 00", "K" },
                new string[] { "N", "25 00", "k" },
                new string[] { "Y", "26 00", "L" },
                new string[] { "N", "26 00", "l" },
                new string[] { "Y", "27 00", ":" },
                new string[] { "N", "27 00", ";" },
                new string[] { "Y", "28 00", "\"" },
                new string[] { "N", "28 00", "'" },
                new string[] { "Y", "29 00", "~" },
                new string[] { "N", "29 00", "`" },
                new string[] { "N", "2A 00", "Left Shift" },
                new string[] { "Y", "2B 00", "|" },
                new string[] { "N", "2B 00", "\\" },
                new string[] { "Y", "2C 00", "Z" },
                new string[] { "N", "2C 00", "z" },
                new string[] { "Y", "2D 00", "X" },
                new string[] { "N", "2D 00", "x" },
                new string[] { "Y", "2E 00", "C" },
                new string[] { "N", "2E 00", "c" },
                new string[] { "N", "2E E0", "Volume Down" },
                new string[] { "Y", "2F 00", "V" },
                new string[] { "N", "2F 00", "v" },
                new string[] { "Y", "30 00", "B" },
                new string[] { "N", "30 00", "b" },
                new string[] { "N", "30 E0", "Volume Up" },
                new string[] { "Y", "31 00", "N" },
                new string[] { "N", "31 00", "n" },
                new string[] { "Y", "32 00", "M" },
                new string[] { "N", "32 00", "m" },
                new string[] { "N", "32 E0", "Web" },
                new string[] { "Y", "33 00", "<" },
                new string[] { "N", "33 00", "," },
                new string[] { "Y", "34 00", ">" },
                new string[] { "N", "34 00", "." },
                new string[] { "Y", "35 00", "?" },
                new string[] { "N", "35 00", "/" },
                new string[] { "N", "36 00", "Right Shift" },
                new string[] { "N", "37 00", "Print Screen" },
                new string[] { "N", "37 E0", "Power" },
                new string[] { "N", "38 00", "Left Alt" },
                new string[] { "N", "38 E0", "Right Alt" },
                new string[] { "N", "39 00", " " },
                new string[] { "N", "3A 00", "Caps Lock" },
                new string[] { "N", "3B 00", "F1" },
                new string[] { "N", "3C 00", "F2" },
                new string[] { "N", "3D 00", "F3" },
                new string[] { "N", "3E 00", "F4" },
                new string[] { "N", "3F 00", "F5" },
                new string[] { "N", "40 00", "F6" },
                new string[] { "N", "41 00", "F7" },
                new string[] { "N", "42 00", "F8" },
                new string[] { "N", "43 00", "F9" },
                new string[] { "N", "44 00", "F10" },
                new string[] { "N", "45 00", "Num Lock" },
                new string[] { "N", "46 00", "Scroll Lock" },
                new string[] { "N", "47 00", "NumPad Home" },
                new string[] { "N", "47 00", "NumPad 7" },
                new string[] { "N", "47 E0", "Home" },
                new string[] { "N", "48 00", "NumPad Up" },
                new string[] { "N", "48 00", "NumPad 8" },
                new string[] { "N", "48 E0", "Up" },
                new string[] { "N", "49 00", "NumPad Page Up" },
                new string[] { "N", "49 00", "NumPad 9" },
                new string[] { "N", "49 E0", "Page Up" },
                new string[] { "N", "4A 00", "NumPad -" },
                new string[] { "N", "4B 00", "NumPad Left" },
                new string[] { "N", "4B 00", "NumPad 4" },
                new string[] { "N", "4B E0", "Left" },
                new string[] { "N", "4C 00", "NumPad 5" },
                new string[] { "N", "4D 00", "NumPad Right" },
                new string[] { "N", "4D 00", "NumPad 6" },
                new string[] { "N", "4D E0", "Right" },
                new string[] { "N", "4E 00", "NumPad +" },
                new string[] { "N", "4F 00", "NumPad End" },
                new string[] { "N", "4F 00", "NumPad 1" },
                new string[] { "N", "4F E0", "End" },
                new string[] { "N", "50 00", "NumPad Down" },
                new string[] { "N", "50 00", "NumPad 2" },
                new string[] { "N", "50 E0", "Down" },
                new string[] { "N", "51 00", "NumPad Page Down" },
                new string[] { "N", "51 00", "NumPad 3" },
                new string[] { "N", "51 E0", "Page Down" },
                new string[] { "N", "52 00", "NumPad Insert" },
                new string[] { "N", "52 00", "NumPad 0" },
                new string[] { "N", "52 E0", "Insert" },
                new string[] { "N", "53 00", "NumPad ." },
                new string[] { "N", "53 00", "NumPad Delete" },
                new string[] { "N", "53 E0", "Delete" },
                new string[] { "N", "57 00", "F11" },
                new string[] { "N", "58 00", "F12" },
                new string[] { "N", "5B E0", "Left Windows" },
                new string[] { "N", "5C E0", "Right Windows" },
                new string[] { "N", "5D E0", "Windows Menu" },
                new string[] { "N", "E0 1C", "NumPad Enter" }
            };
            #endregion

            foreach (string[] entry in map)
            {
                if (entry[2] == s)
                {
                    string i = entry[1];
                    i = i.Substring(0, 2);
                    ki.ScanCode = (ushort)Convert.ToInt32(i, 16);
                    ki.ShiftKey = (entry[0] == "Y") ? true : false;
                    return ki;
                }
            }
            return ki;
        }
    }
}
