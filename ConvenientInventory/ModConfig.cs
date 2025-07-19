using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ConvenientInventory
{
    public class ModConfig
    {
        // bool IsModEnabled { get; set; } = true;

        public bool IsMigrated { get; set; } = false;
        /*
         * Investigate migrating from the old config format to the new one.
         * 
         * IDEA: Try to ReadJsonFile as ModConfig. If this `IsMigrated` value is false,
         * that means either (1) the user removed this line from the config, or (2) we just read an OldModConfig file as a ModConfig file.
         * 
         * (1) In this case 
         *  ... ???
         *  Need to avoid accidentally deleting user's config settings if we are incorrect about which config type to read with ReadJsonFile.
         * 
         * (2) In this case we must perform a migration:
         *  load the config file as OldModConfig,
         *  migrate it to ModConfig (setting IsMigrated=true), then
         *  overwrite the config file.
         */

        public QuickStackConfig QuickStack { get; set; } = new();

        public FavoriteItemsConfig FavoriteItems { get; set; } = new();

        public TakeAllButOneConfig TakeAllButOne { get; set; } = new();

        public AutoOrganizeChestConfig AutoOrganizeChest { get; set; } = new();

        public MiscellaneousConfig Miscellaneous { get; set; } = new();

        public class QuickStackConfig
        {
            public bool IsEnabled { get; set; } = true;

            public string Range { get; set; } = ConfigHelper.QuickStackRange_Default;

            public bool IsHotkeyEnabled { get; set; } = true;

            public KeybindList KeyboardHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.K });

            public KeybindList ControllerHotkey { get; set; } = KeybindList.ForSingle(new[] { SButton.LeftStick });

            public bool OverflowItems { get; set; } = true;

            public bool IgnoreItemQuality { get; set; } = false;

            public bool IntoMills { get; set; } = true;

            public bool IntoJunimoHuts { get; set; } = true;

            public bool IntoDressers { get; set; } = true;

            public bool IntoHoppers { get; set; } = true;

            public bool IntoMiniShippingBins { get; set; } = false;

            public bool DrawChestsInButtonTooltip { get; set; } = true;

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
