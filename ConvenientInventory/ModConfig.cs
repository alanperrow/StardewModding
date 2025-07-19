using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ConvenientInventory
{
    public class ModConfig
    {
        // bool IsModEnabled { get; set; } = true;

        public QuickStackConfig QuickStack { get; set; } = new();

        public FavoriteItemsConfig FavoriteItems { get; set; } = new();

        public TakeAllButOneConfig TakeAllButOne { get; set; } = new();

        public AutoOrganizeChestConfig AutoOrganizeChest { get; set; } = new();

        public MiscellaneousConfig Miscellaneous { get; set; } = new();

        public class QuickStackConfig
        {
            public bool IsEnabled { get; set; } = true;

            public string Range { get; set; } = ConfigHelper.QuickStackRange_Default;

            public bool IntoMills { get; set; } = true;

            public bool IntoJunimoHuts { get; set; } = true;

            public bool IntoDressers { get; set; } = true;

            public bool IntoHoppers { get; set; } = true;

            public bool IntoMiniShippingBins { get; set; } = false;

            public bool OverflowItems { get; set; } = true;

            public bool DrawChestsInButtonTooltip { get; set; } = true;

            public bool IsHotkeyEnabled { get; set; } = true;

            public KeybindList KeyboardHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.K });

            public KeybindList ControllerHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftStick });

            public bool IgnoreItemQuality { get; set; } = false;

            public bool IsAnimationEnabled { get; set; } = true;

            public bool IsChestAnimationEnabled { get; set; } = true;

            public float AnimationItemSpeedFactor { get; set; } = 1.0f;

            public float AnimationStackSpeedFactor { get; set; } = 1.0f;
        }

        public class FavoriteItemsConfig
        {
            public bool IsEnabled { get; set; } = true;

            public int HighlightTextureChoice { get; set; } = 2;

            public KeybindList KeyboardHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftAlt });

            public KeybindList ControllerHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftShoulder });
        }

        public class TakeAllButOneConfig
        {
            public bool IsEnabled { get; set; } = true;

            public KeybindList KeyboardHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftControl, SButton.LeftShift });

            public KeybindList ControllerHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftTrigger });
        }

        public class AutoOrganizeChestConfig
        {
            public bool IsEnabled { get; set; } = true;

            public bool ShowInstructionsInTooltip { get; set; } = true;
        }

        public class MiscellaneousConfig
        {
            public bool IsInventoryPageSideWarpEnabled { get; set; } = true;
        }
    }
}
