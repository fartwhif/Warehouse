using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Interop.Inject;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Warehouse
{
    [View("warehouse.ViewXML.mainView.xml")]
    [WireUpControlEvents]
    public partial class PluginCore
    {
        //[ControlReference("cbLogPlayersNearby")]
        //private CheckBoxWrapper cbLogPlayersNearby;

        //[ControlReference("cbLogOnDeath")]
        //private CheckBoxWrapper cbLogOnDeath;

        //[ControlReference("stNumPlayers1")]
        //private StaticWrapper stNumPlayers1;

        //[ControlReference("stNumPlayers2")]
        //private StaticWrapper stNumPlayers2;

        //[ControlReference("cbLogBackIn")]
        //private CheckBoxWrapper cbLogBackIn;

        //[ControlReference("cbPressPauseOnLogIn")]
        //private CheckBoxWrapper cbPressPauseOnLogIn;

        //[ControlReference("stDoingNow1")]
        //private StaticWrapper stDoingNow1;

        //[ControlReference("stDoingNow2")]
        //private StaticWrapper stDoingNow2;

        //[ControlReference("btnStart")]
        //private ButtonWrapper btnStart;

        //[ControlReference("btnStop")]
        //private ButtonWrapper btnStop;

        //[ControlEvent("btnStart", "Click")]
        //private void onClick(object sender, Decal.Adapter.ControlEventArgs args)
        //{
        //    toggleVirindiTank(true);
        //    DoingNow = OverallState.idle;
        //}
        //[ControlEvent("btnStop", "Click")]
        //private void onClick2(object sender, Decal.Adapter.ControlEventArgs args)
        //{
        //    toggleVirindiTank(false);
        //    DoingNow = OverallState.off;
        //}

        [ControlEvent("btnDebug", "Click")]
        private void onDebugClick(object sender, Decal.Adapter.ControlEventArgs args)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
            Debugger.Break();
        }
    }
}