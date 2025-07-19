using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;

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
            if (!isChestsAnywhereInstalled && ConfigHelper.ParseQuickStackRangeFromConfig(config.QuickStack.Range) == ConfigHelper.QuickStackRange_GlobalInt)
            {
                // QuickStackRange: "Global" option is only supported if the Chests Anywhere mod is installed.
                // If Chests Anywhere API is not found and config value is loaded as "Global", log a warning message to SMAPI and overwrite the config value to "Location".
                monitor.Log(helper.Translation.Get("ModConfigMenu.QuickStackRange.ChestsAnywhereGlobalWarning"), LogLevel.Warn);
                config.QuickStack.Range = ConfigHelper.FormatQuickStackRange(ConfigHelper.QuickStackRange_LocationInt);
                config.Save();
            }

            // == Register GMCM ==
            api.Register(
                mod: modManifest,
                reset: config.Reset,
                save: config.Save
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.QuickStackToNearbyChests")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsEnabled,
                setValue: value => config.QuickStack.IsEnabled = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStack.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStack.Desc")
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => ConfigHelper.ParseQuickStackRangeFromConfig(config.QuickStack.Range),
                setValue: value => config.QuickStack.Range = ConfigHelper.FormatQuickStackRange(value),
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackRange.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackRange.Desc"),
                min: 1,
                max: isChestsAnywhereInstalled ? ConfigHelper.QuickStackRange_GlobalInt : ConfigHelper.QuickStackRange_LocationInt,
                interval: 1,
                formatValue: ConfigHelper.FormatQuickStackRange
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsHotkeyEnabled,
                setValue: value => config.QuickStack.IsHotkeyEnabled = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackHotkey.Desc")
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.QuickStack.KeyboardHotkey,
                setValue: value => config.QuickStack.KeyboardHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackKeyboardHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackKeyboardHotkey.Desc")
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.QuickStack.ControllerHotkey,
                setValue: value => config.QuickStack.ControllerHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackControllerHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackControllerHotkey.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.OverflowItems,
                setValue: value => config.QuickStack.OverflowItems = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackOverflowItems.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IgnoreItemQuality,
                setValue: value => config.QuickStack.IgnoreItemQuality = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIgnoreItemQuality.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIgnoreItemQuality.Desc")
            );

            //api.AddBoolOption(
            //    mod: modManifest,
            //    getValue: () => config.IsQuickStackIntoBuildingsWithInventories,
            //    setValue: value => config.IsQuickStackIntoBuildingsWithInventories = value,
            //    name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoBuildingsWithInventories.Name"),
            //    tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoBuildingsWithInventories.Desc")
            //);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoMills,
                setValue: value => config.QuickStack.IntoMills = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoMills.Name"), // TODO: translation text
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoMills.Desc") // TODO: translation text
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoMills,
                setValue: value => config.QuickStack.IntoMills = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoJunimoHuts.Name"), // TODO: translation text
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoJunimoHuts.Desc") // TODO: translation text
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoDressers,
                setValue: value => config.QuickStack.IntoDressers = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoDressers.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoDressers.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoHoppers,
                setValue: value => config.QuickStack.IntoHoppers = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoHoppers.Name"), // TODO: translation text
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoHoppers.Desc") // TODO: translation text
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoMiniShippingBins,
                setValue: value => config.QuickStack.IntoMiniShippingBins = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoMiniShippingBins.Name"), // TODO: translation text
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackIntoMiniShippingBins.Desc") // TODO: translation text
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.DrawChestsInButtonTooltip,
                setValue: value => config.QuickStack.DrawChestsInButtonTooltip = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsQuickStackTooltipDrawNearbyChests.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsQuickStackTooltipDrawNearbyChests.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsAnimationEnabled,
                setValue: value =>
                {
                    // In GMCM, we tie together IsAnimationEnabled with IsChestAnimationEnabled.
                    // Most users should only care about whether the entire animation is enabled or disabled.
                    // Keeping these as separate config values, however, allows users to manually edit the config file to set either value independently, if necessary.
                    config.QuickStack.IsAnimationEnabled = value;
                    config.QuickStack.IsChestAnimationEnabled = value;
                },
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackAnimation.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableQuickStackAnimation.Desc")
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.QuickStack.AnimationItemSpeedFactor,
                setValue: value => config.QuickStack.AnimationItemSpeedFactor = value,
                name: () => helper.Translation.Get("ModConfigMenu.QuickStackAnimationItemSpeed.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.QuickStackAnimationItemSpeed.Desc"),
                min: 0.5f,
                max: 3f,
                interval: 0.1f
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.QuickStack.AnimationStackSpeedFactor,
                setValue: value => config.QuickStack.AnimationStackSpeedFactor = value,
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
                getValue: () => config.FavoriteItems.IsEnabled,
                setValue: value => config.FavoriteItems.IsEnabled = value,
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
                $"7: {helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc-7")}",
            };
            api.AddTextOption(
                mod: modManifest,
                getValue: () => highlightStyleDescriptions[config.FavoriteItems.HighlightTextureChoice],
                setValue: value =>
                {
                    config.FavoriteItems.HighlightTextureChoice = int.Parse(value[..1]);
                    CachedTextures.FavoriteItemsHighlight = Game1.content.Load<Texture2D>(CachedTextures.ModAssetPrefix + $"favoriteHighlight_{value[0]}");
                },
                name: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsHighlightTextureChoice.Desc"),
                allowedValues: highlightStyleDescriptions
            );

            // TODO: Visualize the highlight texture choice.
            //       Probably doesn't need to update live, but see SplitscreenImproved for reference if it does.

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.FavoriteItems.KeyboardHotkey,
                setValue: value => config.FavoriteItems.KeyboardHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsKeyboardHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsKeyboardHotkey.Desc")
            );

            // Compare this snippet from ConvenientInventory/ApiHelper.cs:
            //       string textureChoice = e.Name.BaseName.Split("favoriteHighlight_")[1];

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.FavoriteItems.ControllerHotkey,
                setValue: value => config.FavoriteItems.ControllerHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsControllerHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.FavoriteItemsControllerHotkey.Desc")
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.TakeAllButOne")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.TakeAllButOne.IsEnabled,
                setValue: value => config.TakeAllButOne.IsEnabled = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableTakeAllButOne.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableTakeAllButOne.Desc")
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.TakeAllButOne.KeyboardHotkey,
                setValue: value => config.TakeAllButOne.KeyboardHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.TakeAllButOneKeyboardHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.TakeAllButOneKeyboardHotkey.Desc")
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.TakeAllButOne.ControllerHotkey,
                setValue: value => config.TakeAllButOne.ControllerHotkey = value,
                name: () => helper.Translation.Get("ModConfigMenu.TakeAllButOneControllerHotkey.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.TakeAllButOneControllerHotkey.Desc")
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.AutoOrganizeChest")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.AutoOrganizeChest.IsEnabled,
                setValue: value => config.AutoOrganizeChest.IsEnabled = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableAutoOrganizeChest.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableAutoOrganizeChest.Desc")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.AutoOrganizeChest.ShowInstructionsInTooltip,
                setValue: value => config.AutoOrganizeChest.ShowInstructionsInTooltip = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsShowAutoOrganizeButtonInstructions.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsShowAutoOrganizeButtonInstructions.Desc")
            );

            api.AddSectionTitle(
                mod: modManifest,
                text: () => helper.Translation.Get("ModConfigMenu.Label.Miscellaneous")
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.Miscellaneous.IsInventoryPageSideWarpEnabled,
                setValue: value => config.Miscellaneous.IsInventoryPageSideWarpEnabled = value,
                name: () => helper.Translation.Get("ModConfigMenu.IsEnableInventoryPageSideWarp.Name"),
                tooltip: () => helper.Translation.Get("ModConfigMenu.IsEnableInventoryPageSideWarp.Desc")
            );
        }
    }
}
