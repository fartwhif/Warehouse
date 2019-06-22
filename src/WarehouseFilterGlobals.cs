using System.Collections.Generic;

namespace Warehouse
{
    public sealed class WarehouseFilterGlobals
    {
        private WarehouseFilterGlobals() { }
        public static WarehouseFilterGlobals Instance => Nested.instance;
        public static int CharacterSlots { get => Nested.instance.characterSlots; set => Nested.instance.characterSlots = value; }
        public static List<Character> Characters { get => Nested.instance.characters; set => Nested.instance.characters = value; }
        private int characterSlots;
        private List<Character> characters = new List<Character>();
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
