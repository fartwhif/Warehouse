using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace Warehouse
{
    public partial class PluginCore
    {
        private string ConnectionString => "Data Source='" + DBFilePath + "';Version=3;Pooling=True;Max Pool Size=100;";
        private string DBFilePath => Pathy.Combine(DataDir, $"{MakeValidFileName(Core.CharacterFilter.AccountName)}-warehouse.db");
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
        //private SQLiteConnection db = null;
        //private void DBClose()
        //{
        //    if (db != null)
        //    {
        //        try { db.Close(); }
        //        catch { }
        //        finally { db = null; }
        //    }
        //}
        private void DBOpen()
        {
            //if (db != null)
            //{
            //    return;
            //}
            if (!File.Exists(DBFilePath))
            {
                File.Copy(DBDefaultFilePath, DBFilePath);
            }
            //db = new SQLiteConnection("Data Source='" + DBFilePath + "';Version=3;");
            //db.Open();
        }
        private Character DBSearchChars(string search)
        {
            string cmdStr = "select * from item WHERE char_name LIKE @Search";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
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
            }
            return null;
        }
        private enum DBSearchItemsBy
        {
            Substring,
            SubstringCharId,
        }
        private List<Item> DBSearchItems(string search, int charId, DBSearchItemsBy searchBy)
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
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
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
        private DBSyncAction DBInsertOrUpdateItem(Item item)
        {
            Item existant = DBSelectItem(item.ItemId);
            if (existant != null)
            {
                item.DbId = existant.DbId;
                if (item.DBEquals(existant))
                {
                    return DBSyncAction.None;
                }
                DBUpdateItem(item);
                return DBSyncAction.Update;
            }
            else
            {
                DBInsertItem(item);
                return DBSyncAction.Insert;
            }
        }
        private void DBDeleteItem(int dbId)
        {
            string cmdStr = "DELETE FROM item where id = @DbId";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
                {
                    command.Parameters.AddWithValue("@DbId", dbId);
                    int affectedRows = command.ExecuteNonQuery();
                }
            }
        }
        private List<Item> DBSelectItemsByCharId(int charId)
        {
            string cmdStr = "select * from item WHERE char_id = @CharId";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
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
        }
        private Item DBSelectItem(int charId, int itemId)
        {
            string cmdStr = "select * from item WHERE char_id = @CharId AND item_id = @ItemId";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
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
            }
            return null;
        }
        private Item DBSelectItem(int itemId)
        {
            string cmdStr = "select * from item WHERE item_id = @ItemId";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
                {
                    command.Parameters.AddWithValue("@ItemId", itemId);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return DBReadRow(reader);
                    }
                }
            }
            return null;
        }
        private void DBInsertItem(Item item)
        {
            string cmdStr = "INSERT INTO item (char_id ,char_name ,item_id, item_name) VALUES (@CharId, @CharName, @ItemId, @ItemName)";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
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
        }
        private void DBUpdateItem(Item item)
        {
            string cmdStr = "UPDATE item SET char_id = @CharId, char_name = @CharName, item_id = @ItemId, item_name = @ItemName WHERE id = @DbId";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
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
        }
        private int DBGetLastInsertedId(SQLiteConnection cnctn)
        {
            using (SQLiteCommand cmd = cnctn.CreateCommand())
            {
                cmd.CommandText = @"SELECT last_insert_rowid()";
                cmd.ExecuteNonQuery();
                int lastID = Convert.ToInt32(cmd.ExecuteScalar());
                return lastID;
            }
        }
        private class ConfigurationSetting
        {
            public int Id { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public bool DBEquals(ConfigurationSetting obj)
            {
                return Id == obj.Id
                    && Key == obj.Key
                    && Value == obj.Value;
            }
            public override string ToString()
            {
                return $"Setting, {Key} = {Value}";
            }
        }
        private ConfigurationSetting DBGetSetting(string key)
        {
            string cmdStr = "select * from config WHERE setting_key = @SettingKey";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
                {
                    command.Parameters.AddWithValue("@SettingKey", key);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return new ConfigurationSetting()
                        {
                            Id = Convert.ToInt32((long)reader["id"]),
                            Key = (string)reader["setting_key"],
                            Value = (string)reader["setting_value"]
                        };
                    }
                }
            }
            return null;
        }
        private DBSyncAction DBSetSetting(ConfigurationSetting setting)
        {
            ConfigurationSetting existant = DBGetSetting(setting.Key);
            if (existant != null)
            {
                setting.Id = existant.Id;
                if (setting.DBEquals(existant))
                {
                    return DBSyncAction.None;
                }
                DBUpdateSetting(setting);
                return DBSyncAction.Update;
            }
            else
            {
                DBInsertSetting(setting);
                return DBSyncAction.Insert;
            }
        }
        private void DBUpdateSetting(ConfigurationSetting setting)
        {
            string cmdStr = "UPDATE config SET setting_key = @SettingKey, setting_value = @SettingValue WHERE id = @Id";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
                {
                    command.Parameters.AddWithValue("@Id", setting.Id);
                    command.Parameters.AddWithValue("@SettingKey", setting.Key);
                    command.Parameters.AddWithValue("@SettingValue", setting.Value);
                    int affectedRows = command.ExecuteNonQuery();
                }
            }
        }
        private void DBInsertSetting(ConfigurationSetting setting)
        {
            string cmdStr = "INSERT INTO config (setting_key ,setting_value) VALUES (@SettingKey, @SettingValue)";
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(cmdStr, connection))
                {
                    command.Parameters.AddWithValue("@SettingKey", setting.Key);
                    command.Parameters.AddWithValue("@SettingValue", setting.Value);
                    int affectedRows = command.ExecuteNonQuery();
                    setting.Id = DBGetLastInsertedId(connection);
                }
            }
        }
    }
}