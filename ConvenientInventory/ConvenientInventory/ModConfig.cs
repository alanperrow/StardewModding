namespace ConvenientInventory
{
	internal class ModConfig
	{
		public int Range { get; set; } = 5;

		public bool IsQuickStackIntoBuildingsWithInventories { get; set; } = false;

		public bool IsQuickStackOverflowItems { get; set; } = true;
	}
}
