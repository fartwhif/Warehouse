using System.Collections.Generic;

namespace Warehouse
{
    /// <remarks>Adapted from Mag-Filter by Magnus, THANK YOU!!!</remarks>
    class CharSelectCoordFinder
    {
        private const int XPixelOffset = 121;
        private const int YTopOfBox = 209;
        private const int YBottomOfBox = 532;
        private const int YSpecialPixelOffset = 30; // not sure what causes the need for this, the window Titlebar?
        public static int GetCharacterIndexById(List<Character> characters, int id)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        public static Vector2D GetCharacterListItemLocationByIndex(int characterSlotCount, int index)
        {
            float characterNameSize = (YBottomOfBox - YTopOfBox) / (float)characterSlotCount;
            int yOffset = (int)(YTopOfBox + (characterNameSize / 2) + (characterNameSize * index));
            return new Vector2D() { X = XPixelOffset, Y = yOffset + YSpecialPixelOffset };
        }
    }
}
