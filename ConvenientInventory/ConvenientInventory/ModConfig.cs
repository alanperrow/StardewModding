using StardewModdingAPI;

namespace ConvenientInventory
{
	internal class ModConfig
	{
		public bool IsEnableQuickStack { get; set; } = true;

		public int QuickStackRange { get; set; } = 5;

		public bool IsQuickStackIntoBuildingsWithInventories { get; set; } = true;

		public bool IsQuickStackOverflowItems { get; set; } = true;

		public bool IsQuickStackTooltipDrawNearbyChests { get; set; } = true;

		public bool IsEnableFavoriteItems { get; set; } = true;

		public int FavoriteItemsHighlightTextureChoice { get; set; } = 0;

		public SButton FavoriteItemsKeyboardHotkey { get; set; } = SButton.LeftAlt;

		public SButton FavoriteItemsControllerHotkey { get; set; } = SButton.LeftStick;
	}
}
