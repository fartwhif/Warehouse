using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Runtime.InteropServices;

namespace Warehouse
{
    public partial class PluginCore : PluginBase
    {
        private const string validFileChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ _-[].01234567890";
        private static string MakeValidFileName(string name)
        {
            string t = "";
            foreach (char c in name)
            {
                if (validFileChars.Contains(c.ToString()))
                    t += c;
                else
                    t += "_";
            }
            t = t.TrimEnd('.');
            return t.Trim();
        }
        private void Ding()
        {
            System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer
            {
                SoundLocation = WavFile
            };
            myPlayer.Play();
        }
        private void SendKey(string key, bool press, bool release)
        {
            Mapper.KeyInfo ki = Mapper.GetScanCode(key);
            if (press)
            {
                Input.SendKeyInput(ki.ScanCode, true, false);
            }
            if (release)
            {
                Input.SendKeyInput(ki.ScanCode, false, true);
            }
        }
        private void SendChatCommand(string cmd)
        {
            SendChatCommand(new string[] { cmd });
        }
        private void SendChatCommand(string[] cmds)
        {
            //Core.Foreground();
            foreach (string cmd in cmds)
            {
                DispatchChatToBoxWithPluginIntercept(cmd);
                //Input.SendKeyInputToAsheronsCall((ushort)ScanCodes.Return, true, false); //PRESS the Return key
                //Input.SendKeyInputToAsheronsCall((ushort)ScanCodes.Return, false, true); //RELEASE the Return key
                //Input.SendStringInputToAsheronsCall(cmd, hWnd);
                //Input.SendKeyInputToAsheronsCall((ushort)ScanCodes.Return, true, false); //PRESS the Return key
                //Input.SendKeyInputToAsheronsCall((ushort)ScanCodes.Return, false, true); //RELEASE the Return key
            }
        }
        /// <summary>
        /// This will first attempt to send the messages to all plugins. If no plugins set e.Eat to true on the message, it will then simply call InvokeChatParser.
        /// </summary>
        /// <param name="cmd"></param>
        /// <remarks>Copied from UtilityBelt source - THANK YOU Trevis!!!</remarks>
        private void DispatchChatToBoxWithPluginIntercept(string cmd)
        {
            if (!Decal_DispatchOnChatCommand(cmd))
            {
                Core.Actions.InvokeChatParser(cmd);
            }
        }
        [DllImport("Decal.dll")]
        private static extern int DispatchOnChatCommand(ref IntPtr str, [MarshalAs(UnmanagedType.U4)] int target);
        private static bool Decal_DispatchOnChatCommand(string cmd)
        {
            IntPtr bstr = Marshal.StringToBSTR(cmd);
            try
            {
                bool eaten = (DispatchOnChatCommand(ref bstr, 1) & 0x1) > 0;
                return eaten;
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
        /// <summary>
        /// doesn't work sometimes
        /// </summary>
        /// <param name="co"></param>
        /// <returns></returns>
        private double GetMyDistanceTo(CoordsObject co)
        {
            try
            {
                WorldObjectCollection woc = Core.WorldFilter.GetByObjectClass(ObjectClass.Player);
                CoordsObject myCoords = woc.Current.Coordinates();
                return myCoords.DistanceToCoords(co);
            }
            catch (Exception ex)
            {
                errorLogging.LogError(ErrorLogFile, ex);
            }
            return -1;
        }
    }
}
