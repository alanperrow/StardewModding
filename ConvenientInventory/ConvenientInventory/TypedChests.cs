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

		public static ChestType GetChestType(Chest chest)
		{
			if (chest.SpecialChestType != Chest.SpecialChestTypes.None)
			{
				return ChestType.Special;
			}

			switch (chest.ParentSheetIndex)
			{
				case 130:
				default:
					return ChestType.Normal;
				case 232:
					return ChestType.Stone;
				case 216:
					return ChestType.MiniFridge;
			}
		}

		public static bool IsBuildingChestType(ChestType chestType)
        {
			return chestType == ChestType.Mill || chestType == ChestType.JunimoHut;
        }
	}
}
