using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ConvenientInventory.Compatibility
{
    public static class ApiHelper
    {
        public static bool IsWearMoreRingsInstalled { get; set; }

        public static IChestsAnywhereApi ChestsAnywhereApi { get; private set; }

        public static void Initialize(IChestsAnywhereApi api)
        {
            ChestsAnywhereApi = api;
        }

        public static void Initialize(IGenericModConfigMenuApi api, ModConfig config, IManifest modManifest, IModHelper helper, IMonitor monitor)
        {
            // == Config Validation ==
            bool isChestsAnywhereInstalled = ChestsAnywhereApi != null;
            if (!isChestsAnywhereInstalled && ConfigHelper.ParseQuickStackRangeFromConfig(config.QuickStackRange) == ConfigHelper.QuickStackRange_GlobalInt)
            {
                // QuickStackRange: "Global" option is only supported if the Chests Anywhere mod is installed.
                // If Chests Anywhere API is not found and config value is loaded as "Global", log a warning message to SMAPI and overwrite the config value to "Location".
                monitor.Log(helper.Translation.Get("ModConfigMenu.QuickStackRange.ChestsAnywhereGlobalWarning"), LogLevel.Warn);
                config.QuickStackRange = ConfigHelper.FormatQuickStackRange(ConfigHelper.QuickStackRange_LocationInt);
                helper.WriteConfig(config);
            }

            // == Register GMCM ==
            api.Register(
                mod: modManifest,
                reset: () =>
                {
                    config = new ModConfig();
                    ModEntry.Config = config;
                },
                save: () => helper.WriteConfig(config)
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.QuickStackToNearbyChests")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableQuickStack,
                setValue: value => config.IsEnableQuickStack = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStack.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStack.Desc")
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => ConfigHelper.ParseQuickStackRangeFromConfig(config.QuickStackRange),
                setValue: value => config.QuickStackRange = ConfigHelper.FormatQuickStackRange(value),
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackRange.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackRange.Desc"),
                min: 1,
                max: isChestsAnywhereInstalled ? ConfigHelper.QuickStackRange_GlobalInt : ConfigHelper.QuickStackRange_LocationInt,
                interval: 1,
                formatValue: ConfigHelper.FormatQuickStackRange
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableQuickStackHotkey,
                setValue: value => config.IsEnableQuickStackHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackHotkey.Desc")
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.QuickStackKeyboardHotkey,
                setValue: value => config.QuickStackKeyboardHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackKeyboardHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackKeyboardHotkey.Desc")
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.QuickStackControllerHotkey,
                setValue: value => config.QuickStackControllerHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackControllerHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackControllerHotkey.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsQuickStackIntoBuildingsWithInventories,
                setValue: value => config.IsQuickStackIntoBuildingsWithInventories = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoBuildingsWithInventories.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoBuildingsWithInventories.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsQuickStackIntoDressers,
                setValue: value => config.IsQuickStackIntoDressers = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoDressers.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoDressers.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsQuickStackOverflowItems,
                setValue: value => config.IsQuickStackOverflowItems = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsQuickStackIgnoreItemQuality,
                setValue: value => config.IsQuickStackIgnoreItemQuality = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIgnoreItemQuality.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIgnoreItemQuality.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsQuickStackTooltipDrawNearbyChests,
                setValue: value => config.IsQuickStackTooltipDrawNearbyChests = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackTooltipDrawNearbyChests.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackTooltipDrawNearbyChests.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableQuickStackAnimation,
                setValue: value =>
                {
                    // In GMCM, we tie together IsEnableQuickStackAnimation with IsEnableQuickStackChestAnimation.
                    // Most users should only care about whether the entire animation is enabled or disabled.
                    // Keeping these as separate config values, however, allows users to manually edit the config file to set either value independently, if necessary.
                    config.IsEnableQuickStackAnimation = value;
                    config.IsEnableQuickStackChestAnimation = value;
                },
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackAnimation.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackAnimation.Desc")
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.QuickStackAnimationItemSpeed,
                setValue: value => config.QuickStackAnimationItemSpeed = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackAnimationItemSpeed.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackAnimationItemSpeed.Desc"),
                min: 0.5f,
                max: 3f,
                interval: 0.1f
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.QuickStackAnimationStackSpeed,
                setValue: value => config.QuickStackAnimationStackSpeed = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackAnimationStackSpeed.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackAnimationStackSpeed.Desc"),
                min: 0.5f,
                max: 3f,
                interval: 0.1f
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.FavoriteItems")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableFavoriteItems,
                setValue: value => config.IsEnableFavoriteItems = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableFavoriteItems.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableFavoriteItems.Desc")
            );

            string[] highlightStyleDescriptions =
            {
                $"0: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-0")}",
                $"1: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-1")}",
                $"2: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-2")}",
                $"3: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-3")}",
                $"4: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-4")}",
                $"5: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-5")}",
                $"6: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-6")}",
            };
            api.AddTextOption(
                mod: modManifest,
                getValue: () => highlightStyleDescriptions[config.FavoriteItemsHighlightTextureChoice],
                setValue: value =>
                {
                    config.FavoriteItemsHighlightTextureChoice = int.Parse(value[..1]);
                    CachedTextures.FavoriteItemsHighlight = Game1.content.Load<Texture2D>(CachedTextures.ModAssetPrefix + $"favoriteHighlight_{value[0]}");
                },
                name: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc"),
                allowedValues: highlightStyleDescriptions
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.FavoriteItemsKeyboardHotkey,
                setValue: value => config.FavoriteItemsKeyboardHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsKeyboardHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsKeyboardHotkey.Desc")
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.FavoriteItemsControllerHotkey,
                setValue: value => config.FavoriteItemsControllerHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsControllerHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsControllerHotkey.Desc")
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.TakeAllButOne")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableTakeAllButOne,
                setValue: value => config.IsEnableTakeAllButOne = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableTakeAllButOne.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableTakeAllButOne.Desc")
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.TakeAllButOneKeyboardHotkey,
                setValue: value => config.TakeAllButOneKeyboardHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.TakeAllButOneKeyboardHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.TakeAllButOneKeyboardHotkey.Desc")
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.TakeAllButOneControllerHotkey,
                setValue: value => config.TakeAllButOneControllerHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.TakeAllButOneControllerHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.TakeAllButOneControllerHotkey.Desc")
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.AutoOrganizeChest")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableAutoOrganizeChest,
                setValue: value => config.IsEnableAutoOrganizeChest = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableAutoOrganizeChest.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableAutoOrganizeChest.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsShowAutoOrganizeButtonInstructions,
                setValue: value => config.IsShowAutoOrganizeButtonInstructions = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsShowAutoOrganizeButtonInstructions.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsShowAutoOrganizeButtonInstructions.Desc")
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.Miscellaneous")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.IsEnableInventoryPageSideWarp,
                setValue: value => config.IsEnableInventoryPageSideWarp = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableInventoryPageSideWarp.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableInventoryPageSideWarp.Desc")
            );
        }
    }
}
