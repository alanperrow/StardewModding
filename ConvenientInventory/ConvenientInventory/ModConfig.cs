using StardewModdingAPI;

namespace ConvenientInventory
{
	public class ModConfig
	{
		public bool IsEnableQuickStack { get; set; } = true;

		public int QuickStackRange { get; set; } = 5;

		public bool IsQuickStackIntoBuildingsWithInventories { get; set; } = true;

		public bool IsQuickStackOverflowItems { get; set; } = true;

		public bool IsQuickStackTooltipDrawNearbyChests { get; set; } = true;

		public bool IsEnableFavoriteItems { get; set; } = true;

		public int FavoriteItemsHighlightTextureChoice { get; set; } = 2;

		public SButton FavoriteItemsKeyboardHotkey { get; set; } = SButton.LeftControl;

		public SButton FavoriteItemsControllerHotkey { get; set; } = SButton.LeftStick;
	}
}
