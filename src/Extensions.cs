using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System.Collections.Generic;
using System.Linq;

namespace Warehouse
{
    internal static class Extensions
    {
        public static WorldObject GetPlayerById(this CoreManager cm, int id)
        {
            WorldObjectCollection players = cm.WorldFilter.GetByObjectClass(ObjectClass.Player);
            return players.FirstOrDefault(k => k.Id == id);
        }
        public static List<Item> GetAllMyItems(this CoreManager cm)
        {
            List<WorldObject> targetContainers = new List<WorldObject>();
            WorldObjectCollection myItems = cm.WorldFilter.GetByOwner(cm.CharacterFilter.Id);
            List<Item> items = new List<Item>();
            foreach (WorldObject item in myItems)
            {
                if (item.ObjectClass != ObjectClass.Container && item.ObjectClass != ObjectClass.Foci && !item.Attuned() && !item.IsWielded() && !item.IsEquipped())
                {
                    items.Add(new Item()
                    {
                        ItemId = item.Id,
                        ItemName = item.RealName()
                    });
                }
            }
            return items;
        }
        public static List<WorldObject> GetAllStackableItems(this CoreManager cm)
        {
            List<WorldObject> targetContainers = new List<WorldObject>();
            WorldObjectCollection myItems = cm.WorldFilter.GetByOwner(cm.CharacterFilter.Id);
            List<WorldObject> items = new List<WorldObject>();
            foreach (WorldObject item in myItems)
            {
                if (item.ObjectClass != ObjectClass.Container && item.ObjectClass != ObjectClass.Foci && !item.IsWielded() && !item.IsEquipped() && item.StackMax().HasValue)
                {
                    items.Add(item);
                }
            }
            return items;
        }
        public static List<Item> GetMainPackContentsForCram(this CoreManager cm)
        {
            List<WorldObject> targetContainers = new List<WorldObject>();
            WorldObjectCollection myItems = cm.WorldFilter.GetByContainer(cm.CharacterFilter.Id);
            List<Item> items = new List<Item>();
            foreach (WorldObject item in myItems)
            {
                if (item.ObjectClass != ObjectClass.Container && item.ObjectClass != ObjectClass.Foci)
                {
                    if (!item.IsEquipped() && !item.IsWielded())
                    {
                        items.Add(new Item() { ItemId = item.Id });
                    }
                }
            }
            return items;
        }
        public static List<Pack> GetSidePacks(this CoreManager cm)
        {
            List<WorldObject> targetContainers = new List<WorldObject>();
            WorldObjectCollection myItems = cm.WorldFilter.GetByOwner(cm.CharacterFilter.Id);
            foreach (WorldObject item in myItems)
            {
                if (item.ObjectClass == ObjectClass.Container)
                {
                    targetContainers.Add(item);
                }
            }
            List<Pack> sidePacks = new List<Pack>();
            foreach (WorldObject cont in targetContainers)
            {
                if (cont.Exists(LongValueKey.ItemSlots))
                {
                    sidePacks.Add(new Pack()
                    {
                        Id = cont.Id,
                        TotalSlots = cont.Values(LongValueKey.ItemSlots),
                        OccupiedSlots = cm.WorldFilter.GetByOwner(cont.Id).Count,
                        WorldObject = cont
                    });
                }
            }
            return sidePacks;
        }
        public static bool IsEquipped(this WorldObject wo)
        {
            int? es = LongValue(wo, LongValueKey.EquippedSlots);
            return es.HasValue && es.Value > 0;
        }
        public static bool IsWielded(this WorldObject wo)
        {
            int? es = LongValue(wo, LongValueKey.Wielder);
            return es.HasValue && es.Value > 0;
        }
        public static Dictionary<LongValueKey, int> GetLongValues(this WorldObject wo)
        {
            Dictionary<LongValueKey, int> longVals = new Dictionary<LongValueKey, int>();
            foreach (int key in wo.LongKeys)
            {
                longVals[(LongValueKey)key] = wo.Values((LongValueKey)key);
            }
            return longVals;
        }
        public static Dictionary<StringValueKey, string> GetStringValues(this WorldObject wo)
        {
            Dictionary<StringValueKey, string> stringVals = new Dictionary<StringValueKey, string>();
            foreach (int key in wo.StringKeys)
            {
                stringVals[(StringValueKey)key] = wo.Values((StringValueKey)key);
            }
            return stringVals;
        }
        public static Dictionary<DoubleValueKey, double> GetDoubleValues(this WorldObject wo)
        {
            Dictionary<DoubleValueKey, double> doubleVals = new Dictionary<DoubleValueKey, double>();
            foreach (int key in wo.DoubleKeys)
            {
                doubleVals[(DoubleValueKey)key] = wo.Values((DoubleValueKey)key);
            }
            return doubleVals;
        }
        public static string GetSalvageMaterialName(WorldObject wo)
        {
            if (wo.ObjectClass != ObjectClass.Salvage)
            {
                return null;
            }
            int k = -1;
            foreach (KeyValuePair<int, int> ms in SalvageLookup.MaterialSalvage)
            {
                if (ms.Value == wo.Type)
                {
                    k = ms.Key;
                }
            }
            if (k == -1)
            {
                return null;
            }
            return SalvageLookup.MaterialNames[k];
        }
        public static string RealName(this WorldObject wo)
        {
            string itemName = wo.Name;
            string salvMat = GetSalvageMaterialName(wo);
            if (salvMat != null)
            {
                itemName = $"{salvMat} {itemName}";
            }
            else
            {
                int? matId = LongValue(wo, LongValueKey.Material);
                if (matId.HasValue)
                {
                    string itemMat = SalvageLookup.MaterialNames[matId.Value];
                    itemName = $"{itemMat} {itemName}";
                }
            }
            return itemName;
        }
        public static int Value(this WorldObject wo)
        {
            return LongValue(wo, LongValueKey.Value).Value;
        }
        public static int? StackCount(this WorldObject wo)
        {
            return LongValue(wo, LongValueKey.StackCount);
        }
        public static int? StackMax(this WorldObject wo)
        {
            return LongValue(wo, LongValueKey.StackMax);
        }
        public static bool Attuned(this WorldObject wo)
        {
            return LongValue(wo, LongValueKey.Attuned) > 0;
        }
        public static int? LongValue(this WorldObject wo, LongValueKey key)
        {
            if (wo.Exists(key))
            {
                return wo.Values(key);
            }
            return null;
        }
        public static BurdenStatus GetBurdenStatus(this CoreManager cm)
        {
            BurdenStatus burden = new BurdenStatus();
            List<Item> mainPackItems = GetMainPackContentsForCram(cm);
            List<Pack> sidePacks = GetSidePacks(cm);
            burden.TotalSlots += sidePacks.Sum(k => k.TotalSlots) + 102;
            burden.OccupiedSlots += sidePacks.Sum(k => k.OccupiedSlots) + mainPackItems.Count;
            burden.CurrentBurdenPercentage = cm.CharacterFilter.Burden;
            burden.CurrentBurden = cm.CharacterFilter.BurdenUnits;
            return burden;
        }
        public static void Foreground(this CoreManager cm)
        {
            Input.SetForegroundWindow(cm.Decal.Hwnd);
            //IntPtr acWindow = Input.FindWindow("Turbine Device Class", null);
            //acWindow = Input.FindWindowByCaption(IntPtr.Zero, "Asheron's Call");
            //if (acWindow != IntPtr.Zero && acWindow != cm.Decal.Hwnd)
            //{
            //    Input.SetForegroundWindow(acWindow);
            //}
            //else
            //{
            //    Input.SetForegroundWindow(cm.Decal.Hwnd);
            //}
        }
    }
}
