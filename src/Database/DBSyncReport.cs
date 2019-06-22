namespace Warehouse
{
    internal class DBSyncReport
    {
        public int NoAction { get; set; }
        public int Insert { get; set; }
        public int Delete { get; set; }
        public int Update { get; set; }
        public override string ToString()
        {
            return $"Item Sync Report: Insert: {Insert}, Delete: {Delete}, Update: {Update}, NoAction: {NoAction}";
        }
        public void Increment(DBSyncAction act)
        {
            switch (act)
            {
                case DBSyncAction.Delete:
                    Delete++;
                    break;
                case DBSyncAction.Insert:
                    Insert++;
                    break;
                case DBSyncAction.None:
                    NoAction++;
                    break;
                case DBSyncAction.Update:
                    Update++;
                    break;
            }
        }
    }
}
