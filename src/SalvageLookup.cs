using System.Collections.Generic;

namespace Warehouse
{
    internal class SalvageLookup
    {
        /// <summary>
        /// A lookup table for MaterialType => Salvage Bag WCIDs
        /// </summary>
        public static Dictionary<int, int> MaterialSalvage = new Dictionary<int, int>()
        {
            {1, 20983},     // Ceramic
            {2, 21067},     // Porcelain
            {3, 0},         // ======= Cloth =======
            {4, 20987},     // Linen
            {5, 20992},     // Satin
            {6, 21076},     // Silk
            {7, 20994},     // Velvet
            {8, 20995},     // Wool
            {9, 0},         // ======= Gems =======
            {10, 21034},    // Agate
            {11, 21035},    // Amber
            {12, 21036},    // Amethyst
            {13, 21037},    // Aquamarine
            {14, 21038},    // Azurite
            {15, 21039},    // Black Garnet
            {16, 21040},    // Black Opal
            {17, 21041},    // Bloodstone
            {18, 21043},    // Carnelian
            {19, 21044},    // Citrine
            {20, 21046},    // Diamond
            {21, 21048},    // Emerald
            {22, 21049},    // Fire Opal
            {23, 21050},    // Green Garnet
            {24, 21051},    // Green Jade
            {25, 21053},    // Hematite
            {26, 21054},    // Imperial Topaz
            {27, 21056},    // Jet
            {28, 21057},    // Lapis Lazuli
            {29, 21058},    // Lavender Jade
            {30, 21060},    // Malachite
            {31, 21062},    // Moonstone
            {32, 21064},    // Onyx
            {33, 21065},    // Opal
            {34, 21066},    // Peridot
            {35, 21069},    // Red Garnet
            {36, 21070},    // Red Jade
            {37, 21071},    // Rose Quartz
            {38, 21072},    // Ruby
            {39, 21074},    // Sapphire
            {40, 21078},    // Smokey Quartz
            {41, 21079},    // Sunstone
            {42, 21081},    // Tiger Eye
            {43, 21082},    // Tourmaline
            {44, 21083},    // Turquoise
            {45, 21084},    // White Jade
            {46, 21085},    // White Quartz
            {47, 21086},    // White Sapphire
            {48, 21087},    // Yellow Garnet
            {49, 21088},    // Yellow Topaz
            {50, 21089},    // Zircon
            {51, 21055},    // Ivory
            {52, 21059},    // Leather
            {53, 20981},    // Armoredillo Hide
            {54, 21052},    // Gromnie Hide
            {55, 20991},    // Reedshark Hide
            {56, 0},        // ======= Metal =======
            {57, 21042},    // Brass
            {58, 20982},    // Bronze
            {59, 21045},    // Copper
            {60, 20984},    // Gold
            {61, 20986},    // Iron
            {62, 21068},    // Pyreal
            {63, 21077},    // Silver
            {64, 20993},    // Steel
            {65, 0},        // ======= Stone =======
            {66, 20980},    // Alabaster
            {67, 20985},    // Granite
            {68, 21061},    // Marble
            {69, 21063},    // Obsidian
            {70, 21073},    // Sandstone
            {71, 21075},    // Serpentine
            {72, 0},        // ======= Wood =======
            {73, 21047},    // Ebony
            {74, 20988},    // Mahogany
            {75, 20989},    // Oak
            {76, 20990},    // Pine
            {77, 21080}     // Teak
        };
        public static Dictionary<int, string> MaterialNames = new Dictionary<int, string>()
        {
            {1, "Ceramic"},
            {2, "Porcelain"},
            {3, ""},         // ======= Cloth =======
            {4, "Linen"},
            {5, "Satin"},
            {6, "Silk"},
            {7, "Velvet"},
            {8, "Wool"},
            {9, ""},         // ======= Gems =======
            {10, "Agate"},
            {11, "Amber"},
            {12, "Amethyst"},
            {13, "Aquamarine"},
            {14, "Azurite"},
            {15, "Black Garnet"},
            {16, "Black Opal"},
            {17, "Bloodstone"},
            {18, "Carnelian"},
            {19, "Citrine"},
            {20, "Diamond"},
            {21, "Emerald"},
            {22, "Fire Opal"},
            {23, "Green Garnet"},
            {24, "Green Jade"},
            {25, "Hematite"},
            {26, "Imperial Topaz"},
            {27, "Jet"},
            {28, "Lapis Lazuli"},
            {29, "Lavender Jade"},
            {30, "Malachite"},
            {31, "Moonstone"},
            {32, "Onyx"},
            {33, "Opal"},
            {34, "Peridot"},
            {35, "Red Garnet"},
            {36, "Red Jade"},
            {37, "Rose Quartz"},
            {38, "Ruby"},
            {39, "Sapphire"},
            {40, "Smokey Quartz"},
            {41, "Sunstone"},
            {42, "Tiger Eye"},
            {43, "Tourmaline"},
            {44, "Turquoise"},
            {45, "White Jade"},
            {46, "White Quartz"},
            {47, "White Sapphire"},
            {48, "Yellow Garnet"},
            {49, "Yellow Topaz"},
            {50, "Zircon"},
            {51, "Ivory"},
            {52, "Leather"},
            {53, "Armoredillo Hide"},
            {54, "Gromnie Hide"},
            {55, "Reedshark Hide"},
            {56, ""},        // ======= Metal =======
            {57, "Brass"},
            {58, "Bronze"},
            {59, "Copper"},
            {60, "Gold"},
            {61, "Iron"},
            {62, "Pyreal"},
            {63, "Silver"},
            {64, "Steel"},
            {65, ""},        // ======= Stone =======
            {66, "Alabaster"},
            {67, "Granite"},
            {68, "Marble"},
            {69, "Obsidian"},
            {70, "Sandstone"},
            {71, "Serpentine"},
            {72, ""},        // ======= Wood =======
            {73, "Ebony"},
            {74, "Mahogany"},
            {75, "Oak"},
            {76, "Pine"},
            {77, "Teak"},
        };
    }
}
