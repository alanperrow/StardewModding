using StardewValley.Objects;

namespace ConvenientInventory
{
    internal class ChestWithDistance
    {
        public Chest Chest { get; private set; }

        public double Distance { get; private set; }

        public ChestWithDistance(Chest chest, double distance)
        {
            Chest = chest;
            Distance = distance;
        }
    }
}
