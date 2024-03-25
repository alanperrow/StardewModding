﻿using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace SplitscreenMusicFix
{
    /// <summary>
    /// The mod entry class loaded by SMAPI.
    /// </summary>
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
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
        }
    }
}
