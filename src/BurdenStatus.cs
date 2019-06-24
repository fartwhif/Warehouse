using System;

namespace Warehouse
{
    public class BurdenStatus
    {
        public int FreeSlots => TotalSlots - OccupiedSlots;
        public int TotalSlots { get; set; }
        public int OccupiedSlots { get; set; }
        public int CurrentBurdenPercentage { get; set; }
        public int CurrentBurdenPercentageCorrected
        {
            get
            {
                if (BurdenCapacity == 0)
                {
                    return 0;
                }
                return (int)(Math.Round(100 * (CurrentBurden / (double)BurdenCapacity)));
            }
        }
        public int CurrentBurden { get; set; }
        public int BurdenCapacity
        {
            get
            {
                if (CurrentBurdenPercentage == 0)
                {
                    return 0;
                }
                return (int)Math.Round(CurrentBurden / (CurrentBurdenPercentage / (double)100)) * 3;
            }
        }
        public override string ToString()
        {
            return $"Items: {OccupiedSlots} / {TotalSlots}  Burden: {CurrentBurden.ToString("N0")} / {BurdenCapacity.ToString("N0")} ( {CurrentBurdenPercentageCorrected}% )";
        }
    }
}
