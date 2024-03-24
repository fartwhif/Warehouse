using ACWatchDog.Interop;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Warehouse
{
    [FriendlyName("Warehouse")]
    public partial class PluginCore : PluginBase
    {
        private int TradePartnerId = 0;
        private string TradePartnerName = "";
        private Stopwatch prevTradeTime = Stopwatch.StartNew();
        private System.Windows.Forms.Timer StackTimer = null;
        private System.Windows.Forms.Timer CramTimer = null;
        private System.Windows.Forms.Timer MainLoopTimer = null;
        private System.Windows.Forms.Timer ItemsTradeAddTimer = null;
        private bool WantToSyncItems = false;
        private bool WantToJump = false;
        private bool ItemScanInProgress = false;
        private int prevPlayerCount;
        private Stopwatch TimeSinceScanFinished;
        private Stopwatch TimeSinceLoginStarted;
        private Stopwatch TimeSinceLogoff;
        private List<WorldObject> PendingItemsToTradeAdd = new List<WorldObject>();
        private string ErrorLogFile => DataDir + "errorLog.txt";
        private string WavFile => Path + "ding.wav";
        private bool loggedIn = false;
        private bool LoginAttempted = false;
        private int LoginPhase = 0;
        private int? CharToLogin = null;
        private Stopwatch TimeSinceLoginStuffAction;
        private List<int> PendingTradeGiveItems = new List<int>();
        private ChatMessage PendingRetrieveCommand = null;
        private Queue<WorldObject> PendingItemsToTradeAdd2 = new Queue<WorldObject>();
        private int[] GivingItems = null;
        private bool FirstInner = true;
        private WorldObject StackItem1;
        private WorldObject StackItem2;
        private Stopwatch TimeSinceWatchDogBark = null;
        private bool WatchDogRegistered = false;

        private void DoLoginStuff()
        {
            if (LoginPhase == 0)
            {
                if (CharToLogin.HasValue && !LoginAttempted && TimeSinceLogoff != null && TimeSinceLogoff.ElapsedMilliseconds > 10000)
                {
                    LoginAttempted = true;
                    Core.Foreground();
                    EnumWindowsItem enumw = new EnumWindowsItem(Core.Decal.Hwnd);
                    int index = CharSelectCoordFinder.GetCharacterIndexById(WarehouseFilterGlobals.Characters, CharToLogin.Value);
                    if (index < 0)
                    {
                        throw new Exception("failed to get character to login's index in the list");
                    }
                    Vector2D loc = CharSelectCoordFinder.GetCharacterListItemLocationByIndex(WarehouseFilterGlobals.CharacterSlots, index);
                    Input.MouseMoveAbsolute(enumw.Location.X + loc.X, enumw.Location.Y + loc.Y);
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
                    LoginPhase = 2;
                }
            }
            else if (LoginPhase == 2)
            {
                if (TimeSinceLoginStuffAction.ElapsedMilliseconds > 1000)
                {
                    //806x632
                    //Host.Actions.SetCursorPosition(350, 400);//crash
                    Core.Foreground();
                    EnumWindowsItem enumw = new EnumWindowsItem(Core.Decal.Hwnd);
                    Input.MouseMoveAbsolute(enumw.Location.X + 350, enumw.Location.Y + 400);
                    TimeSinceLoginStuffAction = Stopwatch.StartNew();
                    LoginPhase = 3;
                }
            }
            else if (LoginPhase == 3)
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
        protected override void Startup()
        {
            WarehouseFilterGlobals.PluginCoreStarted = true;
            try
            {
                InitCharStats();
                InitWorldFilter();
                InitChatEvents();
                MainLoopTimer = new System.Windows.Forms.Timer
                {
                    Interval = 1000
                };
                MainLoopTimer.Tick += MainLoopTimer_Tick;
                MainLoopTimer.Enabled = true;
                MainLoopTimer.Start();

                CramTimer = new System.Windows.Forms.Timer
                {
                    Interval = 500
                };
                CramTimer.Tick += CramTimer_Tick;
                CramTimer.Enabled = false;
                CramTimer.Start();

                StackTimer = new System.Windows.Forms.Timer
                {
                    Interval = 500
                };
                StackTimer.Tick += StackTimer_Tick;
                StackTimer.Enabled = false;
                StackTimer.Start();

                ItemsTradeAddTimer = new System.Windows.Forms.Timer
                {
                    Interval = 10,
                };
                ItemsTradeAddTimer.Tick += ItemsTradeAddTimer_Tick;
                ItemsTradeAddTimer.Enabled = false;
                ItemsTradeAddTimer.Start();

                DBOpen();
            }
            catch (Exception ex)
            {
                errorLogging.LogError(ErrorLogFile, ex);
            }
        }
        protected override void Shutdown()
        {
            try
            {
                DestroyChatEvents();
                DestroyCharStats();
                DestroyWorldFilter();

                //DBClose();
            }
            catch (Exception ex)
            {
                errorLogging.LogError(ErrorLogFile, ex);
            }
        }
        private void MainLoopTimer_Tick(object sender, EventArgs e)
        {
            paket _ffg  = null;
            do
            {
                _ffg = null;
                if (WarehouseFilterGlobals.messages.Any())
                {
                    _ffg = WarehouseFilterGlobals.messages.Dequeue();
                    if (_ffg != null)
                    {
                        if (_ffg.Type == 0x02CD)
                        {
                            WarehouseFilterGlobals.MostRecent_0x02CD_Packet = DateTime.Now;
                        }
                        //Log($"message from server: {_ffg.Type:X4}");
                    }
                }
            }
            while (_ffg != null);

            if (loggedIn && TimeSinceLoginStarted != null && TimeSinceLoginStarted.Elapsed.TotalSeconds > 13)
            {
                if ((TimeSinceWatchDogBark?.Elapsed.TotalSeconds ?? int.MaxValue) > 5)
                {
                    // at char select, or server choke, then "LoginNotComplete"
                    // char logged in, then "Adventurer"
                    if (Core.CharacterFilter.ClassTemplate == "Adventurer")
                    {
                        if (WarehouseFilterGlobals.MostRecent_0x02CD_Packet + TimeSpan.FromSeconds(15) > DateTime.Now)
                        {
                            TimeSinceWatchDogBark = Stopwatch.StartNew();
                            AppMessage resp = Client.Send(AppMessage.New($"Warehouse for {Core.CharacterFilter.AccountName}", 90, true));
                            if (!WatchDogRegistered && resp != null)
                            {
                                WatchDogRegistered = true;
                                Log($"Registered with watchdog, pool occupancy: {resp.PoolSize + 1}");
                            }
                        }
                    }
                }
                if (FirstInner)
                {
                    Log($"CAUTION: Warehouse {VersionString} is running");
                    Log($"Anyone near you can take all your non-attuned non-equipped items.");
                    Log($"Commands: whallowretrieve, whplayerdetectionjump, whallowjumpcmd");
                    FirstInner = false;
                }
                if (PendingRetrieveCommand != null && CharToLogin != null && Core.CharacterFilter.Id == CharToLogin.Value && PendingTradeGiveItems.Count > 0)
                {
                    TradeGiveItem(PendingRetrieveCommand.ChatterId, PendingRetrieveCommand.ChatterName, PendingTradeGiveItems.ToArray());
                    PendingTradeGiveItems.Clear();
                    PendingRetrieveCommand = null;
                    CharToLogin = null;
                }
                else if (CharToLogin != null)
                {
                    CharToLogin = null;
                }

                if (PlayerDetectionJump)
                {
                    #region nearby player check
                    try
                    {
                        WorldObjectCollection woc = Core.WorldFilter.GetByObjectClass(ObjectClass.Player);
                        int playerCount = woc.Quantity;
                        if (prevPlayerCount != playerCount)
                        {
                            prevPlayerCount = playerCount;
                            WantToJump = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogging.LogError(ErrorLogFile, ex);
                    }
                    #endregion
                }

                if (!ItemScanInProgress && (TimeSinceScanFinished == null || TimeSinceScanFinished.Elapsed > TimeSpan.FromMinutes(5)))
                {
                    WantToSyncItems = true;
                }

                if (!CramTimer.Enabled && !StackTimer.Enabled)
                {
                    Cram();
                }
                if (!StackTimer.Enabled)
                {
                    Stack();
                }
                if (WantToJump)
                {
                    Core.Foreground();
                    SendKey("o", true, true);
                    WantToJump = false;
                }
                SyncItems();
            }
            else if (!loggedIn)
            {
                DoLoginStuff();
                return;
            }
        }
        private void SyncItems()
        {
            if (!WantToSyncItems)
            {
                return;
            }
            DBSyncReport rep = new DBSyncReport();
            ItemScanInProgress = true;
            WantToSyncItems = false;
            List<Item> items = Core.GetAllMyItems();
            items.ForEach(k =>
            {
                k.CharId = Core.CharacterFilter.Id;
                k.CharName = Core.CharacterFilter.Name;
            });
            foreach (Item item in items)
            {
                rep.Increment(DBInsertOrUpdateItem(item));
            }
            List<Item> dbItems = DBSelectItemsByCharId(Core.CharacterFilter.Id);
            foreach (Item dbItem in dbItems)
            {
                if (!items.Any(k => k.ItemId == dbItem.ItemId))
                {
                    DBDeleteItem(dbItem.DbId);
                    rep.Increment(DBSyncAction.Delete);
                }
            }
            if (rep.Delete + rep.Insert + rep.Update > 0)
            {
                Log(rep.ToString());
            }
            TimeSinceScanFinished = Stopwatch.StartNew();
        }
        private void Cram()
        {
            List<Pack> sidePacks = Core.GetSidePacks();
            if (sidePacks.Sum(k => k.FreeSlots) > 0)
            {
                List<Item> mainPackItems = Core.GetMainPackContentsForCram();
                int mainPackItemsRemaining = mainPackItems.Count;
                if (mainPackItemsRemaining > 0)
                {
                    foreach (Pack packWithSpace in sidePacks.Where(k => k.FreeSlots > 0))
                    {
                        for (int i = 0; i < packWithSpace.FreeSlots && mainPackItemsRemaining > 0; i++)
                        {
                            Host.Actions.SelectItem(mainPackItems[mainPackItemsRemaining - 1].ItemId);
                            Host.Actions.MoveItem(mainPackItems[mainPackItemsRemaining - 1].ItemId, packWithSpace.Id, 0, true);
                            mainPackItemsRemaining--;
                            CramTimer.Enabled = true;
                            return;
                        }
                    }
                }
            }
            CramTimer.Enabled = false;
        }
        private void CramTimer_Tick(object sender, EventArgs e)
        {
            Cram();
        }
        private void Stack()
        {
            if (StackItem1 != null && StackItem2 != null)
            {
                Core.Actions.SelectItem(StackItem2.Id);
                if (StackItem1.StackCount() + StackItem2.StackCount() > StackItem1.StackMax())
                {
                    Core.Actions.SelectedStackCount = StackItem1.StackMax().Value - StackItem1.StackCount().Value;
                }
                Core.Actions.MoveItem(StackItem2.Id, Core.CharacterFilter.Id, 0, true);
                StackTimer.Enabled = false;
                StackItem1 = null;
                StackItem2 = null;
                WantToSyncItems = true;
                return;
            }
            else
            {
                List<WorldObject> allItems = Core.GetAllStackableItems();
                IEnumerable<WorldObject> nonFull = allItems.Where(k => k.StackCount() != k.StackMax());
                IEnumerable<IGrouping<string, WorldObject>> nonFullSiblingGroups = nonFull.GroupBy(k => k.Name).Where(k => k.Count() > 1);
                foreach (IGrouping<string, WorldObject> nonFullSiblingGroup in nonFullSiblingGroups)
                {
                    StackItem1 = nonFullSiblingGroup.First();
                    StackItem2 = nonFullSiblingGroup.Skip(1).First();
                    Core.Actions.SelectItem(StackItem1.Id);
                    Core.Actions.MoveItem(StackItem1.Id, Core.CharacterFilter.Id, 0, false);
                    StackTimer.Enabled = true;
                    return;
                }
                StackTimer.Enabled = false;
            }
        }
        private void StackTimer_Tick(object sender, EventArgs e)
        {
            Stack();
        }
        private void AddPendingItemsToTrade()
        {
            foreach (WorldObject wo in PendingItemsToTradeAdd
                .GroupBy(item => item.Name.EndsWith("Letter"))
                .SelectMany(d => d.GroupBy(item => item.Name.EndsWith("Heart"))
                .SelectMany(f => f.GroupBy(item => item.Name.EndsWith("Wing"))
                .SelectMany(g => g.GroupBy(item => item.Name.EndsWith("Seed"))
                .SelectMany(h => h.GroupBy(item => item.Name.EndsWith("Gem"))
                .SelectMany(j => j.GroupBy(item => item.Name.EndsWith("Jewel"))
                .SelectMany(m => m.GroupBy(item => item.Name.EndsWith("Key"))
                .SelectMany(n => n.GroupBy(item => item.Name.EndsWith("Plant"))
                .SelectMany(q => q.GroupBy(item => item.Name)
                .OrderByDescending(k => k.First().Name)
                .SelectMany(k => k.OrderByDescending(r => r.RealName())))))))))))
            {
                PendingItemsToTradeAdd2.Enqueue(wo);
                ItemsTradeAddTimer.Enabled = true;
            }
            PendingItemsToTradeAdd.Clear();
        }
        private void ItemsTradeAddTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PendingItemsToTradeAdd2.Count < 1)
                {
                    ItemsTradeAddTimer.Enabled = false;
                    return;
                }
                int id = PendingItemsToTradeAdd2.Dequeue().Id;
                Host.Actions.TradeAdd(id);
            }
            catch (Exception)
            {
                //in the middle of logging out?
                ItemsTradeAddTimer.Enabled = false;
            }
        }
    }
}