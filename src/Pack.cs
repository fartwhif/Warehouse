using Decal.Adapter.Wrappers;

namespace Warehouse
{
    public class Pack
    {
        public int Id { get; set; }
        public int FreeSlots => TotalSlots - OccupiedSlots;
        public int TotalSlots { get; set; }
        public int OccupiedSlots { get; set; }
        public WorldObject WorldObject { get; set; }
    }
}
