using System;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ConvenientInventory.Compatibility
{
    /// <summary>
    /// Supports integration/compatibility with other mods and provides access to their API methods.
    /// </summary>
    public static class ModIntegrations
    {
        private static ICustomBackpackApi customBackpackApi;
        private static Type customBackpackFullInventoryPageType;

        public static bool IsChestsAnywhereInstalled { get; private set; }

        public static bool IsCustomBackpackFrameworkInstalled => customBackpackApi != null;

        public static bool IsWearMoreRingsInstalled { get; private set; }

        public static int CustomBackpackScrollAmount => IsCustomBackpackFrameworkInstalled ? customBackpackApi.GetScroll() : 0;


        /// <summary>
        /// Initializes our mod config through the Generic Mod Config Menu API.
        /// </summary>
        public static void InitializeApi(IGenericModConfigMenuApi api, ModConfig config, IManifest modManifest, IMonitor monitor)
        {
            // == Config Validation ==
            if (!IsChestsAnywhereInstalled && ConfigHelper.ParseQuickStackRangeFromConfig(config.QuickStack.Range) == ConfigHelper.QuickStackRange_GlobalInt)
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
                save: config.Save);

            // ===== Quick Stack To Nearby Chests =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_QuickStackToNearbyChests);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsEnabled,
                setValue: value => config.QuickStack.IsEnabled = value,
                name: I18n.ModConfigMenu_IsEnableQuickStack_Name,
                tooltip: I18n.ModConfigMenu_IsEnableQuickStack_Desc);

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => ConfigHelper.ParseQuickStackRangeFromConfig(config.QuickStack.Range),
                setValue: value => config.QuickStack.Range = ConfigHelper.FormatQuickStackRange(value),
                name: I18n.ModConfigMenu_QuickStackRange_Name,
                tooltip: I18n.ModConfigMenu_QuickStackRange_Desc,
                min: 1,
                max: IsChestsAnywhereInstalled ? ConfigHelper.QuickStackRange_GlobalInt : ConfigHelper.QuickStackRange_LocationInt,
                interval: 1,
                formatValue: ConfigHelper.FormatQuickStackRange);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.OverflowItems,
                setValue: value => config.QuickStack.OverflowItems = value,
                name: I18n.ModConfigMenu_IsQuickStackOverflowItems_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackOverflowItems_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IgnoreItemQuality,
                setValue: value => config.QuickStack.IgnoreItemQuality = value,
                name: I18n.ModConfigMenu_IsQuickStackIgnoreItemQuality_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIgnoreItemQuality_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsHotkeyEnabled,
                setValue: value => config.QuickStack.IsHotkeyEnabled = value,
                name: I18n.ModConfigMenu_IsEnableQuickStackHotkey_Name,
                tooltip: I18n.ModConfigMenu_IsEnableQuickStackHotkey_Desc);

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.QuickStack.KeyboardHotkey,
                setValue: value => config.QuickStack.KeyboardHotkey = value,
                name: I18n.ModConfigMenu_QuickStackKeyboardHotkey_Name,
                tooltip: I18n.ModConfigMenu_QuickStackKeyboardHotkey_Desc);

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.QuickStack.ControllerHotkey,
                setValue: value => config.QuickStack.ControllerHotkey = value,
                name: I18n.ModConfigMenu_QuickStackControllerHotkey_Name,
                tooltip: I18n.ModConfigMenu_QuickStackControllerHotkey_Desc);

            //api.AddSubHeader(
            //    mod: modManifest,
            //    text: I18n.ModConfigMenu_QuickStackAdditionalSettings_Label);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.AllowHotkeyInChestMenu,
                setValue: value => config.QuickStack.AllowHotkeyInChestMenu = value,
                name: I18n.ModConfigMenu_IsQuickStackAllowHotkeyInChestMenu_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackAllowHotkeyInChestMenu_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.WithFillStacksButton,
                setValue: value => config.QuickStack.WithFillStacksButton = value,
                name: I18n.ModConfigMenu_IsQuickStackWithFillStacksButton_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackWithFillStacksButton_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.VisuallyOverrideFillStacksButton,
                setValue: value => config.QuickStack.VisuallyOverrideFillStacksButton = value,
                name: I18n.ModConfigMenu_IsQuickStackVisuallyOverrideFillStacksButton_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackVisuallyOverrideFillStacksButton_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsToggleChestEnabled,
                setValue: value => config.QuickStack.IsToggleChestEnabled = value,
                name: I18n.ModConfigMenu_IsEnableQuickStackToggleChest_Name,
                tooltip: I18n.ModConfigMenu_IsEnableQuickStackToggleChest_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsPrioritizeChestEnabled,
                setValue: value => config.QuickStack.IsPrioritizeChestEnabled = value,
                name: I18n.ModConfigMenu_IsEnableQuickStackPrioritizeChest_Name,
                tooltip: I18n.ModConfigMenu_IsEnableQuickStackPrioritizeChest_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IsToggleChestButtonHidden,
                setValue: value => config.QuickStack.IsToggleChestButtonHidden = value,
                name: I18n.ModConfigMenu_IsQuickStackToggleChestButtonHidden_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackToggleChestButtonHidden_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoMills,
                setValue: value => config.QuickStack.IntoMills = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoMills_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoMills_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoJunimoHuts,
                setValue: value => config.QuickStack.IntoJunimoHuts = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoJunimoHuts_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoJunimoHuts_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoDressers,
                setValue: value => config.QuickStack.IntoDressers = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoDressers_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoDressers_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoHoppers,
                setValue: value => config.QuickStack.IntoHoppers = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoHoppers_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoHoppers_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.IntoMiniShippingBins,
                setValue: value => config.QuickStack.IntoMiniShippingBins = value,
                name: I18n.ModConfigMenu_IsQuickStackIntoMiniShippingBins_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackIntoMiniShippingBins_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.DrawChestsInButtonTooltip,
                setValue: value => config.QuickStack.DrawChestsInButtonTooltip = value,
                name: I18n.ModConfigMenu_IsQuickStackTooltipDrawNearbyChests_Name,
                tooltip: I18n.ModConfigMenu_IsQuickStackTooltipDrawNearbyChests_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.QuickStack.SuppressSoundWhenNoNearbyChests,
                setValue: value => config.QuickStack.SuppressSoundWhenNoNearbyChests = value,
                name: I18n.ModConfigMenu_IsSuppressSoundWhenNoNearbyChests_Name,
                tooltip: I18n.ModConfigMenu_IsSuppressSoundWhenNoNearbyChests_Desc);

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
                tooltip: I18n.ModConfigMenu_IsEnableQuickStackAnimation_Desc);

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.QuickStack.AnimationItemSpeedFactor,
                setValue: value => config.QuickStack.AnimationItemSpeedFactor = value,
                name: I18n.ModConfigMenu_QuickStackAnimationItemSpeed_Name,
                tooltip: I18n.ModConfigMenu_QuickStackAnimationItemSpeed_Desc,
                min: 0.5f,
                max: 3f,
                interval: 0.1f);

            api.AddNumberOption(
                mod: modManifest,
                getValue: () => config.QuickStack.AnimationStackSpeedFactor,
                setValue: value => config.QuickStack.AnimationStackSpeedFactor = value,
                name: I18n.ModConfigMenu_QuickStackAnimationStackSpeed_Name,
                tooltip: I18n.ModConfigMenu_QuickStackAnimationStackSpeed_Desc,
                min: 0.5f,
                max: 3f,
                interval: 0.1f);

            // ===== Favorite Items =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_FavoriteItems);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.FavoriteItems.IsEnabled,
                setValue: value => config.FavoriteItems.IsEnabled = value,
                name: I18n.ModConfigMenu_IsEnableFavoriteItems_Name,
                tooltip: I18n.ModConfigMenu_IsEnableFavoriteItems_Desc);

            string[] highlightStyleDescriptions =
            {
                $"0: {I18n.ModConfigMenu_FavoriteItemsHighlightTextureChoice_Desc0()}",
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
                allowedValues: highlightStyleDescriptions);

            // TODO: Visualize the highlight texture choice.
            //       Probably doesn't need to update live, but see SplitscreenImproved for reference if it does.

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.FavoriteItems.KeyboardHotkey,
                setValue: value => config.FavoriteItems.KeyboardHotkey = value,
                name: I18n.ModConfigMenu_FavoriteItemsKeyboardHotkey_Name,
                tooltip: I18n.ModConfigMenu_FavoriteItemsKeyboardHotkey_Desc);

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.FavoriteItems.ControllerHotkey,
                setValue: value => config.FavoriteItems.ControllerHotkey = value,
                name: I18n.ModConfigMenu_FavoriteItemsControllerHotkey_Name,
                tooltip: I18n.ModConfigMenu_FavoriteItemsControllerHotkey_Desc);

            // ===== Take All But One =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_TakeAllButOne);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.TakeAllButOne.IsEnabled,
                setValue: value => config.TakeAllButOne.IsEnabled = value,
                name: I18n.ModConfigMenu_IsEnableTakeAllButOne_Name,
                tooltip: I18n.ModConfigMenu_IsEnableTakeAllButOne_Desc);

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.TakeAllButOne.KeyboardHotkey,
                setValue: value => config.TakeAllButOne.KeyboardHotkey = value,
                name: I18n.ModConfigMenu_TakeAllButOneKeyboardHotkey_Name,
                tooltip: I18n.ModConfigMenu_TakeAllButOneKeyboardHotkey_Desc);

            api.AddKeybindList(
                mod: modManifest,
                getValue: () => config.TakeAllButOne.ControllerHotkey,
                setValue: value => config.TakeAllButOne.ControllerHotkey = value,
                name: I18n.ModConfigMenu_TakeAllButOneControllerHotkey_Name,
                tooltip: I18n.ModConfigMenu_TakeAllButOneControllerHotkey_Desc);

            // ===== Auto Organize Chest =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_AutoOrganizeChest);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.AutoOrganizeChest.IsEnabled,
                setValue: value => config.AutoOrganizeChest.IsEnabled = value,
                name: I18n.ModConfigMenu_IsEnableAutoOrganizeChest_Name,
                tooltip: I18n.ModConfigMenu_IsEnableAutoOrganizeChest_Desc);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.AutoOrganizeChest.ShowInstructionsInTooltip,
                setValue: value => config.AutoOrganizeChest.ShowInstructionsInTooltip = value,
                name: I18n.ModConfigMenu_IsShowAutoOrganizeButtonInstructions_Name,
                tooltip: I18n.ModConfigMenu_IsShowAutoOrganizeButtonInstructions_Desc);

            // ===== Miscellaneous =====
            api.AddSectionTitle(
                mod: modManifest,
                text: I18n.ModConfigMenu_Label_Miscellaneous);

            api.AddBoolOption(
                mod: modManifest,
                getValue: () => config.Miscellaneous.IsInventoryPageSideWarpEnabled,
                setValue: value => config.Miscellaneous.IsInventoryPageSideWarpEnabled = value,
                name: I18n.ModConfigMenu_IsEnableInventoryPageSideWarp_Name,
                tooltip: I18n.ModConfigMenu_IsEnableInventoryPageSideWarp_Desc);
        }

        /// <summary>
        /// Initializes mod integrations with the Custom Backpack Framework mod and API.
        /// </summary>
        public static void InitializeApi(ICustomBackpackApi api, IModHelper helper, IMonitor monitor)
        {
            try
            {
                IModInfo cbfModInfo = helper.ModRegistry.Get("platinummyr.CustomBackpackFramework")
                    ?? throw new InvalidOperationException("Custom Backpack Framework mod not found in mod registry.");

                // Ensure mod version is recent enough where API interface includes `GetScroll()` method.
                if (cbfModInfo.Manifest.Version.IsOlderThan("1.1.0"))
                {
                    throw new InvalidOperationException($"Custom Backpack Framework mod version {cbfModInfo.Manifest.Version} is outdated. " +
                        "Please update to version 1.1.0 or later to enable compatibility with Convenient Inventory.");
                }

                // Cache the `FullInventoryPage` type to be used for comparisons.
                IMod cbfMod = cbfModInfo.GetType().GetProperty("Mod").GetValue(cbfModInfo) as IMod;
                Type cbfModType = cbfMod?.GetType();
                customBackpackFullInventoryPageType = cbfModType?.Assembly.GetType("CustomBackpack.FullInventoryPage");
                if (customBackpackFullInventoryPageType == null)
                {
                    throw new InvalidOperationException("Unable to find type 'CustomBackpack.FullInventoryPage' in mod assembly.");
                }

                // Initialization successful.
                customBackpackApi = api;
            }
            catch (Exception ex)
            {
                monitor.Log($"Could not initialize mod integrations with Custom Backpack Framework:\n{ex.Message}", LogLevel.Warn);
            }
        }

        /// <summary>
        /// Initializes mod integrations for cases not using a mod API.
        /// </summary>
        public static void InitializeMods(IModHelper helper)
        {
            IsWearMoreRingsInstalled = helper.ModRegistry.IsLoaded("bcmpinc.WearMoreRings");
            IsChestsAnywhereInstalled = helper.ModRegistry.IsLoaded("Pathoschild.ChestsAnywhere");
        }

        /// <summary>
        /// Determines whether the given menu is an instance of Custom Backpack Framework's full inventory page.
        /// </summary>
        public static bool IsCustomBackpackFullInventoryPage(IClickableMenu menu) => menu?.GetType() == customBackpackFullInventoryPageType;
    }
}
