using StardewValley.Objects;

namespace ConvenientInventory.TypedChests
{
	internal enum ChestType
	{
		Normal,
		Stone,
		Fridge,
		MiniFridge,
		Mill,
		JunimoHut,
		Special
	}

	internal class TypedChest
	{
		public Chest Chest { get; private set; }

		public ChestType ChestType { get; private set; }

		public TypedChest(Chest chest, ChestType chestType)
		{
			Chest = chest;
			ChestType = chestType;
		}
	}
}
