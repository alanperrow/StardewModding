using System.Collections.Generic;
using ConvenientInventory.AutoOrganize;
using ConvenientInventory.TypedChests;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
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
        public static bool StackToChestInMenu(ItemGrabMenu itemGrabMenu)
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

            if (!QuickStackLogic.ShouldQuickStackInto(chest, out ChestType chestType, false))
            {
                Game1.playSound("cancel");
                return false;
            }

            //TODO: Add config option to allow manual quick stack into chest to bypass a "disabled" quick stack toggle state, true by default.
            if (QuickStackToggleChestLogic.GetQuickStackToggleChestStateFromModData(chest) == QuickStackToggleChestState.Disabled)
            {
                //TODO: Shake Toggle Quick Stack button, if visible.
                Game1.playSound("cancel");
                return false;
            }

            TypedChest typedChest = new(
                chest, itemGrabMenu.context is JunimoHut ? ChestType.JunimoHut : chestType, chest.Location, null);

            bool movedAtLeastOneTotal = false;
            Farmer who = Game1.player;
            Inventory playerInventory = who.Items;

            QuickStackSummary quickStackSummary = new();
            List<Item> overflowItems = new();

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
                            overflowItems.Add(playerItem.getOne());
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
                        }

                        quickStackSummary.AddToSummary(typedChest, playerItem.Name, playerItem.Stack, beforeStack);
                    }

                    if (chestItem.Stack == chestItem.maximumStackSize())
                    {
                        if (ModEntry.Config.QuickStack.OverflowItems)
                        {
                            overflowItems.Add(chestItem.getOne());
                        }

                        itemGrabMenu.inventory.ShakeItem(playerItem);
                        break;
                    }
                }
            }

            // Add overflow stacks to chest when applicable
            IInventory chestItems = chest.GetItemsForPlayer(who.UniqueMultiplayerID);
            if (ModEntry.Config.QuickStack.OverflowItems && chestItems.Count < chest.GetActualCapacity())
            {
                foreach (Item overflowItem in overflowItems)
                {
                    if (overflowItem is null)
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

                        if (!playerItem.canStackWith(overflowItem))
                        {
                            // Skip overflow item if it doesn't stack with player item.
                            continue;
                        }

                        int beforeStack = playerItem.Stack;
                        Item leftoverItem = chest.addItem(playerItem);
                        bool movedAtLeastOne = leftoverItem is null || beforeStack != leftoverItem.Stack;

                        movedAtLeastOneTotal |= movedAtLeastOne;

                        if (movedAtLeastOne)
                        {
                            ClickableComponent inventoryComponent = itemGrabMenu.inventory.inventory[playerInventory.IndexOf(playerItem)];
                            itemGrabMenu._transferredItemSprites.Add(
                                    new ItemGrabMenu.TransferredItemSprite(playerItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y));

                            quickStackSummary.AddToSummary(typedChest, playerItem.Name, leftoverItem?.Stack ?? 0, beforeStack);
                        }

                        if (leftoverItem is null)
                        {
                            who.removeItemFromInventory(playerItem);
                        }
                        else
                        {
                            itemGrabMenu.inventory.ShakeItem(playerItem);
                        }
                    }
                }
            }

            Game1.playSound(movedAtLeastOneTotal ? "Ship" : "cancel");

            if (movedAtLeastOneTotal)
            {
                if (ModEntry.Config.AutoOrganizeChest.IsEnabled)
                {
                    AutoOrganizeLogic.TryOrganizeChestOnFillOutStacks(itemGrabMenu, chest);
                }

                ModEntry.Instance.Monitor.Log(quickStackSummary.GetSummaryMessage(), LogLevel.Trace);
            }

            return movedAtLeastOneTotal;
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
