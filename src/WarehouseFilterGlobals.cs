using System;
using System.Collections.Generic;

namespace Warehouse
{
    public class paket
    {
        public DateTime when { get; set; }
        public int Type { get; set; }
    }
    public sealed class WarehouseFilterGlobals
    {
        private WarehouseFilterGlobals() { }
        public static WarehouseFilterGlobals Instance => Nested.instance;
        public static int CharacterSlots { get => Nested.instance.characterSlots; set => Nested.instance.characterSlots = value; }
        public static List<Character> Characters { get => Nested.instance.characters; set => Nested.instance.characters = value; }
        public static bool PluginCoreStarted { get => Nested.instance.pluginCoreStarted; set => Nested.instance.pluginCoreStarted = value; }

        public static DateTime MostRecent_0x02CD_Packet { get => Nested.instance._MostRecent_0x02CD_Packet; set => Nested.instance._MostRecent_0x02CD_Packet = value; }

        public static Queue<paket> messages
        {
            get
            {
                return Nested.instance._messages;
            }
        }
        private int characterSlots;
        private bool pluginCoreStarted = false;
        private DateTime _MostRecent_0x02CD_Packet = DateTime.MinValue;
        private List<Character> characters = new List<Character>();
        private Queue<paket> _messages = new Queue<paket>();

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
            internal static readonly WarehouseFilterGlobals instance = new WarehouseFilterGlobals();
        }
    }
}
