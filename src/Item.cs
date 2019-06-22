namespace Warehouse
{
    public class Item
    {
        public int DbId { get; set; }
        public int CharId { get; set; }
        public string CharName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }

        public bool DBEquals(Item obj)
        {
            return DbId == obj.DbId 
                && CharId == obj.CharId
                && ItemId == obj.ItemId
                && CharName == obj.CharName;
        }
    }
}
