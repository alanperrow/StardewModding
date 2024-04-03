using BetterSplitscreen.Compatibility;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BetterSplitscreen
{
    /// <summary>
    /// The mod entry class loaded by SMAPI.
    /// </summary>
    public partial class ModEntry : Mod
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

            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
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

            // Initialize external mod API integrations.
            var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                Initialize(api);
            }
        }

        /// <summary>
        /// Raised after the active menu is drawn to the screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRenderedActiveMenu(object sender, RenderedActiveMenuEventArgs e)
        {
            // TODO: Maybe use transpiler instead of post-render call. I don't want the player name scroll to block useful information if the screen is super small.

            if (!Config.IsModEnabled || !Config.ShowNameFeature.IsFeatureEnabled)
            {
                return;
            }

            // TODO: Check if local multiplayer. Do not display for singleplayer.

            var menu = StardewValley.Game1.activeClickableMenu;
            if (menu is null)
            {
                return;
            }
            
            if (menu is StardewValley.Menus.ShippingMenu or StardewValley.Menus.LevelUpMenu)
            {
                // TODO: Config setting of where to draw player name: Top or Bottom.
                //int posY = 30;
                int posY = StardewValley.Game1.uiViewport.Height - 70;

                StardewValley.BellsAndWhistles.SpriteText.drawStringWithScrollCenteredAt(
                    e.SpriteBatch, StardewValley.Game1.player.Name, StardewValley.Game1.uiViewport.Width / 2, posY);

                return;
            }
        }
    }
}
