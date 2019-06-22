using Decal.Adapter.Wrappers;
using System;
using System.Linq;

namespace Warehouse
{
    public partial class PluginCore
    {
        private void InitWorldFilter()
        {
            //////////////////////////////////////////////////////////////
            // To initialize any of the World Filter Events,            //
            // simply uncomment the appropriate initialization          //
            // statement(s) below to enable the event handler           //
            //////////////////////////////////////////////////////////////
            // Initialize the ResetTrade event handler
            Core.WorldFilter.ResetTrade += new EventHandler<ResetTradeEventArgs>(WorldFilter_ResetTrade);
            // Initialize the ReleaseObject event handler
            Core.WorldFilter.ReleaseObject += new EventHandler<ReleaseObjectEventArgs>(WorldFilter_ReleaseObject);
            // Initialize the ReleaseDone event handler
            Core.WorldFilter.ReleaseDone += new EventHandler(WorldFilter_ReleaseDone);
            // Initialize the MoveObject event handler
            Core.WorldFilter.MoveObject += new EventHandler<MoveObjectEventArgs>(WorldFilter_MoveObject);
            // Initialize the FailToCompleteTrade event handler
            Core.WorldFilter.FailToCompleteTrade += new EventHandler(WorldFilter_FailToCompleteTrade);
            // Initialize the FailToAddTradeItem event handler
            Core.WorldFilter.FailToAddTradeItem += new EventHandler<FailToAddTradeItemEventArgs>(WorldFilter_FailToAddTradeItem);
            // Initialize the EnterTrade event handler
            Core.WorldFilter.EnterTrade += new EventHandler<EnterTradeEventArgs>(WorldFilter_EnterTrade);
            // Initialize the EndTrade event handler
            Core.WorldFilter.EndTrade += new EventHandler<EndTradeEventArgs>(WorldFilter_EndTrade);
            // Initialize the DeclineTrade event handler
            Core.WorldFilter.DeclineTrade += new EventHandler<DeclineTradeEventArgs>(WorldFilter_DeclineTrade);
            // Initialize the CreateObject event handler
            Core.WorldFilter.CreateObject += new EventHandler<CreateObjectEventArgs>(WorldFilter_CreateObject);
            // Initialize the ChangeObject event handler
            Core.WorldFilter.ChangeObject += new EventHandler<ChangeObjectEventArgs>(WorldFilter_ChangeObject);
            // Initialize the ApproachVendor event handler
            Core.WorldFilter.ApproachVendor += new EventHandler<ApproachVendorEventArgs>(WorldFilter_ApproachVendor);
            // Initialize the AddTradeItem event handler
            Core.WorldFilter.AddTradeItem += new EventHandler<AddTradeItemEventArgs>(WorldFilter_AddTradeItem);
            // Initialize the AcceptTrade event handler
            Core.WorldFilter.AcceptTrade += new EventHandler<AcceptTradeEventArgs>(WorldFilter_AcceptTrade);
        }
        private void DestroyWorldFilter()
        {
            // UnInitialize the ResetTrade event handler
            Core.WorldFilter.ResetTrade -= new EventHandler<ResetTradeEventArgs>(WorldFilter_ResetTrade);
            // UnInitialize the ReleaseObject event handler
            Core.WorldFilter.ReleaseObject -= new EventHandler<ReleaseObjectEventArgs>(WorldFilter_ReleaseObject);
            // UnInitialize the ReleaseDone event handler
            Core.WorldFilter.ReleaseDone -= new EventHandler(WorldFilter_ReleaseDone);
            // UnInitialize the MoveObject event handler
            Core.WorldFilter.MoveObject -= new EventHandler<MoveObjectEventArgs>(WorldFilter_MoveObject);
            // UnInitialize the FailToCompleteTrade event handler
            Core.WorldFilter.FailToCompleteTrade -= new EventHandler(WorldFilter_FailToCompleteTrade);
            // UnInitialize the FailToAddTradeItem event handler
            Core.WorldFilter.FailToAddTradeItem -= new EventHandler<FailToAddTradeItemEventArgs>(WorldFilter_FailToAddTradeItem);
            // UnInitialize the EnterTrade event handler
            Core.WorldFilter.EnterTrade -= new EventHandler<EnterTradeEventArgs>(WorldFilter_EnterTrade);
            // UnInitialize the EndTrade event handler
            Core.WorldFilter.EndTrade -= new EventHandler<EndTradeEventArgs>(WorldFilter_EndTrade);
            // UnInitialize the DeclineTrade event handler
            Core.WorldFilter.DeclineTrade -= new EventHandler<DeclineTradeEventArgs>(WorldFilter_DeclineTrade);
            // UnInitialize the CreateObject event handler
            Core.WorldFilter.CreateObject -= new EventHandler<CreateObjectEventArgs>(WorldFilter_CreateObject);
            // UnInitialize the ChangeObject event handler
            Core.WorldFilter.ChangeObject -= new EventHandler<ChangeObjectEventArgs>(WorldFilter_ChangeObject);
            // UnInitialize the ApproachVendor event handler
            Core.WorldFilter.ApproachVendor -= new EventHandler<ApproachVendorEventArgs>(WorldFilter_ApproachVendor);
            // UnInitialize the AddTradeItem event handler
            Core.WorldFilter.AddTradeItem -= new EventHandler<AddTradeItemEventArgs>(WorldFilter_AddTradeItem);
            // UnInitialize the AcceptTrade event handler
            Core.WorldFilter.AcceptTrade -= new EventHandler<AcceptTradeEventArgs>(WorldFilter_AcceptTrade);
        }
        private void WorldFilter_EndTrade(object sender, EndTradeEventArgs e)
        {
            if (!string.IsNullOrEmpty(TradePartnerName))
            {
                SendChatCommand($"/t {TradePartnerName}, goodbye");
            }
            TradePartnerId = 0;
            TradePartnerName = "";
        }
        private void WorldFilter_AcceptTrade(object sender, AcceptTradeEventArgs e)
        {
            if (prevTradeTime.Elapsed > TimeSpan.FromSeconds(1))
            {
                Host.Actions.TradeAccept();
                prevTradeTime.Reset();
                prevTradeTime.Start();
                WantToSyncItems = true;
            }
        }
        private void WorldFilter_EnterTrade(object sender, EnterTradeEventArgs e)
        {
            WorldObjectCollection players = Core.WorldFilter.GetByObjectClass(ObjectClass.Player);
            int myId = Core.CharacterFilter.Id;
            foreach (WorldObject player in players.Where(k => k.Id != myId))
            {
                if (player.Id == e.TraderId || player.Id == e.TradeeId)
                {
                    TradePartnerName = player.Name;
                    TradePartnerId = player.Id;
                }
            }
            if (GivingItems != null)
            {
                WorldObjectCollection woc = Core.WorldFilter.GetByOwner(Core.CharacterFilter.Id);
                foreach (WorldObject wo in woc)
                {
                    if (wo.ObjectClass != ObjectClass.Container)
                    {
                        if (GivingItems.Contains(wo.Id))
                        {
                            PendingItemsToTradeAdd.Add(wo);
                        }
                    }
                }
                AddPendingItemsToTrade();
                GivingItems = null;
            }
            else
            {
                if (TradePartnerName != "")
                {
                    SayHelp(false, true);
                }
                WorldObjectCollection woc = Core.WorldFilter.GetByOwner(Core.CharacterFilter.Id);
                foreach (WorldObject wo in woc)
                {
                    if (wo.ObjectClass != ObjectClass.Container)
                    {
                        PendingItemsToTradeAdd.Add(wo);
                    }
                }
                AddPendingItemsToTrade();
            }
        }
        private void WorldFilter_AddTradeItem(object sender, AddTradeItemEventArgs e)
        {
            // DO STUFF HERE
        }
        private void WorldFilter_ApproachVendor(object sender, ApproachVendorEventArgs e)
        {
            // DO STUFF HERE
        }
        private void WorldFilter_ChangeObject(object sender, ChangeObjectEventArgs e)
        {
        }
        private void WorldFilter_CreateObject(object sender, CreateObjectEventArgs e)
        {
            if (e.New.Container == Core.CharacterFilter.Id)
            {
                WantToSyncItems = true;
            }
        }
        private void WorldFilter_DeclineTrade(object sender, DeclineTradeEventArgs e)
        {
            // DO STUFF HERE
        }
        private void WorldFilter_FailToAddTradeItem(object sender, FailToAddTradeItemEventArgs e)
        {
            // DO STUFF HERE
        }
        private void WorldFilter_FailToCompleteTrade(object sender, EventArgs e)
        {
            // DO STUFF HERE
        }
        private void WorldFilter_MoveObject(object sender, MoveObjectEventArgs e)
        {
            // DO STUFF HERE
        }
        private void WorldFilter_ReleaseDone(object sender, EventArgs e)
        {
            // DO STUFF HERE
        }
        private void WorldFilter_ReleaseObject(object sender, ReleaseObjectEventArgs e)
        {
            // DO STUFF HERE
        }
        private void WorldFilter_ResetTrade(object sender, ResetTradeEventArgs e)
        {
            // DO STUFF HERE
        }
    }
}