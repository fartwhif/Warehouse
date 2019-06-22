using Decal.Adapter;
using System;

namespace Warehouse
{
    [FriendlyName("WarehouseFilter")]
    public class WarehouseFilter : FilterBase
    {
        protected override void Shutdown()
        {
            ServerDispatch -= new EventHandler<NetworkMessageEventArgs>(FilterCore_ServerDispatch);
        }
        protected override void Startup()
        {
            ServerDispatch += new EventHandler<NetworkMessageEventArgs>(FilterCore_ServerDispatch);
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
            }
        }
    }
}
