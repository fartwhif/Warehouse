namespace Warehouse
{
    internal class ConfigurationSetting
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
}
