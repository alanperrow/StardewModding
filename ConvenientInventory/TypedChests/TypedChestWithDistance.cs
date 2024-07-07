namespace ConvenientInventory.TypedChests
{
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
