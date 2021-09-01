using StardewModdingAPI;
using HarmonyLib;
using StardewModdingAPI.Events;
using GenericModConfigMenu;

namespace FasterPathSpeed
{
    public class ModEntry : Mod
    {
        public ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            FarmerPatches.Initialize(Monitor, Config);

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.getMovementSpeed)),
                postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.GetMovementSpeed_Postfix))
            );
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Get Generic Mod Config Menu API (if it's installed)
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api is null)
                return;

            // Register mod configuration
            api.RegisterModConfig(
                mod: ModManifest,
                revertToDefault: () => Config = new ModConfig(),
                saveToFile: () => Helper.WriteConfig(Config)
            );

            // Let players configure your mod in-game (instead of just from the title screen)
            api.SetDefaultIngameOptinValue(this.ModManifest, true);

            // Add some config options
            api.RegisterSimpleOption(
                mod: this.ModManifest,
                optionName: "Example checkbox",
                optionDesc: "An optional description shown as a tooltip to the player.",
                optionGet: () => this.Config.ExampleCheckbox,
                optionSet: value => this.Config.ExampleCheckbox = value
            );
            api.RegisterSimpleOption(
                mod: this.ModManifest,
                optionName: "Example string",
                optionDesc: "...",
                optionGet: () => this.Config.ExampleString,
                optionSet: value => this.Config.ExampleString = value
            );
            api.RegisterChoiceOption(
                mod: this.ModManifest,
                optionName: "Example dropdown",
                optionDesc: "...",
                optionGet: () => this.Config.ExampleDropdown,
                optionSet: value => this.Config.ExampleDropdown = value,
                choices: new string[] { "choice A", "choice B", "choice C" }
            );
        }

            public static float GetPathSpeedBoostByFlooringType(StardewValley.TerrainFeatures.Flooring flooring)
        {
            if (Config.IsUseCustomPathSpeedBuffValues)
            {
                switch (flooring.whichFloor.Value)
                {
                    case StardewValley.TerrainFeatures.Flooring.wood:
                        return Config.CustomPathSpeedBuffValues.Wood;
                    case StardewValley.TerrainFeatures.Flooring.stone:
                        return Config.CustomPathSpeedBuffValues.Stone;
                    case StardewValley.TerrainFeatures.Flooring.ghost:
                        return Config.CustomPathSpeedBuffValues.Ghost;
                    case StardewValley.TerrainFeatures.Flooring.iceTile:
                        return Config.CustomPathSpeedBuffValues.IceTile;
                    case StardewValley.TerrainFeatures.Flooring.straw:
                        return Config.CustomPathSpeedBuffValues.Straw;
                    case StardewValley.TerrainFeatures.Flooring.gravel:
                        return Config.CustomPathSpeedBuffValues.Gravel;
                    case StardewValley.TerrainFeatures.Flooring.boardwalk:
                        return Config.CustomPathSpeedBuffValues.Boardwalk;
                    case StardewValley.TerrainFeatures.Flooring.colored_cobblestone:
                        return Config.CustomPathSpeedBuffValues.ColoredCobblestone;
                    case StardewValley.TerrainFeatures.Flooring.cobblestone:
                        return Config.CustomPathSpeedBuffValues.Cobblestone;
                    case StardewValley.TerrainFeatures.Flooring.steppingStone:
                        return Config.CustomPathSpeedBuffValues.SteppingStone;
                    case StardewValley.TerrainFeatures.Flooring.brick:
                        return Config.CustomPathSpeedBuffValues.Brick;
                    case StardewValley.TerrainFeatures.Flooring.plankFlooring:
                        return Config.CustomPathSpeedBuffValues.PlankFlooring;
                    case StardewValley.TerrainFeatures.Flooring.townFlooring:
                        return Config.CustomPathSpeedBuffValues.TownFlooring;
                }
            }

            return Config.DefaultPathSpeedBuff;
        }
    }
}
