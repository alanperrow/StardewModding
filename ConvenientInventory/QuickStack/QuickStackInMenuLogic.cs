using System.Collections.Generic;
using ConvenientInventory.AutoOrganize;
using ConvenientInventory.TypedChests;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientInventory.QuickStack
{
    /// <summary>
    /// Provides methods similar to <see cref="QuickStackLogic"/> but tweaked to support quick stacking
    /// while in an <see cref="ItemGrabMenu"/> for a chest.
    /// </summary>
    public static class QuickStackInMenuLogic
    {
        public static bool IsStackingToChestInMenu { get; private set; }

        public static void OnConstructedItemGrabMenu(ItemGrabMenu itemGrabMenu)
        {
            if (!ModEntry.Config.QuickStack.IsEnabled
                || !ModEntry.Config.QuickStack.WithFillStacksButton
                || !HasValidContext(itemGrabMenu)
                || itemGrabMenu.fillStacksButton == null)
            {
                return;
            }

            itemGrabMenu.fillStacksButton.hoverText = I18n.FillStacksQuickStackButton_HoverText();

            if (ModEntry.Config.QuickStack.VisuallyOverrideFillStacksButton)
            {
                itemGrabMenu.fillStacksButton.texture = CachedTextures.FillStacksQuickStackButtonIcon;
                itemGrabMenu.fillStacksButton.sourceRect = CachedTextures.FillStacksQuickStackButtonIcon.Bounds;
            }
        }

        /// <summary>
        /// Determines whether the given <see cref="ItemGrabMenu.context"/> is valid for supporting quick stack.
        /// </summary>
        public static bool HasValidContext(ItemGrabMenu itemGrabMenu) => itemGrabMenu.context is Chest or JunimoHut;

        public static bool StackToChestInMenu(ItemGrabMenu itemGrabMenu, bool playSound)
        {
            try
            {
                IsStackingToChestInMenu = true;
                return StackToChestInMenuCore(itemGrabMenu, playSound);
            }
            finally
            {
                IsStackingToChestInMenu = false;
            }
        }

        private static bool StackToChestInMenuCore(ItemGrabMenu itemGrabMenu, bool playSound)
        {
            Chest chest = GetChestFromContext(itemGrabMenu);
            if (chest is null)
            {
                if (playSound)
                {
                    Game1.playSound("cancel");
                }

                ModEntry.Instance.Monitor.Log(
                    $"Cannot quick stack into ItemGrabMenu context of type '{itemGrabMenu.context?.GetType().Name ?? "null"}'.",
                    LogLevel.Debug);

                return false;
            }

            // Wrap this chest in a TypedChest.
            GameLocation chestLocation = chest.Location;
            ChestType chestType;
            if (itemGrabMenu.context is JunimoHut)
            {
                chestType = ChestType.JunimoHut;
            }
            else
            {
                chestType = TypedChest.DetermineChestType(chest);
                if (chestType is ChestType.Package or ChestType.Dungeon)
                {
                    // Do not consider new farmer packages or dungeon chests for quick stack.
                    if (playSound)
                    {
                        Game1.playSound("cancel");
                    }

                    return false;
                }
                else if (chestType == ChestType.Normal)
                {
                    // Double check for fridge chest before accepting default Normal result.
                    if (Game1.currentLocation is FarmHouse farmHouse && chest == farmHouse.GetFridge())
                    {
                        chestLocation ??= farmHouse;
                        chestType = ChestType.Fridge;
                    }
                    else if (Game1.currentLocation is IslandFarmHouse islandFarmHouse && chest == islandFarmHouse.GetFridge())
                    {
                        chestLocation ??= islandFarmHouse;
                        chestType = ChestType.IslandFridge;
                    }
                }
            }

            TypedChest typedChest = new(chest, chestType, chestLocation ?? Game1.currentLocation, null);

            bool movedAtLeastOneTotal = false;
            Farmer who = Game1.player;
            Inventory playerInventory = who.Items;

            QuickStackAnimation quickStackAnimation = null;
            if (ModEntry.Config.QuickStack.IsAnimationEnabled)
            {
                quickStackAnimation = new(who);
            }

            QuickStackSummary quickStackSummary = new();
            HashSet<Item> overflowItems = new();

            // Fill chest stacks with player inventory items
            IInventory chestItems = chest.GetItemsForPlayer(who.UniqueMultiplayerID);
            foreach (Item chestItem in chestItems)
            {
                if (chestItem is null)
                {
                    continue;
                }

                foreach (Item playerItem in playerInventory)
                {
                    if (playerItem is null)
                    {
                        continue;
                    }

                    int itemIndex = playerInventory.IndexOf(playerItem);
                    if (ModEntry.Config.FavoriteItems.IsEnabled && ConvenientInventory.FavoriteItemSlots[itemIndex])
                    {
                        // Skip favorited items
                        continue;
                    }

                    if (!playerItem.canStackWith(chestItem))
                    {
                        if (QuickStackLogic.ShouldOverflowItem(playerItem, chestItem))
                        {
                            overflowItems.Add(playerItem);
                        }

                        continue;
                    }

                    int beforeStack = playerItem.Stack;
                    playerItem.Stack = chestItem.addToStack(playerItem);
                    bool movedAtLeastOne = beforeStack != playerItem.Stack;

                    movedAtLeastOneTotal |= movedAtLeastOne;

                    if (movedAtLeastOne)
                    {
                        itemGrabMenu.ItemsToGrabMenu.ShakeItem(chestItem);

                        ClickableComponent inventoryComponent = itemGrabMenu.inventory.inventory[itemIndex];
                        itemGrabMenu._transferredItemSprites.Add(
                            new ItemGrabMenu.TransferredItemSprite(playerItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y));

                        if (playerItem.Stack == 0)
                        {
                            // Remove player item from inventory (and overflow list, if we are tracking it).
                            who.removeItemFromInventory(playerItem);
                            overflowItems.Remove(playerItem);
                        }

                        quickStackAnimation?.AddToAnimation(typedChest, playerItem);
                        quickStackSummary.AddToSummary(typedChest, playerItem.Name, playerItem.Stack, beforeStack);
                    }

                    if (chestItem.Stack == chestItem.maximumStackSize() && playerItem.Stack != 0)
                    {
                        itemGrabMenu.inventory.ShakeItem(playerItem);
                        if (ModEntry.Config.QuickStack.OverflowItems)
                        {
                            overflowItems.Add(playerItem);
                        }
                    }
                }
            }

            // Add overflow stacks to chest when applicable
            if (ModEntry.Config.QuickStack.OverflowItems && chestItems.Count < chest.GetActualCapacity())
            {
                foreach (Item overflowItem in overflowItems)
                {
                    if (overflowItem is null || overflowItem.Stack == 0)
                    {
                        continue;
                    }

                    int itemIndex = playerInventory.IndexOf(overflowItem);
                    if (itemIndex == -1 || (ModEntry.Config.FavoriteItems.IsEnabled && ConvenientInventory.FavoriteItemSlots[itemIndex]))
                    {
                        // Skip favorited items
                        continue;
                    }

                    int beforeStack = overflowItem.Stack;
                    Item leftoverItem = null;
                    try
                    {
                        (chest.GetItemsForPlayer() as Inventory).OnSlotChanged += ChestSlotChanged;
                        leftoverItem = chest.addItem(overflowItem);
                    }
                    finally
                    {
                        (chest.GetItemsForPlayer() as Inventory).OnSlotChanged -= ChestSlotChanged;
                    }
                    bool movedAtLeastOne = leftoverItem is null || beforeStack != leftoverItem.Stack;

                    movedAtLeastOneTotal |= movedAtLeastOne;

                    if (movedAtLeastOne)
                    {
                        ClickableComponent inventoryComponent = itemGrabMenu.inventory.inventory[itemIndex];
                        itemGrabMenu._transferredItemSprites.Add(
                                new ItemGrabMenu.TransferredItemSprite(overflowItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y));

                        quickStackAnimation?.AddToAnimation(typedChest, overflowItem);
                        quickStackSummary.AddToSummary(typedChest, overflowItem.Name, leftoverItem?.Stack ?? 0, beforeStack);
                    }

                    if (leftoverItem is null)
                    {
                        who.removeItemFromInventory(overflowItem);
                    }
                    else
                    {
                        itemGrabMenu.inventory.ShakeItem(overflowItem);
                    }
                }
            }

            if (playSound)
            {
                Game1.playSound(movedAtLeastOneTotal ? "Ship" : "cancel");
            }

            quickStackAnimation?.Complete();
            if (movedAtLeastOneTotal)
            {
                if (ModEntry.Config.AutoOrganizeChest.IsEnabled)
                {
                    // Manually trigger auto organize now that quick stack has completed and added at least one item.
                    AutoOrganizeLogic.TryOrganizeChestOnFillOutStacks(itemGrabMenu, chest);
                }

                ModEntry.Instance.Monitor.Log(quickStackSummary.GetSummaryMessage(), LogLevel.Trace);
            }

            return movedAtLeastOneTotal;

            // Local function to shake item if we add it to the chest.
            void ChestSlotChanged(Inventory inventory, int index, Item before, Item after)
            {
                if (before == null && after != null)
                {
                    itemGrabMenu.ItemsToGrabMenu.ShakeItem(index);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Chest"/> from the given <see cref="ItemGrabMenu.context"/>, if any.
        /// </summary>
        private static Chest GetChestFromContext(ItemGrabMenu itemGrabMenu) => itemGrabMenu.context switch
        {
            Chest c => c,
            JunimoHut jh => jh.GetOutputChest(),
            _ => null,
        };
    }
}
