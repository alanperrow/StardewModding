namespace ConvenientInventory.TypedChests
{
    /// <summary>
    /// Wrapper for <see cref="TypedChests.TypedChest"/>, holding a distance value for sorting purposes.
    /// </summary>
    internal class TypedChestWithDistance
    {
        public TypedChest TypedChest { get; private set; }

        public double Distance { get; private set; }

        public TypedChestWithDistance(TypedChest chest, double distance)
        {
            TypedChest = chest;
            Distance = distance;
        }
    }
}
