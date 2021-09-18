using GenericModConfigMenu;
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
			ConvenientInventory.QuickStackButtonIcon = helper.Content.Load<Texture2D>(@"Assets\\icon.png");

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

				api.RegisterClampedOption(
					mod: ModManifest,
					optionName: "Range",
					optionDesc: "How many tiles away from the player to search for nearby chests.",
					optionGet: () => Config.Range,
					optionSet: value => Config.Range = value,
					min: 0,
					max: 10
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
			}

			
		}
	}
}
