using System;
using BetterSplitscreen.Layout;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BetterSplitscreen
{
    /// <summary>
    /// The mod entry class loaded by SMAPI.
    /// </summary>
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }

        public static ModConfig Config { get; private set; }

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <summary>
        /// Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).
        /// All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Harmony patches
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            // Get Generic Mod Config Menu API (if it's installed)
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                InitializeApi(api);
            }
        }

        private void InitializeApi(IGenericModConfigMenuApi api)
        {
            api.Register(
                mod: ModManifest,
                reset: () =>
                {
                    Config = new ModConfig();
                },
                save: () => Helper.WriteConfig(Config));

            api.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.IsModEnabled,
                setValue: value =>
                {
                    if (Config.IsModEnabled == value)
                    {
                        return;
                    }

                    Config.IsModEnabled = value;

                    // Refresh window to apply config change.
                    StardewValley.Game1.game1.Window_ClientSizeChanged(null, null);
                },
                name: () => "Is Mod Enabled",
                tooltip: () => "Enables/disables the Better Splitscreen mod. This option has precedence over all others.");

            api.AddSectionTitle(
                mod: ModManifest,
                text: () => "Layout");

            api.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.LayoutFeature.IsFeatureEnabled,
                setValue: value =>
                {
                    if (Config.LayoutFeature.IsFeatureEnabled == value)
                    {
                        return;
                    }

                    Config.LayoutFeature.IsFeatureEnabled = value;

                    // Refresh window to apply config change.
                    StardewValley.Game1.game1.Window_ClientSizeChanged(null, null);
                },
                name: () => "Is Layout Feature Enabled",
                tooltip: () => "Enables/disables the custom splitscreen layout feature.");

            string[] layoutPresetNames = Enum.GetNames(typeof(LayoutPreset));
            api.AddTextOption(
                mod: ModManifest,
                getValue: () => Config.LayoutFeature.PresetChoice.ToString(),
                setValue: value =>
                {
                    LayoutPreset valueEnum = Enum.Parse<LayoutPreset>(value);
                    if (Config.LayoutFeature.PresetChoice == valueEnum)
                    {
                        return;
                    }

                    Config.LayoutFeature.PresetChoice = valueEnum;

                    // TODO: Temporary solution just to get mod config working.
                    //       We should be storing each custom layout definition somewhere, rather than simply overwriting it every time.
                    //       IDEA: Include a json file to store all layout definitions. (See ModConfig comment about saving presets)
                    Config.LayoutFeature.CurrentLayout = new SplitscreenLayout(Config.LayoutFeature.PresetChoice);

                    // Refresh window to apply config change.
                    StardewValley.Game1.game1.Window_ClientSizeChanged(null, null);
                },
                name: () => "Layout Preset",
                tooltip: () => "The currently selected layout preset.\n" +
                    "'Default' = Vanilla splitscreen layout\n" +
                    "'SwapSides' = Left and right screens are swapped\n" +
                    "'Custom' = Use a custom layout (see below)",
                allowedValues: layoutPresetNames);

            // TODO: Make a nice pretty graphic with red/blue/green/yellow boxes representing each individual splitscreen position.
            api.AddComplexOption(
                mod: ModManifest,
                name: () => "Preview Layout:",
                draw: (sb, p) =>
                {
                    //sb.Draw(
                    //    StardewValley.Game1.staminaRect,
                    //    new Microsoft.Xna.Framework.Rectangle((int)p.X, (int)p.Y, 200, 200),
                    //    Microsoft.Xna.Framework.Color.Red);

                    int px = (int)p.X;
                    int py = (int)p.Y;

                    // Border
                    sb.Draw(
                        StardewValley.Game1.fadeToBlackRect,
                        new Microsoft.Xna.Framework.Rectangle(px, py, 208, 208),
                        Microsoft.Xna.Framework.Color.Black);

                    // TODO: In LayoutManager, dynamically perform the following draw logic:

                    // Player window locations
                    // TODO: Draw layout preview based on current SplitscreenLayout
                    int p1_x, p1_y, p2_x, p2_y;
                    if (Config.LayoutFeature.PresetChoice == LayoutPreset.Default)
                    {
                        p1_x = px + 4;
                        p1_y = py + 4;
                        p2_x = px + 4 + 100;
                        p2_y = py + 4;
                    }
                    else
                    {
                        p1_x = px + 4 + 100;
                        p1_y = py + 4;
                        p2_x = px + 4;
                        p2_y = py + 4;
                    }

                    sb.Draw(
                        StardewValley.Game1.fadeToBlackRect,
                        new Microsoft.Xna.Framework.Rectangle(p1_x, p1_y, 100, 200),
                        Microsoft.Xna.Framework.Color.Red);
                    sb.Draw(
                        StardewValley.Game1.fadeToBlackRect,
                        new Microsoft.Xna.Framework.Rectangle(p2_x, p2_y, 100, 200),
                        Microsoft.Xna.Framework.Color.Blue);

                    // Player indicators
                    // TODO: P1, P2, P3, P4; dynamically
                    sb.DrawString(
                        StardewValley.Game1.dialogueFont,
                        "P1",
                        new Microsoft.Xna.Framework.Vector2(p1_x + 30, p1_y + 30),
                        Microsoft.Xna.Framework.Color.Black);
                    sb.DrawString(
                        StardewValley.Game1.dialogueFont,
                        "P2",
                        new Microsoft.Xna.Framework.Vector2(p2_x + 30, p2_y + 30),
                        Microsoft.Xna.Framework.Color.Black);
                },
                tooltip: () => "Preview how the currently selected layout preset will be displayed.\n" +
                    "Blue = Player 1\n" +
                    "Red = Player 2\n" +
                    "Green = Player 3\n" +
                    "Yellow = Player 4");

            // TODO: Preview Player Count: Dropdown list {1, 2, 3, 4} for number of players to preview splitscreen layout.
            //api.AddTextOption()
        }
    }
}
