using System;

namespace Warehouse
{
    public class Character
    {
        public readonly int Id;
        public readonly string Name;
        public readonly TimeSpan DelTim;
        public Character(int id, string name, int timeout)
        {
            DelTim = TimeSpan.FromSeconds(timeout);
            Id = id;
            Name = name;
        }
    }
}
