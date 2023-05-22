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
            string cmd = e.Text;
            if (cmd.StartsWith("@") || cmd.StartsWith("/"))
            {
                cmd = cmd.Substring(1);
                if (cmd.ToLower().StartsWith("whallowretrieve"))
                {
                    e.Eat = true;
                    cmd = cmd.Substring(15);
                    bool? allow = ParseBool(cmd);
                    if (!allow.HasValue)
                    {
                        Log("warehouse setting value unknown, example:  @whallowretrieve yes");
                        return;
                    }
                    AllowRetrieve = allow.Value;
                    Log(DBGetSetting("allowretrieve").ToString());
                }
                else if (cmd.ToLower().StartsWith("whplayerdetectionjump"))
                {
                    e.Eat = true;
                    cmd = cmd.Substring(21);
                    bool? jump = ParseBool(cmd);
                    if (!jump.HasValue)
                    {
                        Log("warehouse setting value unknown, example:  @whplayerdetectionjump yes");
                        return;
                    }
                    PlayerDetectionJump = jump.Value;
                    Log(DBGetSetting("playerdetectionjump").ToString());
                }
                else if (cmd.ToLower().StartsWith("whallowjumpcmd"))
                {
                    e.Eat = true;
                    cmd = cmd.Substring(14);
                    bool? jump = ParseBool(cmd);
                    if (!jump.HasValue)
                    {
                        Log("warehouse setting value unknown, example:  @whallowjumpcmd yes");
                        return;
                    }
                    AllowJumpCommand = jump.Value;
                    Log(DBGetSetting("allowjumpcmd").ToString());
                }
            }
        }
        private static bool? ParseBool(string inp)
        {
            inp = inp.ToLower().Trim();
            bool? val = null;
            if (inp == "y")
            {
                val = true;
            }
            else if (inp == "n")
            {
                val = false;
            }
            else if (inp == "1")
            {
                val = true;
            }
            else if (inp == "0")
            {
                val = false;
            }
            else if (inp == "yes")
            {
                val = true;
            }
            else if (inp == "no")
            {
                val = false;
            }
            else if (inp == "true")
            {
                val = true;
            }
            else if (inp == "false")
            {
                val = false;
            }
            return val;
        }
        private void InboundChat(object sender, ChatTextInterceptEventArgs e)
        {
            Match match = Regex.Match(e.Text, @"^(\[.+\] |)\<Tell\:IIDString\:([0-9]+)\:([^\>]*)\>[^\<]*\<\\Tell\> ([^,]+), \""([^\n]*)\""\n*$");
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
                match = Regex.Match(chatMessage.Message, "^(switch|search|retrieve|tag|tags|help|add|xadd|show|clear)(.*)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    chatMessage.ParseSuccess = true;
                    chatMessage.ParsedCommand = match.Groups[1].Value.Trim();
                    chatMessage.ParsedParameters = match.Groups[2].Value.Trim();
                }
                HandleChatMessage(chatMessage);
            }
        }
        private void HandleChatMessage(ChatMessage chatMessage)
        {
            //"[Trade]"
            //"[Allegiance]"
            //Channel = "[General]"
            if (!chatMessage.IsTell || chatMessage.Channel != "")
            {
                return;//only handle tells
            }
            if (chatMessage.Message.ToLower().StartsWith("help"))
            {
                SayHelp(chatMessage.ChatterName, false, false);
            }
            else if (!chatMessage.ParseSuccess && chatMessage.IsTell)
            {
                SayHelp(chatMessage.ChatterName, true, false);
            }
            else if (chatMessage.Message.ToLower() == "jump")
            {
                if (AllowJumpCommand)
                {
                    WantToJump = true;
                }
                else
                {
                    SendChatCommand($"/t {chatMessage.ChatterName}, Sorry, jump command is disabled.");
                }
            }
            else if (chatMessage.Message.ToLower().StartsWith("tags"))
            {
                Tags(chatMessage);
            }
            else if (chatMessage.Message.ToLower().StartsWith("tag"))
            {
                Tag(chatMessage);
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
            else if (chatMessage.IsEither(TradePartnerId, TradePartnerName))
            {
                HandleTradeCommand(chatMessage);
                return;
            }
        }
        private void Tags(ChatMessage chatMessage)
        {
            List<CharacterTag> tags = DBGetAllTags();
            if (tags.Any())
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, {tags.Count} tagged characters:");
                foreach (CharacterTag tag in tags)
                {
                    SendChatCommand($"/t {chatMessage.ChatterName}, {DBSearchChars(tag.CharacterId)?.Name ?? tag.CharacterId.ToString()} ( {tag.Tag} )");
                }
            }
            else
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, No tagged characters yet!");
                SendChatCommand($"/t {chatMessage.ChatterName}, To tag me 'misc', open a trade with me and type /r tag misc");
                SendChatCommand($"/t {chatMessage.ChatterName}, Then, when using search, retrieve, and switch commands reference me with my tag, misc");
                SendChatCommand($"/t {chatMessage.ChatterName}, Don't give the same tag to multiple mules, or you will have a bad time.");
            }
        }
        private void Tag(ChatMessage chatMessage)
        {
            string parameters = chatMessage.Message.Trim().Substring(3).Trim();
            CharacterTag existing = DBGetTag(Core.CharacterFilter.Id);
            DBSyncAction actionTaken = DBSetTag(new CharacterTag() { CharacterId = Core.CharacterFilter.Id, Tag = parameters });
            switch (actionTaken)
            {
                case DBSyncAction.Insert:
                    SendChatCommand($"/t {chatMessage.ChatterName}, my tag is now: ( {parameters} )");
                    break;
                case DBSyncAction.Update:
                    SendChatCommand($"/t {chatMessage.ChatterName}, changed my tag:");
                    SendChatCommand($"/t {chatMessage.ChatterName}, from ( {existing.Tag} ) to ( {parameters} )");
                    break;
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
            if (items == null)
            {
                return;
            }
            List<string> lines = new List<string>();
            List<CharacterTag> tags = DBGetAllTags();
            if (items.Count < 1)
            {
                lines.Add($"/t {who}, no items found");
            }
            else
            {
                lines.Add($"/t {who}, {items.Count} items:");
                foreach (IGrouping<int, Item> charGrp in items.GroupBy(k => k.CharId))
                {
                    foreach (IGrouping<string, Item> itemGrp in charGrp.GroupBy(k => k.ItemName))
                    {
                        Item item = itemGrp.First();
                        int cnt = itemGrp.Count();
                        bool multi = cnt > 1;
                        string strTally = multi ? $" x{cnt}" : "";

                        CharacterTag tag = tags.FirstOrDefault(k => k.CharacterId == item.CharId);
                        string tagTxt = (tag == null) ? "" : $" ( {tag.Tag} )";
                        lines.Add($"/t {who}, {item.CharName}{tagTxt}, {item.ItemName}{strTally}");
                    }
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
                string searchForCharOrTag = parameters.Substring(0, parameters.IndexOf(",")).Trim();
                if (string.IsNullOrEmpty(searchForCharOrTag))
                {
                    SendChatCommand($"/t {chatterName}, incorrect char specific retrieval, use: /t {Core.CharacterFilter.Name}, retrieve <char name or tag>,itemname");
                    SendChatCommand($"/t {chatterName}, example: /t {Core.CharacterFilter.Name}, retrieve misc, argenory");
                    return null;
                }
                searchCharacter = DBSearchChars(searchForCharOrTag);
                if (searchCharacter == null)
                {
                    CharacterTag tag = DBGetTag(searchForCharOrTag);
                    if (tag != null)
                    {
                        searchCharacter = DBSearchChars(tag.CharacterId);
                    }
                }
                if (searchCharacter == null)
                {
                    SendChatCommand($"/t {chatterName}, char specific retrieval failed: did not find character by name or tag: {searchForCharOrTag}");
                    return null;
                }
                parameters = parameters.Substring(parameters.IndexOf(",") + 1).Trim();
                searchBy = DBSearchItemsBy.SubstringCharId;
            }
            return DBSearchItems(parameters, (searchCharacter != null) ? searchCharacter.Id : 0, searchBy)
                .Where(k => WarehouseFilterGlobals.Characters.Any(g => g.Id == k.CharId))
                .OrderBy(k => k.ItemName).ToList();
        }
        private void Retrieve(ChatMessage chatMessage)
        {
            if (!AllowRetrieve)
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, Sorry, retrieval command is disabled.");
                return;
            }
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
                if (TradePartnerId != 0 && !chatMessage.IsEither(TradePartnerId, TradePartnerName))
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
                SendChatCommand($"switching to {firstItem.CharName} to retrieve {PendingTradeGiveItems.Count} item(s)");
                CharToLogin = firstItem.CharId;
                Core.Actions.Logout();
            }
        }
        private void TradeGiveItem(int recipientId, string recipientName, int[] itemIds)
        {
            WorldObject player = Core.GetPlayerByIdOrName(recipientId, recipientName);
            if (player == null)
            {
                SendChatCommand($"/t {recipientName}, You are too far away to open a trade window. Please move closer and try the retrieve command again.");
                return;
            }
            if (recipientId == 0)
            {
                recipientId = player.Id;
            }
            double dist = Core.WorldFilter.Distance(recipientId, Core.CharacterFilter.Id, true);
            //double dist = GetMyDistanceTo(player.Coordinates());//broken
            if (dist > .005)
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
        private void SwitchCharacter(ChatMessage chatMessage)
        {
            string switchToCharName = chatMessage.Message.Substring(6).Trim();
            Character switchTargetFound = WarehouseFilterGlobals.Characters.FirstOrDefault(k => k.Name.ToLower() == switchToCharName.ToLower());
            CharacterTag charTag = null;

            if (switchTargetFound == null)
            {
                charTag = DBGetTag(switchToCharName);
                if (charTag != null)
                {
                    switchTargetFound = WarehouseFilterGlobals.Characters.FirstOrDefault(k => k.Id == charTag.CharacterId);
                }
            }
            string charTagTxt = (charTag == null) ? "" : $" ( {charTag.Tag} )";
            if (switchTargetFound == null)
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, Sorry, I can't switch to {switchToCharName} right now because that character name or tag is not in this account.");
                SendChatCommand($"Sorry, I can't switch to {switchToCharName} right now because that character name or tag is not in this account.");
                return;
            }
            if (TradePartnerId != 0 && !chatMessage.IsEither(TradePartnerId, TradePartnerName))
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, Sorry, I can't switch to {switchTargetFound.Name}{charTagTxt} right now because I'm in the middle of trading.");
                SendChatCommand($"Sorry, I can't switch to {switchTargetFound.Name}{charTagTxt} right now because I'm in the middle of trading.");
                return;
            }

            if (Core.CharacterFilter.Id == switchTargetFound.Id)
            {
                SendChatCommand($"/t {chatMessage.ChatterName}, Switch failed: I'm already logged in!");
                SendChatCommand($"Switch failed: I'm already logged in!");
                return;
            }

            SendChatCommand($"/t {chatMessage.ChatterName}, switching to {switchTargetFound.Name}{charTagTxt}");
            SendChatCommand($"switching to {switchTargetFound.Name}{charTagTxt}");

            CharToLogin = switchTargetFound.Id;
            Core.Actions.Logout();
        }
        private void HandleTradeCommand(ChatMessage chatMessage)
        {
            bool show = false;
            bool regex = false;
            bool number = false;
            int numberParsed = -1;
            string command = chatMessage.ParsedCommand;
            if (chatMessage.ParsedCommand.ToLower() == "show")
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
                Match match = Regex.Match(chatMessage.ParsedParameters, "^-?\\d+$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    if (int.TryParse(chatMessage.ParsedParameters, out numberParsed))
                    {
                        number = true;
                    }
                }
            }
            switch (command.ToLower())
            {
                case "add":
                    WorldObjectCollection woc = Core.WorldFilter.GetByOwner(Core.CharacterFilter.Id);
                    var equippedItems = GetEquippedItems();
                    bool complaints = false;
                    foreach (WorldObject wo in woc)
                    {
                        string itemName = wo.RealName();
                        if (show)
                        {
                            if (wo.ObjectClass != ObjectClass.Container)
                            {
                                if (AddPendingItemToTrade(equippedItems, wo, chatMessage.ChatterName, show))
                                {
                                    complaints = true;
                                }
                            }
                        }
                        else if (number && wo.Exists(LongValueKey.Value))
                        {
                            int value = wo.Values(LongValueKey.Value);
                            if (numberParsed == value)
                            {
                                if (AddPendingItemToTrade(equippedItems, wo, chatMessage.ChatterName, show))
                                {
                                    complaints = true;
                                }
                            }
                        }
                        else if (number && wo.Exists(LongValueKey.TotalValue))
                        {
                            int value = wo.Values(LongValueKey.TotalValue);
                            if (numberParsed == value)
                            {
                                if (AddPendingItemToTrade(equippedItems, wo, chatMessage.ChatterName, show))
                                {
                                    complaints = true;
                                }
                            }
                        }
                        else if (regex)
                        {
                            Match match = Regex.Match(itemName, chatMessage.ParsedParameters, RegexOptions.IgnoreCase);
                            if (match.Success)
                            {
                                if (wo.ObjectClass != ObjectClass.Container)
                                {
                                    if (AddPendingItemToTrade(equippedItems, wo, chatMessage.ChatterName, show))
                                    {
                                        complaints = true;
                                    }
                                }
                            }
                        }
                        else if (itemName.ToLower().Contains(chatMessage.ParsedParameters.ToLower()))
                        {
                            if (wo.ObjectClass != ObjectClass.Container)
                            {
                                if (AddPendingItemToTrade(equippedItems, wo, chatMessage.ChatterName, show))
                                {
                                    complaints = true;
                                }
                            }
                        }
                    }
                    if (PendingItemsToTradeAdd.Count < 1)
                    {
                        if (!complaints)
                        {
                            SendChatCommand($"/t {chatMessage.ChatterName}, no items found");
                        }
                    }
                    AddPendingItemsToTrade();
                    break;
                case "clear":
                    PendingItemsToTradeAdd2.Clear();
                    PendingItemsToTradeAdd.Clear();
                    Host.Actions.TradeReset();
                    break;
            }
        }
        public List<WorldObject> GetEquippedItems()
        {
            var list = new List<WorldObject>();
            using (var inv = Core.WorldFilter.GetInventory())
            {
                foreach (var item in inv)
                {
                    if (item.Values(LongValueKey.Slot, -1) == -1)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private bool AddPendingItemToTrade(List<WorldObject> equippedItems, WorldObject wo, string chatterName = null, bool show = false)
        {
            bool complaint = false;
            var thisItem = equippedItems.FirstOrDefault(k => k.Id == wo.Id);
            if (thisItem == null)
            {
                PendingItemsToTradeAdd.Add(wo);
            }
            else if (!string.IsNullOrEmpty(chatterName))
            {
                if (!show)//if it's an "add" or "fetch"
                {
                    SendChatCommand($"/t {chatterName}, I'm not that kind of bot!");
                    complaint = true;
                }
            }
            return complaint;
        }
        private string VersionString
        {
            get
            {
                Version ver = Assembly.GetAssembly(typeof(PluginCore)).GetName().Version;
                return $"v{ver.Major}.{ver.Minor}.{ver.Build}";
            }
        }
        private void SayHelp(string toWho, bool SayWotM8 = true, bool bannerOnly = false)
        {
            string commands = $"Commands: (switch|search|retrieve|tag|tags|help|add|xadd|show|clear) warehouse {VersionString}";
            if (bannerOnly)
            {
                SendChatCommand(new string[] { !SayWotM8 ? $"/t {toWho}, Hello. {commands}" : "" });
                SendChatCommand(new string[] { !SayWotM8 ? $"/t {toWho}, {Core.GetBurdenStatus().ToString()}" : "" });
                return;
            }
            else
            {
                string[] help = new string[] {
                !SayWotM8 ? $"/t {toWho},  Hello. I can mule for you.  Open up a trade window!" : "",
                SayWotM8 ? $"/t {toWho},  Sorry, I don't understand that command." : "",
                $"/t {toWho}, {commands}",
                $"/t {toWho}, search for diamond powders across all characters: /t {Core.CharacterFilter.Name}, search diamond powder",
                $"/t {toWho}, retrieve item(s) either on this character or on another.  Switches characters if needed.",
                $"/t {toWho}, from a specific character: /t {Core.CharacterFilter.Name}, retrieve MuleMan I, quiddity ingot",
                $"/t {toWho}, switch to a character tagged weapons: /t {Core.CharacterFilter.Name}, switch weapons",
                $"/t {toWho}, get a list of all tagged characters and their tags: /t {Core.CharacterFilter.Name}, tags",
                $"/t {toWho}, tag me as salvage to make things easier: /t {Core.CharacterFilter.Name}, tag salvage",
                $"/t {toWho}, add all quill types to trade window: /t {Core.CharacterFilter.Name}, add quill",
                $"/t {toWho}, all types of salvage bags: /t {Core.CharacterFilter.Name}, add salvage",
                $"/t {toWho}, all types of complete salvage bags: /t {Core.CharacterFilter.Name}, add salvage (100)",
                $"/t {toWho}, complete iron salvage bags: /t {Core.CharacterFilter.Name}, retrieve iron salvage (100)",
                $"/t {toWho}, all partial salvage bags via regex: /t {Core.CharacterFilter.Name}, xadd salvage \\([0-9]{{1,2}}\\)"};
                SendChatCommand(help);
            }
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
            WriteToChat("[Warehouse] " + what);
        }
    }
}