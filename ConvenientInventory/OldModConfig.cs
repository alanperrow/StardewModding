using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;

namespace ConvenientInventory
{
    [Obsolete("Only exists to migrate users from old config format. Use 'ModConfig' instead.")]
    public class OldModConfig
    {
        // ===== Quick Stack To Nearby Chests =====
        public bool IsEnableQuickStack { get; set; } = true;

        public string QuickStackRange { get; set; } = ConfigHelper.QuickStackRange_Default;

        public bool IsQuickStackIntoBuildingsWithInventories { get; set; } = true;

        public bool IsQuickStackIntoDressers { get; set; } = true;

        public bool IsQuickStackOverflowItems { get; set; } = true;

        public bool IsQuickStackTooltipDrawNearbyChests { get; set; } = true;

        public bool IsEnableQuickStackHotkey { get; set; } = false;

        public KeybindList QuickStackKeyboardHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.K });

        public KeybindList QuickStackControllerHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftStick });

        public bool IsQuickStackIgnoreItemQuality { get; set; } = false;

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

        /// <summary>
        /// Migrates this <see cref="OldModConfig"/> to a new instance of <see cref="ModConfig"/>, mapping each property
        /// to their corresponding property in the new type.
        /// </summary>
        public ModConfig Migrate() => new()
        {
            QuickStack = new ModConfig.QuickStackConfig()
            {
                IsEnabled = IsEnableQuickStack,
                Range = QuickStackRange,
                IntoMills = IsQuickStackIntoBuildingsWithInventories,
                IntoJunimoHuts = IsQuickStackIntoBuildingsWithInventories,
                IntoDressers = IsQuickStackIntoDressers,
                OverflowItems = IsQuickStackOverflowItems,
                DrawChestsInButtonTooltip = IsQuickStackTooltipDrawNearbyChests,
                IsHotkeyEnabled = IsEnableQuickStackHotkey,
                KeyboardHotkey = QuickStackKeyboardHotkey,
                ControllerHotkey = QuickStackControllerHotkey,
                IsItemQualityIgnored = IsQuickStackIgnoreItemQuality,
                IsAnimationEnabled = IsEnableQuickStackAnimation,
                IsChestAnimationEnabled = IsEnableQuickStackChestAnimation,
                AnimationItemSpeedFactor = QuickStackAnimationItemSpeed,
                AnimationStackSpeedFactor = QuickStackAnimationStackSpeed,
            },
            FavoriteItems = new ModConfig.FavoriteItemsConfig()
            {
                IsEnabled = IsEnableFavoriteItems,
                HighlightTextureChoice = FavoriteItemsHighlightTextureChoice,
                KeyboardHotkey = FavoriteItemsKeyboardHotkey,
                ControllerHotkey = FavoriteItemsControllerHotkey,
            },
            TakeAllButOne = new ModConfig.TakeAllButOneConfig()
            {
                IsEnabled = IsEnableTakeAllButOne,
                KeyboardHotkey = TakeAllButOneKeyboardHotkey,
                ControllerHotkey = TakeAllButOneControllerHotkey,
            },
            AutoOrganizeChest = new ModConfig.AutoOrganizeChestConfig()
            {
                IsEnabled = IsEnableAutoOrganizeChest,
                ShowInstructionsInTooltip = IsShowAutoOrganizeButtonInstructions,
            },
            Miscellaneous = new ModConfig.MiscellaneousConfig()
            {
                IsInventoryPageSideWarpEnabled = IsEnableInventoryPageSideWarp,
            },
        };
    }
}
