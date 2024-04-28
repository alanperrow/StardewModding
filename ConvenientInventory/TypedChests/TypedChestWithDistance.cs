namespace ConvenientInventory.TypedChests
{
    internal class TypedChestWithDistance
    {
        public ITypedChest TypedChest { get; private set; }

        public double Distance { get; private set; }

        public TypedChestWithDistance(ITypedChest chest, double distance)
        {
            TypedChest = chest;
            Distance = distance;
        }
    }
}
