namespace Warehouse
{
    public partial class PluginCore
    {
        private bool AllowRetrieve
        {
            get
            {
                ConfigurationSetting g = DBGetSetting("allowretrieve");
                return (g == null ? true : bool.Parse(g.Value));
            }
            set => DBSetSetting(new ConfigurationSetting() { Key = "allowretrieve", Value = value.ToString() });
        }

        private bool PlayerDetectionJump
        {
            get
            {
                ConfigurationSetting g = DBGetSetting("playerdetectionjump");
                return (g == null ? true : bool.Parse(g.Value));
            }
            set => DBSetSetting(new ConfigurationSetting() { Key = "playerdetectionjump", Value = value.ToString() });
        }

        private bool AllowJumpCommand
        {
            get
            {
                ConfigurationSetting g = DBGetSetting("allowjumpcmd");
                return (g == null ? true : bool.Parse(g.Value));
            }
            set => DBSetSetting(new ConfigurationSetting() { Key = "allowjumpcmd", Value = value.ToString() });
        }
    }
}
