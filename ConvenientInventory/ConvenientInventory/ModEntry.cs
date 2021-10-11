﻿using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using ConvenientInventory.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace ConvenientInventory
{
	/// <summary>The mod entry class loaded by SMAPI.</summary>
	internal class ModEntry : Mod
	{
		public static ModEntry Context { get; private set; }

		public static ModConfig Config { get; private set; }

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Context = this;
			Config = helper.ReadConfig<ModConfig>();

			ConvenientInventory.QuickStackButtonIcon = helper.Content.Load<Texture2D>(@"Assets\icon.png");
			ConvenientInventory.FavoriteItemsCursor = helper.Content.Load<Texture2D>(@"Assets\favoriteCursor.png");
			ConvenientInventory.FavoriteItemsHighlight = helper.Content.Load<Texture2D>(@"Assets\favoriteHighlight.png");

			helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
		}

		/// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves). All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			var harmony = new Harmony(this.ModManifest.UniqueID);

			// Manually patch InventoryPage constructor, otherwise Harmony cannot find method.
			harmony.Patch(
				original: AccessTools.Constructor(typeof(StardewValley.Menus.InventoryPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
				postfix: new HarmonyMethod(typeof(InventoryPageConstructorPatch), nameof(InventoryPageConstructorPatch.Postfix))
			);

			harmony.PatchAll();

			// Get Generic Mod Config Menu API (if it's installed)
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api != null)
            {
				api.RegisterModConfig(
					mod: ModManifest,
					revertToDefault: () => Config = new ModConfig(),
					saveToFile: () => Helper.WriteConfig(Config)
				);

				api.SetDefaultIngameOptinValue(ModManifest, true);

				api.RegisterLabel(
					mod: ModManifest,
					labelName: "Quick Stack To Nearby Chests",
					labelDesc: null
				);

				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Enable Quick stack?",
					optionDesc: "If enabled, adds a \"Quick Stack To Nearby Chests\" button to your inventory menu. Pressing this button will stack items from your inventory to any nearby chests which contain that item.",
					optionGet: () => Config.IsEnableQuickStack,
					optionSet: value => Config.IsEnableQuickStack = value
				);
				api.RegisterClampedOption(
					mod: ModManifest,
					optionName: "Range",
					optionDesc: "How many tiles away from the player to search for nearby chests.",
					optionGet: () => Config.QuickStackRange,
					optionSet: value => Config.QuickStackRange = value,
					min: 0,
					max: 10,
					interval: 1
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Quick stack into buildings?",
					optionDesc: "If enabled, nearby buildings with inventories (such as Mills or Junimo Huts) will also be checked when quick stacking.",
					optionGet: () => Config.IsQuickStackIntoBuildingsWithInventories,
					optionSet: value => Config.IsQuickStackIntoBuildingsWithInventories = value
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Quick stack overflow items?",
					optionDesc: "If enabled, quick stack will place as many items as possible into chests which contain that item, rather than just a single stack.",
					optionGet: () => Config.IsQuickStackOverflowItems,
					optionSet: value => Config.IsQuickStackOverflowItems = value
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Show nearby chests in tooltip?",
					optionDesc: "If enabled, hovering over the quick stack button will show a preview of all nearby chests, ordered by distance.",
					optionGet: () => Config.IsQuickStackTooltipDrawNearbyChests,
					optionSet: value => Config.IsQuickStackTooltipDrawNearbyChests = value
				);

				api.RegisterLabel(
					mod: ModManifest,
					labelName: "Favorite Items",
					labelDesc: null
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Enable favorite items?",   // TODO: Will favorited items ignore Auto Stash to Chest? Or just quick stack?
					optionDesc: "If enabled, items in your inventory can be \"favorited\". Favorited items will be ignored when stacking into chests.",
					optionGet: () => Config.IsEnableFavoriteItems,
					optionSet: value => Config.IsEnableFavoriteItems = value
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Favorite keybind (keyboard)",
					optionDesc: "Hold this key when selecting an item to favorite it.",
					optionGet: () => Config.FavoriteItemsKeyboardHotkey,
					optionSet: value => Config.FavoriteItemsKeyboardHotkey = value
				);
				api.RegisterSimpleOption(
					mod: ModManifest,
					optionName: "Favorite keybind (controller)",
					optionDesc: "Press this button when hovering over an item to favorite it.",
					optionGet: () => Config.FavoriteItemsControllerHotkey,
					optionSet: value => Config.FavoriteItemsControllerHotkey = value
				);
			}
		}
	}
}
