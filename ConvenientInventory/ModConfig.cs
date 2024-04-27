using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ConvenientInventory
{
    public class ModConfig
    {
        public bool IsEnableQuickStack { get; set; } = true;

        public int QuickStackRange { get; set; } = 5;

        public bool IsQuickStackIntoBuildingsWithInventories { get; set; } = true;

        public bool IsQuickStackOverflowItems { get; set; } = true;

        public bool IsQuickStackTooltipDrawNearbyChests { get; set; } = true;

        public bool IsEnableQuickStackHotkey { get; set; } = false;

        public SButton QuickStackKeyboardHotkey { get; set; } = SButton.K;

        public SButton QuickStackControllerHotkey { get; set; } = SButton.LeftStick;

        public bool IsQuickStackIgnoreItemQuality { get; set; } = false;

        public bool IsEnableQuickStackAnimation { get; set; } = true;

        public bool IsEnableFavoriteItems { get; set; } = true;

        public int FavoriteItemsHighlightTextureChoice { get; set; } = 2;

        public SButton FavoriteItemsKeyboardHotkey { get; set; } = SButton.LeftAlt;

        public SButton FavoriteItemsControllerHotkey { get; set; } = SButton.LeftShoulder;

        public bool IsEnableTakeAllButOne { get; set; } = true;

        public KeybindList TakeAllButOneKeyboardHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftControl, SButton.LeftShift });

        public KeybindList TakeAllButOneControllerHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftTrigger });

        public bool IsEnableInventoryPageSideWarp { get; set; } = true;
    }
}
