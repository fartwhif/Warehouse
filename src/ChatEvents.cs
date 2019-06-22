using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Warehouse
{
    public partial class PluginCore
    {
        private int MessageColor = 5;
        private void InitChatEvents()
        {
            Core.ChatBoxMessage += new EventHandler<ChatTextInterceptEventArgs>(InboundChat);
            Core.CommandLineText += new EventHandler<ChatParserInterceptEventArgs>(OutboundChat);
        }
        private void OutboundChat(object sender, ChatParserInterceptEventArgs e)
        {
        }
        private void InboundChat(object sender, ChatTextInterceptEventArgs e)
        {
            Match match = Regex.Match(e.Text, "^(\\[.+\\] |)\\<Tell\\:IIDString\\:([0-9]+)\\:([^\\>]*)\\>[^\\<]*\\<\\\\Tell\\> (.+), \\\"([^\\n]*)\\\"\\n$");
            if (match.Success)
            {
                ChatMessage chatMessage = new ChatMessage()
                {
                    Channel = match.Groups[1].Value.Trim(),
                    ChatterId = int.Parse(match.Groups[2].Value.Trim()),
                    ChatterName = match.Groups[3].Value.Trim(),
                    Verb = match.Groups[4].Value.Trim(),
                    Message = match.Groups[5].Value.Trim()
                };
                HandleChatMessage(chatMessage);
            }
        }
        private void HandleChatMessage(ChatMessage chatMessage)
        {
            if (chatMessage.Verb == "tells you" && TradePartnerId == chatMessage.ChatterId)
            {
                HandleTradePartnerCommandText(chatMessage);
                return;
            }
            else if (string.IsNullOrEmpty(chatMessage.Channel) && chatMessage.Verb == "says")
            {
                HandleProximityChatMessage(chatMessage);
            }
        }
        private void HandleProximityChatMessage(ChatMessage chatMessage)
        {
            if (chatMessage.Message.ToLower() == "jump")
            {
                Jump();
            }
            else if (chatMessage.Message.ToLower() == "tog")
            {
                //PauseBreakToggle();
            }
            else if (chatMessage.Message.ToLower().StartsWith("search"))
            {
                Search(chatMessage);
            }
            else if (chatMessage.Message.ToLower().StartsWith("retrieve"))
            {
                Retrieve(chatMessage);
            }
            else if (chatMessage.Message.ToLower().StartsWith("switch"))
            {
                SwitchCharacter(chatMessage);
            }
        }
        private void Search(ChatMessage chatMessage)
        {
            string parameters = chatMessage.Message.Trim().Substring(6).Trim();
            List<Item> items = SearchFor(parameters, chatMessage.ChatterName);
            Tell(items, chatMessage.ChatterName);
        }
        public void Tell(List<Item> items, string who)
        {
            List<string> lines = new List<string>();
            if (items.Count < 1)
            {
                lines.Add($"/t {who}, no items found");
            }
            else
            {
                lines.Add($"/t {who}, {items.Count} items:");
                foreach (Item item in items)
                {
                    lines.Add($"/t {who}, {item.CharName}, {item.ItemName}");
                }
            }
            SendChatCommand(lines.ToArray());
        }
        private List<Item> SearchFor(string parameters, string chatterName)
        {
            Character searchCharacter = null;
            DBSearchItemsBy searchBy = DBSearchItemsBy.Substring;
            if (parameters.Contains(","))
            {
                string searchForChar = parameters.Substring(0, parameters.IndexOf(",")).Trim();
                if (string.IsNullOrEmpty(searchForChar))
                {
                    SendChatCommand($"/t {chatterName}, incorrect char specific retrieval, use: /t {Core.CharacterFilter.Name}, retrieve charname,itemname");
                    return null;
                }
                searchCharacter = DBSearchChars(db, searchForChar);
                if (searchCharacter == null)
                {
                    SendChatCommand($"/t {chatterName}, char specific retrieval failed: did not find character: {searchForChar}");
                    return null;
                }
                parameters = parameters.Substring(parameters.IndexOf(",") + 1).Trim();
                searchBy = DBSearchItemsBy.SubstringCharId;
            }
            return DBSearchItems(db, parameters, (searchCharacter != null) ? searchCharacter.Id : 0, searchBy)
                .Where(k => WarehouseFilterGlobals.Characters.Any(g => g.Id == k.CharId))
                .OrderBy(k => k.ItemName).ToList();
        }
        private void Retrieve(ChatMessage chatMessage)
        {
            string parameters = chatMessage.Message.Trim().Substring(8).Trim();
            List<Item> items = SearchFor(parameters, chatMessage.ChatterName);
            Tell(items, chatMessage.ChatterName);
            if (items.Count < 1)
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, retrieval failed, could not find item(s), use search command to find items to retrieve.");
            }
            List<IGrouping<int, Item>> uniqueChars2 = items.GroupBy(k => k.CharId).ToList();
            if (uniqueChars2.Count > 1)
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, retrieval failed, items found on multiple characters, use: /t {Core.CharacterFilter.Name}, retrieve charname, itemname");
                return;
            }
            else if (items.First().CharId == Core.CharacterFilter.Id)
            {
                TradeGiveItem(chatMessage.ChatterId, chatMessage.ChatterName, items.Select(k => k.ItemId).ToArray());
            }
            else
            {
                Item firstItem = items.First();
                if (TradePartnerId != 0 && TradePartnerId != chatMessage.ChatterId)
                {
                    SendChatCommand($"/t {chatMessage.ChatterName}, Sorry, I can't switch to {firstItem.CharName} to retrieve {PendingTradeGiveItems.Count} item(s) right now because I'm in the middle of trading.");
                    SendChatCommand($"Sorry, I can't switch to {firstItem.CharName} to retrieve {PendingTradeGiveItems.Count} item(s) right now because I'm in the middle of trading.");
                    return;
                }

                if (!WarehouseFilterGlobals.Characters.Any(k => k.Id != firstItem.CharId))
                {
                    SendChatCommand($"/t {chatMessage.ChatterName}, Sorry, I can't switch to {firstItem.CharName} to retrieve {PendingTradeGiveItems.Count} item(s) right now because that character is not in this account.");
                    SendChatCommand($"Sorry, I can't switch to {firstItem.CharName} to retrieve {PendingTradeGiveItems.Count} item(s) right now because that character is not in this account.");
                    return;
                }

                PendingTradeGiveItems.AddRange(items.Select(k => k.ItemId));
                PendingRetrieveCommand = chatMessage;
                SendChatCommand($"/t {chatMessage.ChatterName}, switching to {firstItem.CharName} to retrieve {PendingTradeGiveItems.Count} item(s)");
                SendChatCommand($"switching to {firstItem.CharId} to retrieve {PendingTradeGiveItems.Count} item(s)");
                CharToLogin = firstItem.CharId;
                Core.Actions.Logout();
            }
        }
        private void TradeGiveItem(int recipientId, string recipientName, int[] itemIds)
        {
            WorldObject player = Core.GetPlayerById(recipientId);
            if (player == null)
            {
                SendChatCommand($"/t {recipientName}, You are too far away to open a trade window. Please move closer and try the retrieve command again.");
                return;
            }
            double dist = GetMyDistanceTo(player.Coordinates());
            if (dist > 0.00454)
            {
                SendChatCommand($"/t {recipientName}, You are too far away to open a trade window. Please move closer and try the retrieve command again.");
                return;
            }
            Host.Actions.SelectItem(recipientId);
            GivingItems = itemIds;
            Core.Foreground();
            Mapper.KeyInfo ki = Mapper.GetScanCode("r");
            Input.SendKeyInput(ki.ScanCode, true, false);
            Input.SendKeyInput(ki.ScanCode, false, true);
        }
        private void HandleTradePartnerCommandText(ChatMessage chatMessage)
        {
            string command = "";
            string predicate = "";
            Match match = Regex.Match(chatMessage.Message, "^(switch|search|retrieve|help|add|xadd|show|clear)(.*)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                SayHelp();
                return;
            }
            command = match.Groups[1].Value;
            predicate = match.Groups[2].Value;
            switch (command.ToLower())
            {
                case "search":
                    Search(chatMessage);
                    break;
                case "retrieve":
                    Retrieve(chatMessage);
                    break;
                case "switch":
                    SwitchCharacter(chatMessage);
                    break;
                default:
                    HandleTradeCommand(command.Trim(), predicate.Trim(), chatMessage);
                    break;
            }

        }
        private void SwitchCharacter(ChatMessage chatMessage)
        {
            string switchToCharName = chatMessage.Message.Substring(6).Trim();
            Character switchTargetFound = WarehouseFilterGlobals.Characters.FirstOrDefault(k => k.Name.ToLower() == switchToCharName.ToLower());
            if (switchTargetFound == null)
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, Sorry, I can't switch to {switchToCharName} right now because that character is not in this account.");
                SendChatCommand($"Sorry, I can't switch to {switchToCharName} right now because that character is not in this account.");
                return;
            }

            if (TradePartnerId != 0 && TradePartnerId != chatMessage.ChatterId)
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, Sorry, I can't switch to {switchTargetFound.Name} right now because I'm in the middle of trading.");
                SendChatCommand($"Sorry, I can't switch to {switchTargetFound.Name} right now because I'm in the middle of trading.");
                return;
            }

            if (Core.CharacterFilter.Id == switchTargetFound.Id)
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, Switch failed: I'm already logged in!");
                SendChatCommand($"Switch failed: I'm already logged in!");
                return;
            }

            SendChatCommand($"/t {chatMessage.ChatterName}, switching to {switchTargetFound.Name}.");
            SendChatCommand($"switching to {switchTargetFound.Name}");

            CharToLogin = switchTargetFound.Id;
            Core.Actions.Logout();
        }
        private void HandleTradeCommand(string command, string predicate, ChatMessage chatMessage)
        {
            bool show = false;
            bool regex = false;
            bool number = false;
            int numberParsed = -1;
            if (command.ToLower() == "show")
            {
                command = "add";
                show = true;
            }
            else if (command.ToLower() == "xadd")
            {
                command = "add";
                regex = true;
            }
            if (!show)
            {
                Match match = Regex.Match(predicate, "^-?\\d+$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    if (int.TryParse(predicate, out numberParsed))
                    {
                        number = true;
                    }
                }
            }
            switch (command.ToLower())
            {
                case "add":
                    WorldObjectCollection woc = Core.WorldFilter.GetByOwner(Core.CharacterFilter.Id);
                    foreach (WorldObject wo in woc)
                    {
                        string itemName = wo.RealName();
                        if (show)
                        {
                            if (wo.ObjectClass != ObjectClass.Container)
                            {
                                PendingItemsToTradeAdd.Add(wo);
                            }
                        }
                        else if (number && wo.Exists(LongValueKey.Value))
                        {
                            int value = wo.Values(LongValueKey.Value);
                            if (numberParsed == value)
                            {
                                PendingItemsToTradeAdd.Add(wo);
                            }
                        }
                        else if (number && wo.Exists(LongValueKey.TotalValue))
                        {
                            int value = wo.Values(LongValueKey.TotalValue);
                            if (numberParsed == value)
                            {
                                PendingItemsToTradeAdd.Add(wo);
                            }
                        }
                        else if (regex)
                        {
                            Match match = Regex.Match(itemName, predicate, RegexOptions.IgnoreCase);
                            if (match.Success)
                            {
                                if (wo.ObjectClass != ObjectClass.Container)
                                {
                                    PendingItemsToTradeAdd.Add(wo);
                                }
                            }
                        }
                        else if (itemName.ToLower().Contains(predicate.ToLower()))
                        {
                            if (wo.ObjectClass != ObjectClass.Container)
                            {
                                PendingItemsToTradeAdd.Add(wo);
                            }
                        }
                    }
                    if (PendingItemsToTradeAdd.Count < 1)
                    {
                        SendChatCommand($"/t {chatMessage.ChatterName}, no items found");
                    }
                    AddPendingItemsToTrade();
                    break;
                case "clear":
                    Host.Actions.TradeReset();
                    break;
                case "help":
                    SayHelp(false);
                    break;

            }
        }
        private void SayHelp(bool meta = true, bool bannerOnly = false)
        {
            string ver = "";
            if (!meta)
            {
                Version ver2 = Assembly.GetAssembly(typeof(PluginCore)).GetName().Version;
                ver = $"{ver2.Major}.{ver2.Minor}";
            }
            if (bannerOnly)
            {
                SendChatCommand(new string[] { !meta ? $"/t {TradePartnerName}, Hello. Commands: [ help, add, xadd, show, clear, search, retrieve, switch ] warehouse v{ver}" : "" });
                SendChatCommand(new string[] { !meta ? $"/t {TradePartnerName}, {Core.GetBurdenStatus().ToString()}" : "" });

                return;
            }

            string[] help = new string[] {
                !meta ? $"/r Hello. I can mule for you.  Open up a trade window!  warehouse v{ver}" : "",
                meta ? $"/r sorry, I don't understand that command." : "",
                $"/r Commands: [ help, add, xadd, show, clear, search, retrieve, switch ]",
                $"/r search for diamond powders across all characters: /t {Core.CharacterFilter.Name}, search diamond powder",
                $"/r retrieve item(s) either on this character or on another.  Switches characters if needed.",
                $"/r from a specific character: /t {Core.CharacterFilter.Name}, retrieve MuleMan I, quiddity ingot",
                $"/r switch to another character in the account: /t {Core.CharacterFilter.Name}, switch MuleMan II",
                $"/r quills: /t {Core.CharacterFilter.Name}, add quill",
                $"/r all types of salvage bags: /t {Core.CharacterFilter.Name}, add salvage",
                $"/r all types of complete salvage bags: /t {Core.CharacterFilter.Name}, add salvage (100)",
                $"/r complete iron salvage bags: /t {Core.CharacterFilter.Name}, retrieve iron salvage (100)",
                $"/r partial salvage bags: /t {Core.CharacterFilter.Name}, xadd salvage \\([0-9]{{1,2}}\\)"};
            SendChatCommand(help);
        }
        private void DestroyChatEvents()
        {
            Core.ChatBoxMessage -= new EventHandler<ChatTextInterceptEventArgs>(InboundChat);
            Core.CommandLineText -= new EventHandler<ChatParserInterceptEventArgs>(OutboundChat);
        }
        private void WriteToChat(string message)
        {
            try
            {
                Host.Actions.AddChatText(message, MessageColor);
            }
            catch (Exception ex)
            {
                errorLogging.LogError(ErrorLogFile, ex);
            }
        }
        private void Log(string what)
        {
            WriteToChat("[warehouse] " + what);
        }
    }
}