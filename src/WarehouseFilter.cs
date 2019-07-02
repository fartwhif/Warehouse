using Decal.Adapter;
using System;
using System.Diagnostics;

namespace Warehouse
{
    [FriendlyName("WarehouseFilter")]
    public class WarehouseFilter : FilterBase
    {
        private System.Windows.Forms.Timer MainLoopTimer = null;
        private bool LoginAttempted = false;
        private int LoginPhase = 0;
        private Stopwatch TimeSinceLoginStuffAction;
        private bool gotCharList = false;
        private Stopwatch TimeSinceStartup;

        protected override void Shutdown()
        {
            ServerDispatch -= new EventHandler<NetworkMessageEventArgs>(FilterCore_ServerDispatch);
        }
        protected override void Startup()
        {
            TimeSinceStartup = Stopwatch.StartNew();
            ServerDispatch += new EventHandler<NetworkMessageEventArgs>(FilterCore_ServerDispatch);
            MainLoopTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            MainLoopTimer.Tick += MainLoopTimer_Tick;
            MainLoopTimer.Enabled = true;
            MainLoopTimer.Start();
        }
        private void MainLoopTimer_Tick(object sender, EventArgs e)
        {
            if (!WarehouseFilterGlobals.PluginCoreStarted && gotCharList && TimeSinceStartup.Elapsed.TotalSeconds > 15)
            {
                DoLoginStuff();
            }
        }
        /// <remarks>Copied from Mag-Filter by Magnus, THANK YOU!!!</remarks>
        internal void FilterCore_ServerDispatch(object sender, NetworkMessageEventArgs e)
        {
            if (e.Message.Type == 0xF658) // Character List
            {
                WarehouseFilterGlobals.CharacterSlots = Convert.ToInt32(e.Message["slotCount"]);
                WarehouseFilterGlobals.Characters.Clear();
                MessageStruct charactersStruct = e.Message.Struct("characters");
                for (int i = 0; i < charactersStruct.Count; i++)
                {
                    int character = Convert.ToInt32(charactersStruct.Struct(i)["character"]);
                    string name = Convert.ToString(charactersStruct.Struct(i)["name"]);
                    int deleteTimeout = Convert.ToInt32(charactersStruct.Struct(i)["deleteTimeout"]);
                    WarehouseFilterGlobals.Characters.Add(new Character(character, name, deleteTimeout));
                }
                WarehouseFilterGlobals.Characters.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
                gotCharList = true;
            }
        }
        private void DoLoginStuff()
        {
            if (LoginPhase == 0)
            {
                if (!LoginAttempted)
                {
                    LoginAttempted = true;
                    Core.Foreground();
                    //806x632
                    //Host.Actions.SetCursorPosition(350, 400);//crash
                    EnumWindowsItem enumw = new EnumWindowsItem(Core.Decal.Hwnd);
                    Input.MouseMoveAbsolute(enumw.Location.X + 350, enumw.Location.Y + 400);
                    TimeSinceLoginStuffAction = Stopwatch.StartNew();
                    LoginPhase = 1;
                }
            }
            else if (LoginPhase == 1)
            {
                if (TimeSinceLoginStuffAction.ElapsedMilliseconds > 1000)
                {
                    Core.Foreground();
                    Input.LeftClick();
                    TimeSinceLoginStuffAction = Stopwatch.StartNew();
                    LoginPhase = 0;
                }
            }
        }
    }
}
