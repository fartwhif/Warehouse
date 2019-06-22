using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace Warehouse
{
    public partial class PluginCore
    {
        private string DBFilePath => Pathy.Combine(DataDir, "warehouse.db");
        private string DataDir
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string dataDir = System.IO.Path.GetDirectoryName(assembly.CodeBase);
                return dataDir.StartsWith("file:\\") ? dataDir.Substring(6) : dataDir;
            }
        }
        private string DBDefaultFilePath => Pathy.Combine(DataDir, "warehouse.default.db");
        private SQLiteConnection db = null;
        private void DBClose()
        {
            if (db != null)
            {
                try { db.Close(); }
                catch { }
                finally { db = null; }
            }
        }
        private void DBOpen()
        {
            if (db != null)
            {
                return;
            }
            if (!File.Exists(DBFilePath))
            {
                File.Copy(DBDefaultFilePath, DBFilePath);
            }
            db = new SQLiteConnection("Data Source='" + DBFilePath + "';Version=3;");
            db.Open();
        }
        private static Character DBSearchChars(SQLiteConnection connection, string search)
        {
            string cmdStr = "select * from item WHERE char_name LIKE @Search";
            using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
            {
                search = search.Trim();
                command.Parameters.AddWithValue("@Search", search);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Item item = DBReadRow(reader);
                    return new Character(item.CharId, item.CharName, 0);
                }
            }
            return null;
        }
        private enum DBSearchItemsBy
        {
            Substring,
            SubstringCharId,
        }
        private static List<Item> DBSearchItems(SQLiteConnection connection, string search, int charId, DBSearchItemsBy searchBy)
        {
            string cmdStr = "";
            switch (searchBy)
            {
                case DBSearchItemsBy.Substring:
                    cmdStr = "select * from item WHERE item_name LIKE @Search";
                    break;
                case DBSearchItemsBy.SubstringCharId:
                    cmdStr = "select * from item WHERE item_name LIKE @Search AND char_id = @CharId";
                    break;
            }
            using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
            {
                switch (searchBy)
                {
                    case DBSearchItemsBy.Substring:
                        search = $"%{search}%";
                        command.Parameters.AddWithValue("@Search", search);
                        break;
                    case DBSearchItemsBy.SubstringCharId:
                        search = $"%{search}%";
                        command.Parameters.AddWithValue("@Search", search);
                        command.Parameters.AddWithValue("@CharId", charId);
                        break;
                }
                SQLiteDataReader reader = command.ExecuteReader();
                List<Item> items = new List<Item>();
                while (reader.Read())
                {
                    items.Add(DBReadRow(reader));
                }
                return items;
            }
        }
        private static Item DBReadRow(SQLiteDataReader reader)
        {
            return new Item()
            {
                DbId = Convert.ToInt32((long)reader["id"]),
                CharId = Convert.ToInt32((long)reader["char_id"]),
                ItemId = Convert.ToInt32((long)reader["item_id"]),
                CharName = (string)reader["char_name"],
                ItemName = (string)reader["item_name"]
            };
        }
        private static DBSyncAction DBInsertOrUpdateItem(SQLiteConnection db, Item item)
        {
            Item existant = DBSelectItem(db, item.ItemId);
            if (existant != null)
            {
                item.DbId = existant.DbId;
                if (item.DBEquals(existant))
                {
                    return DBSyncAction.None;
                }
                DBUpdateItem(db, item);
                return DBSyncAction.Update;
            }
            else
            {
                DBInsertItem(db, item);
                return DBSyncAction.Insert;
            }
        }
        private static void DBDeleteItem(SQLiteConnection connection, int dbId)
        {
            string cmdStr = "DELETE FROM item where id = @DbId";
            using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
            {
                command.Parameters.AddWithValue("@DbId", dbId);
                int affectedRows = command.ExecuteNonQuery();
            }
        }
        private static List<Item> DBSelectItemsByCharId(SQLiteConnection connection, int charId)
        {
            string cmdStr = "select * from item WHERE char_id = @CharId";
            using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
            {
                command.Parameters.AddWithValue("@CharId", charId);
                SQLiteDataReader reader = command.ExecuteReader();
                List<Item> items = new List<Item>();
                while (reader.Read())
                {
                    items.Add(DBReadRow(reader));
                }
                return items;
            }
        }
        private static Item DBSelectItem(SQLiteConnection connection, int charId, int itemId)
        {
            string cmdStr = "select * from item WHERE char_id = @CharId AND item_id = @ItemId";
            //using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
            {
                command.Parameters.AddWithValue("@CharId", charId);
                command.Parameters.AddWithValue("@ItemId", itemId);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return DBReadRow(reader);
                }
            }
            return null;
        }
        private static Item DBSelectItem(SQLiteConnection connection, int itemId)
        {
            string cmdStr = "select * from item WHERE item_id = @ItemId";
            //using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
            {
                command.Parameters.AddWithValue("@ItemId", itemId);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return DBReadRow(reader);
                }
            }
            return null;
        }
        private static void DBInsertItem(SQLiteConnection connection, Item item)
        {
            string cmdStr = "INSERT INTO item (char_id ,char_name ,item_id, item_name) VALUES (@CharId, @CharName, @ItemId, @ItemName)";
            using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
            {
                command.Parameters.AddWithValue("@CharId", item.CharId);
                command.Parameters.AddWithValue("@CharName", item.CharName);
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                command.Parameters.AddWithValue("@ItemName", item.ItemName);
                int affectedRows = command.ExecuteNonQuery();
                item.DbId = DBGetLastInsertedId(connection);
            }
        }
        private static void DBUpdateItem(SQLiteConnection connection, Item item)
        {
            string cmdStr = "UPDATE item SET char_id = @CharId, char_name = @CharName, item_id = @ItemId, item_name = @ItemName WHERE id = @DbId";
            using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
            {
                command.Parameters.AddWithValue("@DbId", item.DbId);
                command.Parameters.AddWithValue("@CharId", item.CharId);
                command.Parameters.AddWithValue("@CharName", item.CharName);
                command.Parameters.AddWithValue("@ItemId", item.ItemId);
                command.Parameters.AddWithValue("@ItemName", item.ItemName);
                int affectedRows = command.ExecuteNonQuery();
            }
        }
        private static int DBGetLastInsertedId(SQLiteConnection cnctn)
        {
            using (SQLiteCommand cmd = cnctn.CreateCommand())
            {
                cmd.CommandText = @"SELECT last_insert_rowid()";
                cmd.ExecuteNonQuery();
                int lastID = Convert.ToInt32(cmd.ExecuteScalar());
                return lastID;
            }
        }
    }
}