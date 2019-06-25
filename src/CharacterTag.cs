namespace Warehouse
{
    internal class CharacterTag
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public string Tag { get; set; }
        public bool DBEquals(CharacterTag obj)
        {
            return Id == obj.Id
                && CharacterId == obj.CharacterId
                && Tag == obj.Tag;
        }
        public override string ToString()
        {
            return $"Tag, {CharacterId} = {Tag}";
        }
    }
}
