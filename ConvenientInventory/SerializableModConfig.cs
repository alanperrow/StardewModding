using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ConvenientInventory
{
    /// <summary>Represents the mod configuration to be serialized to and deserialized from the mod's config file.</summary>
    /// <remarks>
    /// This class has been superseded by <see cref="ModConfig"/> but is still used as the config serialization type
    /// for compatibility with existing config files.
    /// </remarks>
    public class SerializableModConfig
    {
        // bool IsModEnabled { get; set; } = true;

        // ===== Quick Stack To Nearby Chests =====
        public bool IsEnableQuickStack { get; set; } = true;

        public string QuickStackRange { get; set; } = ConfigHelper.QuickStackRange_Default;

        public bool IsEnableQuickStackHotkey { get; set; } = false;

        public KeybindList QuickStackKeyboardHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.K });

        public KeybindList QuickStackControllerHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftStick });

        public bool IsQuickStackOverflowItems { get; set; } = true;

        public bool IsQuickStackIgnoreItemQuality { get; set; } = false;

        public bool IsQuickStackIntoMills { get; set; } = true;

        public bool IsQuickStackIntoJunimoHuts { get; set; } = true;

        public bool IsQuickStackIntoDressers { get; set; } = true;

        public bool IsQuickStackIntoHoppers { get; set; } = true;

        public bool IsQuickStackIntoMiniShippingBins { get; set; } = false;

        public bool IsQuickStackTooltipDrawNearbyChests { get; set; } = true;

        public bool IsEnableQuickStackAnimation { get; set; } = true;

        public bool IsEnableQuickStackChestAnimation { get; set; } = true;

        public float QuickStackAnimationItemSpeed { get; set; } = 1.0f;

        public float QuickStackAnimationStackSpeed { get; set; } = 1.0f;

        // ===== Favorite Items =====
        public bool IsEnableFavoriteItems { get; set; } = true;

        public int FavoriteItemsHighlightTextureChoice { get; set; } = 2;

        public KeybindList FavoriteItemsKeyboardHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftAlt });

        public KeybindList FavoriteItemsControllerHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftShoulder });

        // ===== Take All But One =====
        public bool IsEnableTakeAllButOne { get; set; } = true;

        public KeybindList TakeAllButOneKeyboardHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftControl, SButton.LeftShift });

        public KeybindList TakeAllButOneControllerHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftTrigger });

        // ===== Auto Organize Chest =====
        public bool IsEnableAutoOrganizeChest { get; set; } = true;

        public bool IsShowAutoOrganizeButtonInstructions { get; set; } = true;

        // ===== Miscellaneous =====
        public bool IsEnableInventoryPageSideWarp { get; set; } = true;
    }
}
