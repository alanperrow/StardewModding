using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;

namespace ConvenientInventory.AutoOrganize
{
    /// <summary>
    /// Handles all logic related to the auto organize chest feature.
    /// </summary>
    public static class AutoOrganizeLogic
    {
        private static string AutoOrganizeModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/AutoOrganize";

        /// <summary>
        /// Iterates through all chests in each game location and removes any auto organize mod data.
        /// </summary>
        public static bool CleanupAutoOrganizeModDataByLocation(GameLocation gameLocation)
        {
            try
            {
                foreach (Chest chest in gameLocation.Objects.Values.OfType<Chest>())
                {
                    chest.modData.Remove(AutoOrganizeModDataKey);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the chest from the provided <paramref name="itemGrabMenuContext"/>, if any.
        /// </summary>
        /// <param name="itemGrabMenuContext">The <see cref="ItemGrabMenu.context"/> object.</param>
        /// <returns>The chest, or null if chest could not be obtained from context.</returns>
        public static Chest GetChestFromItemGrabMenuContext(object itemGrabMenuContext)
        {
            return itemGrabMenuContext as Chest
              ?? (itemGrabMenuContext as JunimoHut)?.GetOutputChest();
        }

        /// <summary>
        /// Determines if this chest's mod data contains auto organize data, and if so, organizes the chest's inventory.
        /// </summary>
        public static void TryOrganizeChest(Chest chest)
        {
            if (!chest.modData.ContainsKey(AutoOrganizeModDataKey))
            {
                return;
            }

            if (ConfigHelper.GetQuickStackRangeType(ModEntry.Config.QuickStackRange) == QuickStack.QuickStackRangeType.Global
                && chest.Location != Game1.currentLocation)
            {
                // NetMutex.RequestLock() seems to be invoking neither `acquired` nor `failed` Action when chest is outside of the current location.
                // The request only seems to be evaluated upon entering that location, which will then invoke the appropriate Action.
                // To avoid this, we perform Auto Organize without mutex lock in this case (unsafe; note that Chests Anywhere avoids mutex locks, presumably for the same reason).

                // DEBUG //////////////////////////////////////////////////////////////////////
                ModEntry.Instance.Monitor.Log($"TryOrganizeChest without mutex lock; chest.Location={chest.Location}.", LogLevel.Info);
                ////////////////////////////////////////////////////////////////////////
                ///
                IInventory chestInventory = chest.GetItemsForPlayer();
                ItemGrabMenu.organizeItemsInList(chestInventory);
            }
            else
            {
                // TODO: Move mutex vars into else block
                NetMutex chestMutex = chest.GetMutex();
                bool chestMutexWasAlreadyLocked = chestMutex.IsLocked() && chestMutex.IsLockHeld();

                // DEBUG //////////////////////////////////////////////////////////////////////
                ModEntry.Instance.Monitor.Log($"TryOrganizeChest about to request lock...", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"\tchestMutex.IsLocked() = {chestMutex.IsLocked()}", LogLevel.Info);
                ModEntry.Instance.Monitor.Log($"\tchestMutex.IsLockHeld() = {chestMutex.IsLockHeld()}", LogLevel.Info);
                ////////////////////////////////////////////////////////////////////////

                // Perform organize with a mutex lock, for item safety.
                chestMutex.RequestLock(
                    acquired: () =>
                    {
                        // DEBUG //////////////////////////////////////////////////////////////////////
                        ModEntry.Instance.Monitor.Log($"TryOrganizeChest lock acquired.", LogLevel.Info);
                        ////////////////////////////////////////////////////////////////////////

                        IInventory chestInventory = chest.GetItemsForPlayer();
                        ItemGrabMenu.organizeItemsInList(chestInventory);

                        // DEBUG //////////////////////////////////////////////////////////////////////
                        ModEntry.Instance.Monitor.Log($"TryOrganizeChest after requesting lock...", LogLevel.Info);
                        ModEntry.Instance.Monitor.Log($"\tchestMutex.IsLocked() = {chestMutex.IsLocked()}", LogLevel.Info);
                        ModEntry.Instance.Monitor.Log($"\tchestMutex.IsLockHeld() = {chestMutex.IsLockHeld()}", LogLevel.Info);
                        ////////////////////////////////////////////////////////////////////////

                        if (!chestMutexWasAlreadyLocked && chestMutex.IsLocked() && chestMutex.IsLockHeld())
                        {
                            // Chest mutex was not locked before this method was invoked, so release the lock we acquired.
                            chestMutex.ReleaseLock();

                            // DEBUG //////////////////////////////////////////////////////////////////////
                            ModEntry.Instance.Monitor.Log($"TryOrganizeChest lock released.", LogLevel.Info);
                            ////////////////////////////////////////////////////////////////////////
                        }
                        // DEBUG //////////////////////////////////////////////////////////////////////
                        else
                        {
                            ModEntry.Instance.Monitor.Log($"TryOrganizeChest lock was not released, since lock was previously being held.", LogLevel.Info);
                        }
                        ////////////////////////////////////////////////////////////////////////
                    },
                    failed: () =>
                    {
                        // Log for debugging purposes; this shouldn't happen.
                        string itemNames = string.Empty;
                        foreach (Item item in chest.GetItemsForPlayer())
                        {
                            itemNames += $"'{item.Name}' x {item.Stack}, ";
                        }

                        ModEntry.Instance.Monitor.Log(
                            $"Failed to acquire chest mutex lock before auto organizing. Chest items: {itemNames}.",
                            //LogLevel.Debug);
                            LogLevel.Info);
                    });
            }

            // DEBUG //////////////////////////////////////////////////////////////////////
            ModEntry.Instance.Monitor.Log($"TryOrganizeChest method complete.", LogLevel.Info);
            ModEntry.Instance.Monitor.Log($"\tchestMutex.IsLocked() = {chestMutex.IsLocked()}", LogLevel.Info);
            ModEntry.Instance.Monitor.Log($"\tchestMutex.IsLockHeld() = {chestMutex.IsLockHeld()}", LogLevel.Info);
            ////////////////////////////////////////////////////////////////////////
        }

        /// <summary>
        /// Determines if this chest's mod data contains auto organize data, and if so, calls <see cref="OrganizeAndCreateNewItemGrabMenu"/>.
        /// </summary>
        public static void TryOrganizeChestInMenu(ItemGrabMenu chestMenu, Chest chest)
        {
            if (!chest.modData.ContainsKey(AutoOrganizeModDataKey))
            {
                return;
            }

            OrganizeAndCreateNewItemGrabMenu(chestMenu);
        }

        /// <summary>
        /// Determines if this chest's mod data contains auto organize data, and if so, applies the auto organize icon texture
        /// to the chest menu's organize button.
        /// </summary>
        public static void TrySetupAutoOrganizeButton(ItemGrabMenu chestMenu, Chest chest)
        {
            if (chest.modData.ContainsKey(AutoOrganizeModDataKey))
            {
                UpdateToAutoOrganizeButton(chestMenu.organizeButton);
            }
        }

        /// <summary>
        /// Toggles the auto organize state of the chest in its mod data, and updates the chest menu's organize button approppriately.
        /// </summary>
        public static void ToggleChestAutoOrganize(ItemGrabMenu itemGrabMenu, Chest chest)
        {
            if (chest.modData.ContainsKey(AutoOrganizeModDataKey))
            {
                // Reset to default
                chest.modData.Remove(AutoOrganizeModDataKey);
                ResetToDefaultOrganizeButton(itemGrabMenu.organizeButton);
                Game1.playSound("dialogueCharacter");
            }
            else
            {
                // Update to Auto Organize
                chest.modData[AutoOrganizeModDataKey] = "1";
                OrganizeAndCreateNewItemGrabMenu(itemGrabMenu);
                Game1.playSound("Ship");
                Game1.playSound("smallSelect");

                // No need to update organize button here because we postfix ItemGrabMenu constructor with TrySetupAutoOrganizeButton.
            }

            Game1.playSound("openBox");
        }

        /// <summary>
        /// Determines if this chest's mod data contains auto organize data, and if so, updates the chest menu's auto organize button's hover text
        /// to reference the appropriate hotkey based on the current gamepad mode.
        /// </summary>
        public static void TryUpdateAutoOrganizeButtonHoverTextByGamePadMode(ClickableTextureComponent organizeButton, Chest chest)
        {
            if (chest.modData.ContainsKey(AutoOrganizeModDataKey))
            {
                UpdateHoverTextByGamePadMode(organizeButton);
            }
        }

        private static void UpdateToAutoOrganizeButton(ClickableTextureComponent organizeButton)
        {
            organizeButton.texture = CachedTextures.AutoOrganizeButtonIcon;
            organizeButton.sourceRect = CachedTextures.AutoOrganizeButtonIcon.Bounds;
            UpdateHoverTextByGamePadMode(organizeButton);
        }

        private static void ResetToDefaultOrganizeButton(ClickableTextureComponent organizeButton)
        {
            // Default organize button values taken from base game ItemGrabMenu constructor.
            organizeButton.texture = Game1.mouseCursors;
            organizeButton.sourceRect = new Rectangle(162, 440, 16, 16);
            organizeButton.hoverText = Game1.content.LoadString("Strings\\UI:ItemGrab_Organize");
        }

        private static void UpdateHoverTextByGamePadMode(ClickableTextureComponent organizeButton)
        {
            organizeButton.hoverText = ModEntry.Instance.Helper.Translation.Get("AutoOrganizeButton.hoverText");
            if (ModEntry.Config.IsShowAutoOrganizeButtonInstructions)
            {
                organizeButton.hoverText += '\n';
                organizeButton.hoverText += Game1.options.gamepadControls
                    ? ModEntry.Instance.Helper.Translation.Get("AutoOrganizeButton.hoverText.disable-gamepad")
                    : ModEntry.Instance.Helper.Translation.Get("AutoOrganizeButton.hoverText.disable");
            }
        }

        /// <summary>
        /// Organizes the provided <paramref name="chestMenu"/>, and creates a new <see cref="ItemGrabMenu"/> using the organized inventory
        /// which is then assigned to <see cref="Game1.activeClickableMenu"/>.
        /// </summary>
        private static void OrganizeAndCreateNewItemGrabMenu(ItemGrabMenu chestMenu)
        {
            // Taken from base game ItemGrabMenu.receiveLeftClick (when checking if organizeButton was clicked).
            ClickableComponent lastSnappedComponent = chestMenu.currentlySnappedComponent;
            ItemGrabMenu.organizeItemsInList(chestMenu.ItemsToGrabMenu.actualInventory);
            Item heldItem = chestMenu.heldItem;
            chestMenu.heldItem = null;

            ItemGrabMenu newMenu = new ItemGrabMenu(
                chestMenu.ItemsToGrabMenu.actualInventory,
                reverseGrab: false,
                showReceivingMenu: true,
                InventoryMenu.highlightAllItems,
                chestMenu.behaviorFunction,
                null,
                chestMenu.behaviorOnItemGrab,
                snapToBottom: false,
                canBeExitedWithKey: true,
                playRightClickSound: true,
                allowRightClick: true,
                showOrganizeButton: true,
                chestMenu.source,
                chestMenu.sourceItem,
                chestMenu.whichSpecialButton,
                chestMenu.context,
                chestMenu.HeldItemExitBehavior,
                chestMenu.AllowExitWithHeldItem)
                .setEssential(chestMenu.essential);

            if (lastSnappedComponent != null)
            {
                newMenu.setCurrentlySnappedComponentTo(lastSnappedComponent.myID);
                if (Game1.options.SnappyMenus)
                {
                    chestMenu.snapCursorToCurrentSnappedComponent();
                }
            }

            newMenu.heldItem = heldItem;
            Game1.activeClickableMenu = newMenu;
        }
    }
}
