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

        public static void Initialize(IGenericModConfigMenuApi api, ModConfig config, IManifest modManifest, IMonitor monitor)
        {
            // == Config Validation ==
            bool isChestsAnywhereInstalled = ChestsAnywhereApi != null;
            if (!isChestsAnywhereInstalled && ConfigHelper.ParseQuickStackRangeFromConfig(config.QuickStack.Range) == ConfigHelper.QuickStackRange_GlobalInt)
            {
                // QuickStackRange: "Global" option is only supported if the Chests Anywhere mod is installed.
                // If Chests Anywhere API is not found and config value is loaded as "Global", log a warning message to SMAPI and overwrite the config value to "Location".
                monitor.Log(I18n.ModConfigMenu_QuickStackRange_ChestsAnywhereGlobalWarning(), LogLevel.Warn);
                config.QuickStack.Range = ConfigHelper.FormatQuickStackRange(ConfigHelper.QuickStackRange_LocationInt);
                config.Save();
            }

            // == Register GMCM ==
            api.Register(
                mod: modManifest,
                reset: config.Reset,
                save: config.Save
            );

            // ===== Quick Stack To Nearby Chests =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_QuickStackToNearbyChests
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsEnabled,
                setValue: value => config.QuickStack.IsEnabled = value,
                name: I18n.ModConfigMenu_IsEnableQuickStack_Name,
                tooltip: I18n.ModConfigMenu_IsEnableQuickStack_Desc
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => ConfigHelper.ParseQuickStackRangeFromConfig(config.QuickStack.Range),
                setValue: value => config.QuickStack.Range = ConfigHelper.FormatQuickStackRange(value),
                name: I18n.ModConfigMenu_QuickStackRange_Name,
                tooltip: I18n.ModConfigMenu_QuickStackRange_Desc,
                min: 1,
                max: isChestsAnywhereInstalled ? ConfigHelper.QuickStackRange_GlobalInt : ConfigHelper.QuickStackRange_LocationInt,
                interval: 1,
                formatValue: ConfigHelper.FormatQuickStackRange
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsHotkeyEnabled,
                setValue: value => config.QuickStack.IsHotkeyEnabled = value,
                name: I18n.ModConfigMenu_IsEnableQuickStackHotkey_Name,
                tooltip: I18n.ModConfigMenu_IsEnableQuickStackHotkey_Desc
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.QuickStack.KeyboardHotkey,
                setValue: value => config.QuickStack.KeyboardHotkey = value,
                name: I18n.ModConfigMenu_QuickStackKeyboardHotkey_Name,
                tooltip: I18n.ModConfigMenu_QuickStackKeyboardHotkey_Desc
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.QuickStack.ControllerHotkey,
                setValue: value => config.QuickStack.ControllerHotkey = value,
                name: I18n.ModConfigMenu_QuickStackControllerHotkey_Name,
                tooltip: I18n.ModConfigMenu_QuickStackControllerHotkey_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.OverflowItems,
                setValue: value => config.QuickStack.OverflowItems = value,
                name: I18n.ModConfigMenu_IsQuickStackOverflowItems_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackOverflowItems_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IgnoreItemQuality,
                setValue: value => config.QuickStack.IgnoreItemQuality = value,
                name: I18n.ModConfigMenu_IsQuickStackIgnoreItemQuality_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIgnoreItemQuality_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoMills,
                setValue: value => config.QuickStack.IntoMills = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoMills_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoMills_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoJunimoHuts,
                setValue: value => config.QuickStack.IntoJunimoHuts = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoJunimoHuts_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoJunimoHuts_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoDressers,
                setValue: value => config.QuickStack.IntoDressers = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoDressers_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoDressers_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoHoppers,
                setValue: value => config.QuickStack.IntoHoppers = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoHoppers_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoHoppers_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoMiniShippingBins,
                setValue: value => config.QuickStack.IntoMiniShippingBins = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoMiniShippingBins_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoMiniShippingBins_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.DrawChestsInButtonTooltip,
                setValue: value => config.QuickStack.DrawChestsInButtonTooltip = value,
                name: I18n.ModConfigMenu_IsQuickStackTooltipDrawNearbyChests_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackTooltipDrawNearbyChests_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.SuppressSoundWhenNoNearbyChests,
                setValue: value => config.QuickStack.SuppressSoundWhenNoNearbyChests = value,
                name: I18n.ModConfigMenu_IsSuppressSoundWhenNoNearbyChests_Name,
                tooltip: I18n.ModConfigMenu_IsSuppressSoundWhenNoNearbyChests_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsToggleChestEnabled,
                setValue: value => config.QuickStack.IsToggleChestEnabled = value,
                name: I18n.ModConfigMenu_IsEnableQuickStackToggleChest_Name,
                tooltip: I18n.ModConfigMenu_IsEnableQuickStackToggleChest_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsPrioritizeChestEnabled,
                setValue: value => config.QuickStack.IsPrioritizeChestEnabled = value,
                name: I18n.ModConfigMenu_IsEnableQuickStackPrioritizeChest_Name,
                tooltip: I18n.ModConfigMenu_IsEnableQuickStackPrioritizeChest_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsToggleChestButtonHidden,
                setValue: value => config.QuickStack.IsToggleChestButtonHidden = value,
                name: I18n.ModConfigMenu_IsQuickStackToggleChestButtonHidden_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackToggleChestButtonHidden_Desc
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
                name: I18n.ModConfigMenu_IsEnableQuickStackAnimation_Name,
                tooltip: I18n.ModConfigMenu_IsEnableQuickStackAnimation_Desc
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.QuickStack.AnimationItemSpeedFactor,
                setValue: value => config.QuickStack.AnimationItemSpeedFactor = value,
                name: I18n.ModConfigMenu_QuickStackAnimationItemSpeed_Name,
                tooltip: I18n.ModConfigMenu_QuickStackAnimationItemSpeed_Desc,
                min: 0.5f,
                max: 3f,
                interval: 0.1f
            );

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.QuickStack.AnimationStackSpeedFactor,
                setValue: value => config.QuickStack.AnimationStackSpeedFactor = value,
                name: I18n.ModConfigMenu_QuickStackAnimationStackSpeed_Name,
                tooltip: I18n.ModConfigMenu_QuickStackAnimationStackSpeed_Desc,
                min: 0.5f,
                max: 3f,
                interval: 0.1f
            );

            // ===== Favorite Items =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_FavoriteItems
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.FavoriteItems.IsEnabled,
                setValue: value => config.FavoriteItems.IsEnabled = value,
                name: I18n.ModConfigMenu_IsEnableFavoriteItems_Name,
                tooltip: I18n.ModConfigMenu_IsEnableFavoriteItems_Desc
            );

            string[] highlightStyleDescriptions =
            {
                $"0: {() => I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc0()}",
                $"1: {I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc1()}",
                $"2: {I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc2()}",
                $"3: {I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc3()}",
                $"4: {I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc4()}",
                $"5: {I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc5()}",
                $"6: {I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc6()}",
                $"7: {I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc7()}",
            };
            api.AddTextOption(
                mod: modManifest,
                getValue: () => highlightStyleDescriptions[config.FavoriteItems.HighlightTextureChoice],
                setValue: value =>
                {
                    config.FavoriteItems.HighlightTextureChoice = int.Parse(value[..1]);
                    CachedTextures.FavoriteItemsHighlight = Game1.content.Load<Texture2D>(CachedTextures.ModAssetPrefix + $"favoriteHighlight_{value[0]}");
                },
                name: I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Name,
                tooltip: I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc,
                allowedValues: highlightStyleDescriptions
            );

            // TODO: Visualize the highlight texture choice.
            //       Probably doesn't need to update live, but see SplitscreenImproved for reference if it does.

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.FavoriteItems.KeyboardHotkey,
                setValue: value => config.FavoriteItems.KeyboardHotkey = value,
                name: I18n.ModConfigMenu_FavoriteItemsKeyboardHotkey_Name,
                tooltip: I18n.ModConfigMenu_FavoriteItemsKeyboardHotkey_Desc
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.FavoriteItems.ControllerHotkey,
                setValue: value => config.FavoriteItems.ControllerHotkey = value,
                name: I18n.ModConfigMenu_FavoriteItemsControllerHotkey_Name,
                tooltip: I18n.ModConfigMenu_FavoriteItemsControllerHotkey_Desc
            );

            // ===== Take All But One =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_TakeAllButOne
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.TakeAllButOne.IsEnabled,
                setValue: value => config.TakeAllButOne.IsEnabled = value,
                name: I18n.ModConfigMenu_IsEnableTakeAllButOne_Name,
                tooltip: I18n.ModConfigMenu_IsEnableTakeAllButOne_Desc
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.TakeAllButOne.KeyboardHotkey,
                setValue: value => config.TakeAllButOne.KeyboardHotkey = value,
                name: I18n.ModConfigMenu_TakeAllButOneKeyboardHotkey_Name,
                tooltip: I18n.ModConfigMenu_TakeAllButOneKeyboardHotkey_Desc
            );

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.TakeAllButOne.ControllerHotkey,
                setValue: value => config.TakeAllButOne.ControllerHotkey = value,
                name: I18n.ModConfigMenu_TakeAllButOneControllerHotkey_Name,
                tooltip: I18n.ModConfigMenu_TakeAllButOneControllerHotkey_Desc
            );

            // ===== Auto Organize Chest =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_AutoOrganizeChest
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.AutoOrganizeChest.IsEnabled,
                setValue: value => config.AutoOrganizeChest.IsEnabled = value,
                name: I18n.ModConfigMenu_IsEnableAutoOrganizeChest_Name,
                tooltip: I18n.ModConfigMenu_IsEnableAutoOrganizeChest_Desc
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.AutoOrganizeChest.ShowInstructionsInTooltip,
                setValue: value => config.AutoOrganizeChest.ShowInstructionsInTooltip = value,
                name: I18n.ModConfigMenu_IsShowAutoOrganizeButtonInstructions_Name,
                tooltip: I18n.ModConfigMenu_IsShowAutoOrganizeButtonInstructions_Desc
            );

            // ===== Miscellaneous =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_Miscellaneous
            );

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.Miscellaneous.IsInventoryPageSideWarpEnabled,
                setValue: value => config.Miscellaneous.IsInventoryPageSideWarpEnabled = value,
                name: I18n.ModConfigMenu_IsEnableInventoryPageSideWarp_Name,
                tooltip: I18n.ModConfigMenu_IsEnableInventoryPageSideWarp_Desc
            );
        }
    }
}
