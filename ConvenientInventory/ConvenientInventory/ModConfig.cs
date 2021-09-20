namespace ConvenientInventory
{
	internal class ModConfig
	{
		public int QuickStackRange { get; set; } = 5;

		public bool IsQuickStackIntoBuildingsWithInventories { get; set; } = true;

		public bool IsQuickStackOverflowItems { get; set; } = true;

		public bool IsQuickStackTooltipDrawNearbyChests { get; set; } = true;
	}
}
