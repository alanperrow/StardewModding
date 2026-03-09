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

        public static bool StackToChestInMenu(ItemGrabMenu itemGrabMenu)
        {
            try
            {
                IsStackingToChestInMenu = true;
                return StackToChestInMenuCore(itemGrabMenu);
            }
            finally
            {
                IsStackingToChestInMenu = false;
            }
        }

        private static bool StackToChestInMenuCore(ItemGrabMenu itemGrabMenu)
        {
            Chest chest = GetChestFromContext(itemGrabMenu);
            if (chest is null)
            {
                Game1.playSound("cancel");
                ModEntry.Instance.Monitor.Log(
                    $"Cannot quick stack into ItemGrabMenu context of type '{itemGrabMenu.context?.GetType().Name ?? "null"}'.",
                    LogLevel.Debug);
                return false;
            }

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
                    Game1.playSound("cancel");
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

            QuickStackSummary quickStackSummary = new();
            HashSet<Item> overflowItems = new();

            // Fill chest stacks with player inventory items
            foreach (Item chestItem in chest.GetItemsForPlayer(who.UniqueMultiplayerID))
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

                    if (ModEntry.Config.FavoriteItems.IsEnabled && ConvenientInventory.FavoriteItemSlots[playerInventory.IndexOf(playerItem)])
                    {
                        // Skip favorited items
                        continue;
                    }

                    if (!playerItem.canStackWith(chestItem))
                    {
                        if (ModEntry.Config.QuickStack.OverflowItems
                            && ModEntry.Config.QuickStack.IgnoreItemQuality
                            && QuickStackLogic.CanStackWithIgnoreQuality(playerItem, chestItem))
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

                        ClickableComponent inventoryComponent = itemGrabMenu.inventory.inventory[playerInventory.IndexOf(playerItem)];
                        itemGrabMenu._transferredItemSprites.Add(
                            new ItemGrabMenu.TransferredItemSprite(playerItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y));

                        if (playerItem.Stack == 0)
                        {
                            who.removeItemFromInventory(playerItem);

                            // Remove player item from overflow list, if we are tracking it.
                            overflowItems.Remove(playerItem);
                        }

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
            IInventory chestItems = chest.GetItemsForPlayer(who.UniqueMultiplayerID);
            if (ModEntry.Config.QuickStack.OverflowItems && chestItems.Count < chest.GetActualCapacity())
            {
                foreach (Item overflowPlayerItem in overflowItems)
                {
                    if (overflowPlayerItem is null || overflowPlayerItem.Stack == 0)
                    {
                        continue;
                    }

                    int itemIndex = playerInventory.IndexOf(overflowPlayerItem);
                    if (itemIndex == -1 || (ModEntry.Config.FavoriteItems.IsEnabled && ConvenientInventory.FavoriteItemSlots[itemIndex]))
                    {
                        // Skip favorited items
                        continue;
                    }

                    int beforeStack = overflowPlayerItem.Stack;
                    Item leftoverItem = null;
                    try
                    {
                        (chest.GetItemsForPlayer() as Inventory).OnSlotChanged += ChestSlotChanged;
                        leftoverItem = chest.addItem(overflowPlayerItem);
                    }
                    finally
                    {
                        (chest.GetItemsForPlayer() as Inventory).OnSlotChanged -= ChestSlotChanged;
                    }
                    bool movedAtLeastOne = leftoverItem is null || beforeStack != leftoverItem.Stack;

                    movedAtLeastOneTotal |= movedAtLeastOne;

                    if (movedAtLeastOne)
                    {
                        ClickableComponent inventoryComponent = itemGrabMenu.inventory.inventory[playerInventory.IndexOf(overflowPlayerItem)];
                        itemGrabMenu._transferredItemSprites.Add(
                                new ItemGrabMenu.TransferredItemSprite(overflowPlayerItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y));

                        quickStackSummary.AddToSummary(typedChest, overflowPlayerItem.Name, leftoverItem?.Stack ?? 0, beforeStack);
                    }

                    if (leftoverItem is null)
                    {
                        who.removeItemFromInventory(overflowPlayerItem);
                    }
                    else
                    {
                        itemGrabMenu.inventory.ShakeItem(overflowPlayerItem);
                    }
                }
            }

            Game1.playSound(movedAtLeastOneTotal ? "Ship" : "cancel");

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
        /// Gets the <see cref="Chest"/> from the given <see cref="ItemGrabMenu"/>'s <see cref="ItemGrabMenu.context"/>, if any.
        /// </summary>
        private static Chest GetChestFromContext(ItemGrabMenu itemGrabMenu) => itemGrabMenu.context switch
        {
            Chest c => c,
            JunimoHut jh => jh.GetOutputChest(),
            _ => null,
        };
    }
}
