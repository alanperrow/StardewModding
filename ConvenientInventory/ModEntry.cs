using System;
using ConvenientInventory.AutoOrganize;
using ConvenientInventory.Compatibility;
using ConvenientInventory.Patches;
using ConvenientInventory.QuickStack;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ConvenientInventory
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod
    {
        public static ModEntry Instance { get; private set; }

        public static ModConfig Config { get; set; }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();
            Config.QuickStackRange = ConfigHelper.ValidateAndConstrainQuickStackRange(Config.QuickStackRange);

            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.Content.AssetReady += OnAssetReady;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;

            helper.ConsoleCommands.Add("player_fixinventory",
                "Resizes the player's inventory to its correct maximum size, dropping any extra items contained in inventory." +
                "\n(Some mods directly modify the player's inventory size, causing compatibility issues and/or leaving extra null items when uninstalled; " +
                "this command should fix these issues.)" +
                "\n\nUsage: player_fixinventory",
                FixInventory);

            helper.ConsoleCommands.Add("convinv_clearmoddata",
                "Clears all mod data set by Convenient Inventory for the currently loaded save; no other mod data is removed. " +
                "Changes to the save file will take effect the next time the game is saved." +
                "\n(This command is intended for players who want to remove any Convenient Inventory mod data from their save file for a complete uninstallation.)" +
                "\n\nUsage: convinv_cleanup_autoorganize",
                ClearModDataForCurrentlyLoadedSave);
        }

        /// <summary>Raised when an asset is being requested from the content pipeline.</summary>
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e) => CachedTextures.OnAssetRequested(e);

        /// <summary>Raised after an asset is loaded by the content pipeline, after all mod edits specified via "AssetRequested" have been applied.</summary>
        private void OnAssetReady(object sender, AssetReadyEventArgs e) => CachedTextures.OnAssetReady(e);

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).
        /// All mods are loaded and initialised at this point, so this is a good time to set up mod integrations.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var harmony = new Harmony(ModManifest.UniqueID);

            // Manually patch InventoryPage constructor, otherwise Harmony cannot find method.
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Menus.InventoryPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(InventoryPageConstructorPatch), nameof(InventoryPageConstructorPatch.Postfix))
            );

            harmony.PatchAll();

            // Initialize mod API integrations
            var apiCA = Helper.ModRegistry.GetApi<IChestsAnywhereApi>("Pathoschild.ChestsAnywhere");
            if (apiCA != null)
            {
                ApiHelper.Initialize(apiCA);
            }

            var apiGMCM = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (apiGMCM != null)
            {
                ApiHelper.Initialize(apiGMCM, Config, ModManifest, Helper, Monitor);
            }

            ApiHelper.IsWearMoreRingsInstalled = Helper.ModRegistry.IsLoaded("bcmpinc.WearMoreRings");

            // Load cached textures
            CachedTextures.LoadGameAssets();
            CachedTextures.LoadModAssets(Config);
        }

        /// <summary>Raised after the player loads a save slot and the world is initialized.</summary>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Config.IsEnableFavoriteItems)
            {
                ConvenientInventory.LoadFavoriteItemSlots();
            }
        }

        /// <summary>Raised before the game begins writing data to the save file (except the initial save creation).</summary>
        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Config.IsEnableFavoriteItems)
            {
                ConvenientInventory.SaveFavoriteItemSlots();
            }

            StardewValley.Utility.ForEachLocation(loc => QuickStackChestAnimation.CleanupChestAnimationModDataByLocation(loc));
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Handle quick stack hotkey being pressed.
            if (Config.IsEnableQuickStackHotkey
                && Context.IsWorldReady
                && StardewValley.Game1.CurrentEvent is null
                && (Config.QuickStackKeyboardHotkey.JustPressed() || Config.QuickStackControllerHotkey.JustPressed()))
            {
                ConvenientInventory.OnQuickStackHotkeyPressed();
            }
        }

        /// <summary>Raised after the player presses or releases any buttons on the keyboard, controller, or mouse.</summary>
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            // Handle favorite items hotkey being toggled.
            if (Config.IsEnableFavoriteItems)
            {
                bool isHotkeyDown = Config.FavoriteItemsKeyboardHotkey.IsDown() || Config.FavoriteItemsControllerHotkey.IsDown();
                if (!ConvenientInventory.IsFavoriteItemsHotkeyDown && Context.IsWorldReady && isHotkeyDown)
                {
                    ConvenientInventory.IsFavoriteItemsHotkeyDown = true;
                }
                else if (ConvenientInventory.IsFavoriteItemsHotkeyDown && !isHotkeyDown)
                {
                    ConvenientInventory.IsFavoriteItemsHotkeyDown = false;
                }
            }
        }

        /// <summary>
        /// Resizes player's inventory to player.MaxItems, dropping any extra items contained in inventory.
        /// </summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void FixInventory(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Please load a save before using this command.", LogLevel.Info);
                return;
            }

            var who = StardewValley.Game1.player;
            var items = who.Items;

            if (items.Count == who.MaxItems)
            {
                Monitor.Log($"Inventory size is already correct, no fix needed. (Inventory size: {items.Count}, Max items: {who.MaxItems})", LogLevel.Info);
                return;
            }

            Monitor.Log($"Resizing inventory from {items.Count} => {who.MaxItems}...", LogLevel.Info);

            while (items.Count > who.MaxItems)
            {
                int index = items.Count - 1;

                if (items[index] != null)
                {
                    StardewValley.Game1.playSound("throwDownITem");

                    StardewValley.Game1.createItemDebris(items[index], who.getStandingPosition(), who.FacingDirection)
                        .DroppedByPlayerID.Value = who.UniqueMultiplayerID;

                    Monitor.Log($"Found non-null item: '{items[index].Name}' (x {items[index].Stack}) at index: {index} when resizing inventory."
                        + " The item was manually dropped; this may have resulted in unexpected behavior.",
                        LogLevel.Warn);
                }

                // Remove the last item of the list
                items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clears all mod data set by Convenient Inventory for the currently loaded save.
        /// </summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ClearModDataForCurrentlyLoadedSave(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Please load a save before using this command.", LogLevel.Info);
                return;
            }

            StardewValley.Utility.ForEachLocation(loc =>
            {
                bool result = true;
                result &= AutoOrganizeLogic.CleanupAutoOrganizeModDataByLocation(loc);

                // This shouldn't be necessary as we already do this before saving, but just in case...
                result &= QuickStackChestAnimation.CleanupChestAnimationModDataByLocation(loc);

                return result;
            });
        }
    }
}
