﻿using GenericModConfigMenu;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace ConvenientInventory.Compatibility
{
    public class ModInitializer
    {
        private readonly IManifest modManifest;
        private readonly IModHelper helper;

        public ModInitializer(IManifest modManifest, IModHelper helper)
        {
            this.modManifest = modManifest;
            this.helper = helper;
        }

        public void Initialize(IGenericModConfigMenuApi api, ModConfig config)
        {
            api.RegisterModConfig(
                mod: modManifest,
                revertToDefault: () =>
                {
                    config = new ModConfig();
                    ModEntry.Config = config;
                },
                saveToFile: () => helper.WriteConfig(config)
            );

            api.SetDefaultIngameOptinValue(modManifest, true);

            api.RegisterLabel(
                mod: modManifest,
                labelName: "Quick Stack To Nearby Chests",
                labelDesc: null
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Enable Quick stack?",
                optionDesc: "If enabled, adds a \"Quick Stack To Nearby Chests\" button to your inventory menu. Pressing this button will stack items from your inventory to any nearby chests which contain that item.",
                optionGet: () => config.IsEnableQuickStack,
                optionSet: value => config.IsEnableQuickStack = value
            );

            api.RegisterClampedOption(
                mod: modManifest,
                optionName: "Range",
                optionDesc: "How many tiles away from the player to search for nearby chests.",
                optionGet: () => config.QuickStackRange,
                optionSet: value => config.QuickStackRange = value,
                min: 0,
                max: 10,
                interval: 1
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Enable hotkey?",
                optionDesc: "If enabled, pressing either of the quick stack hotkeys specified below will quick stack your items, even outside of your inventory menu.",
                optionGet: () => config.IsEnableQuickStackHotkey,
                optionSet: value => config.IsEnableQuickStackHotkey = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Keybind (keyboard)",
                optionDesc: "Press this key to quick stack your items.",
                optionGet: () => config.QuickStackKeyboardHotkey,
                optionSet: value => config.QuickStackKeyboardHotkey = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Keybind (controller)",
                optionDesc: "Press this button to quick stack your items.",
                optionGet: () => config.QuickStackControllerHotkey,
                optionSet: value => config.QuickStackControllerHotkey = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Quick stack into buildings?",
                optionDesc: "If enabled, nearby buildings with inventories (such as Mills or Junimo Huts) will also be checked when quick stacking.",
                optionGet: () => config.IsQuickStackIntoBuildingsWithInventories,
                optionSet: value => config.IsQuickStackIntoBuildingsWithInventories = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Quick stack overflow items?",
                optionDesc: "If enabled, quick stack will place as many items as possible into chests which contain that item, rather than just a single stack.",
                optionGet: () => config.IsQuickStackOverflowItems,
                optionSet: value => config.IsQuickStackOverflowItems = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Quick stack ignore item quality?",
                optionDesc: "(Requires \"Quick stack overflow items?\" to be enabled.) If enabled, quick stack will place items into chests which contain ANY quality of that same item.",
                optionGet: () => config.IsQuickStackIgnoreItemQuality,
                optionSet: value => config.IsQuickStackIgnoreItemQuality = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Show nearby chests in tooltip?",
                optionDesc: "If enabled, hovering over the quick stack button will show a preview of all nearby chests, ordered by distance.",
                optionGet: () => config.IsQuickStackTooltipDrawNearbyChests,
                optionSet: value => config.IsQuickStackTooltipDrawNearbyChests = value
            );

            api.RegisterLabel(
                mod: modManifest,
                labelName: "Favorite Items",
                labelDesc: null
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Enable favorite items?",
                optionDesc: "If enabled, items in your inventory can be favorited. Favorited items will be ignored when stacking into chests.",
                optionGet: () => config.IsEnableFavoriteItems,
                optionSet: value => config.IsEnableFavoriteItems = value
            );

            string[] highlightStyleDescriptions =
                {
                    "0: Gold dashed",
                    "1: Clean gold dashed",
                    "2: Thick gold border",
                    "3: Textured gold inset border",
                    "4: Gold inset border",
                    "5: Dark dashed"
                };
            api.RegisterChoiceOption(
                mod: modManifest,
                optionName: "Highlight style",
                optionDesc: "Choose your preferred texture style for highlighting favorited items in your inventory.",
                optionGet: () => highlightStyleDescriptions[config.FavoriteItemsHighlightTextureChoice],
                optionSet: value =>
                    {
                        config.FavoriteItemsHighlightTextureChoice = int.Parse(value.Substring(0, 1));
                        ConvenientInventory.FavoriteItemsHighlightTexture = helper.Content.Load<Texture2D>($@"assets\favoriteHighlight_{value[0]}.png");
                    },
                choices: highlightStyleDescriptions
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Keybind (keyboard)",
                optionDesc: "Hold this key when selecting an item to favorite it.",
                optionGet: () => config.FavoriteItemsKeyboardHotkey,
                optionSet: value => config.FavoriteItemsKeyboardHotkey = value
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Keybind (controller)",
                optionDesc: "Hold this button when selecting an item to favorite it.",
                optionGet: () => config.FavoriteItemsControllerHotkey,
                optionSet: value => config.FavoriteItemsControllerHotkey = value
            );

            api.RegisterLabel(
                mod: modManifest,
                labelName: "Miscellaneous",
                labelDesc: null
            );

            api.RegisterSimpleOption(
                mod: modManifest,
                optionName: "Enable inventory cursor side warp?",
                optionDesc: "If enabled, moving your controller's cursor beyond either side of your inventory menu will warp the cursor to the opposite side.",
                optionGet: () => config.IsEnableInventoryPageSideWarp,
                optionSet: value => config.IsEnableInventoryPageSideWarp = value
            );
        }
    }
}
