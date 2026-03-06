using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConvenientInventory.AutoOrganize;
using ConvenientInventory.TypedChests;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientInventory.QuickStack
{
    public static class QuickStackLogic
    {
        public static bool IsStackingIntoItemGrabMenu { get; private set; }

        public static bool StackInItemGrabMenu(ItemGrabMenu itemGrabMenu)
        {
            try
            {
                IsStackingIntoItemGrabMenu = true;
                return StackInItemGrabMenuCore(itemGrabMenu);
            }
            finally
            {
                IsStackingIntoItemGrabMenu = false;
            }
        }

        private static bool StackInItemGrabMenuCore(ItemGrabMenu itemGrabMenu)
        {
            Chest chest = itemGrabMenu.context switch
            {
                Chest c => c,
                JunimoHut jh => jh.GetOutputChest(),
                _ => null,
            };

            if (chest is null)
            {
                Game1.playSound("cancel");
                ModEntry.Instance.Monitor.Log(
                    $"Cannot quick stack into ItemGrabMenu context of type '{itemGrabMenu.context?.GetType().Name ?? "null"}'.",
                    LogLevel.Debug);
                return false;
            }

            if (!ShouldQuickStackInto(chest, out ChestType chestType, false))
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
                            && CanStackWithIgnoreQuality(playerItem, chestItem))
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

        public static bool StackToNearbyChests(string rangeStr, InventoryPage inventoryPage = null)
        {
            bool movedAtLeastOneTotal = false;
            Farmer who = Game1.player;

            List<TypedChest> chests = GetTypedChestsWithinRange(who, rangeStr, true);

            Inventory playerInventory = who.Items;

            QuickStackAnimation quickStackAnimation = null;
            if (ModEntry.Config.QuickStack.IsAnimationEnabled)
            {
                quickStackAnimation = new(who);
            }

            QuickStackSummary quickStackSummary = new();

            foreach (TypedChest typedChest in chests)
            {
                Chest chest = typedChest.Chest;
                DresserFakeChest dresserFakeChest = chest as DresserFakeChest;
                ICollection<Item> chestItems;
                if (dresserFakeChest != null)
                {
                    chestItems = dresserFakeChest.Dresser.heldItems.ToList();
                }
                else
                {
                    chestItems = chest.GetItemsForPlayer(who.UniqueMultiplayerID);
                }

                List<Item> overflowItems = new();

                // Fill chest stacks with player inventory items
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

                        if (ModEntry.Config.FavoriteItems.IsEnabled && ConvenientInventory.FavoriteItemSlots[playerInventory.IndexOf(playerItem)])
                        {
                            // Skip favorited items
                            continue;
                        }

                        if (dresserFakeChest != null)
                        {
                            if (ModEntry.Config.QuickStack.OverflowItems && playerItem.QualifiedItemId == chestItem.QualifiedItemId)
                            {
                                // We found an existing occurence of this item in this Dresser, so add it to overflowItems.
                                overflowItems.Add(playerItem.getOne());
                            }

                            // In a Dresser, never attempt to add to existing stacks, as we only deal with single-stack items.
                            continue;
                        }
                        else if (!playerItem.canStackWith(chestItem))
                        {
                            if (ModEntry.Config.QuickStack.OverflowItems
                                && ModEntry.Config.QuickStack.IgnoreItemQuality
                                && CanStackWithIgnoreQuality(playerItem, chestItem))
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
                            if (inventoryPage != null)
                            {
                                ClickableComponent inventoryComponent = inventoryPage.inventory.inventory[playerInventory.IndexOf(playerItem)];
                                ConvenientInventory.AddTransferredItemSprite(
                                    new ItemGrabMenu.TransferredItemSprite(playerItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y));
                            }

                            if (playerItem.Stack == 0)
                            {
                                who.removeItemFromInventory(playerItem);
                            }

                            quickStackAnimation?.AddToAnimation(typedChest, playerItem);
                            quickStackSummary.AddToSummary(typedChest, playerItem.Name, playerItem.Stack, beforeStack);
                        }

                        if (chestItem.Stack == chestItem.maximumStackSize())
                        {
                            if (ModEntry.Config.QuickStack.OverflowItems)
                            {
                                overflowItems.Add(chestItem.getOne());
                            }

                            inventoryPage?.inventory.ShakeItem(playerItem);
                            break;
                        }
                    }
                }

                // Add overflow stacks to chest when applicable
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

                            if (dresserFakeChest != null)
                            {
                                if (playerItem.QualifiedItemId != overflowItem.QualifiedItemId)
                                {
                                    // In a Dresser, skip overflow item if it doesn't match player item.
                                    continue;
                                }
                            }
                            else if (!playerItem.canStackWith(overflowItem))
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
                                if (inventoryPage != null)
                                {
                                    ClickableComponent inventoryComponent = inventoryPage.inventory.inventory[playerInventory.IndexOf(playerItem)];
                                    ConvenientInventory.AddTransferredItemSprite(
                                        new ItemGrabMenu.TransferredItemSprite(playerItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y));
                                }

                                quickStackAnimation?.AddToAnimation(typedChest, playerItem);
                                quickStackSummary.AddToSummary(typedChest, playerItem.Name, leftoverItem?.Stack ?? 0, beforeStack);
                            }

                            if (leftoverItem is null)
                            {
                                who.removeItemFromInventory(playerItem);
                            }
                            else
                            {
                                inventoryPage?.inventory.ShakeItem(playerItem);
                            }
                        }
                    }
                }
            }

            quickStackAnimation?.Complete();

            bool shouldPlaySound = !ModEntry.Config.QuickStack.SuppressSoundWhenNoNearbyChests || chests.Any();
            if (shouldPlaySound)
            {
                Game1.playSound(movedAtLeastOneTotal ? "Ship" : "cancel");
            }

            if (movedAtLeastOneTotal)
            {
                ModEntry.Instance.Monitor.Log(quickStackSummary.GetSummaryMessage(), LogLevel.Trace);
            }

            return movedAtLeastOneTotal;
        }

        public static List<TypedChest> GetTypedChestsWithinRange(Farmer who, string rangeStr, bool sorted = false)
        {
            List<TypedChest> typedChestsWithinRange = new();
            if (who is null)
            {
                return typedChestsWithinRange;
            }

            QuickStackRangeType rangeType = ConfigHelper.GetQuickStackRangeType(rangeStr);
            if (rangeType == QuickStackRangeType.Global)
            {
                // Get all chests in the world.
                Vector2 farmerPosition = who.getStandingPosition();
                GameLocation gameLocation = who.currentLocation;

                typedChestsWithinRange = GetGlobalTypedChests(farmerPosition, gameLocation);
            }
            else if (rangeType == QuickStackRangeType.Location)
            {
                // Get all chests in the player's current location.
                Vector2 farmerPosition = who.getStandingPosition();
                GameLocation gameLocation = who.currentLocation;

                typedChestsWithinRange = GetLocationTypedChests(farmerPosition, gameLocation, orderByDistance: true);
            }
            else
            {
                // Get all chests within the given tile range.
                int tileRange = ConfigHelper.GetQuickStackTileRange(rangeStr);

                Vector2 farmerPosition = who.getStandingPosition();
                Point farmerTileLocation = who.TilePoint;
                GameLocation gameLocation = who.currentLocation;

                typedChestsWithinRange = sorted
                    ? GetNearbyTypedChestsWithDistance(farmerPosition, tileRange, gameLocation).OrderBy(x => x.Distance).Select(x => x.TypedChest).ToList()
                    : GetNearbyTypedChests(farmerTileLocation, tileRange, gameLocation);
            }

            if (ModEntry.Config.QuickStack.IsToggleChestEnabled)
            {
                // Exclude any chests that have toggled off quick stack.
                List<(TypedChest TypedChest, QuickStackToggleChestState ToggleState)> filteredChestsWithToggleState = new();
                foreach (TypedChest typedChest in typedChestsWithinRange)
                {
                    QuickStackToggleChestState state = QuickStackToggleChestLogic.GetQuickStackToggleChestStateFromModData(typedChest.Chest);
                    if (state != QuickStackToggleChestState.Disabled)
                    {
                        filteredChestsWithToggleState.Add((typedChest, state));
                    }
                }

                // Order the filtered chests by quick stack priority.
                // If chests are already sorted by distance, this will maintain that sorting order but will prioritize chests with higher priority levels.
                typedChestsWithinRange = ModEntry.Config.QuickStack.IsPrioritizeChestEnabled
                    ? filteredChestsWithToggleState.OrderByDescending(x => x.ToggleState).Select(x => x.TypedChest).ToList()
                    : filteredChestsWithToggleState.Select(x => x.TypedChest).ToList();
            }

            return typedChestsWithinRange;
        }

        /// <summary>
        /// Determines if the provided chest can be quick stacked into, based on its chest type and mutex state (optional).
        /// If <paramref name="chest"/> is <see langword="null"/>, returns <see langword="false"/>.
        /// </summary>
        public static bool ShouldQuickStackInto(Chest chest, out ChestType chestType, bool checkMutex = true)
        {
            if (chest is null || (checkMutex && chest.GetMutex().IsLocked()))
            {
                // Prevent quick stack if chest is in-use by a player (easy fix to avoid concurrency issues and possibly item deletion/duplication)
                chestType = default;
                return false;
            }

            chestType = TypedChest.DetermineChestType(chest);
            if (chestType is ChestType.Package or ChestType.Dungeon)
            {
                // Do not consider new farmer packages or dungeon chests for quick stack
                return false;
            }
            else if (!ModEntry.Config.QuickStack.IntoHoppers && chestType == ChestType.Hopper)
            {
                return false;
            }
            else if (!ModEntry.Config.QuickStack.IntoMiniShippingBins && chestType == ChestType.MiniShippingBin)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns all chests from all locations in the world.
        /// Priority is given to chests in <paramref name="gameLocation"/>, which are ordered by the point-distance from their tile-center to origin.
        /// Next priority is given to any interior locations of <paramref name="gameLocation"/>.
        /// The remainder of chests in other locations of the world are returned in an arbitrary order.
        /// </summary>
        /// <remarks>In multiplayer, only the host (main player) has access to all locations in the world;
        /// non-host players will only be able to access chests in active locations.</remarks>
        private static List<TypedChest> GetGlobalTypedChests(Vector2 origin, GameLocation gameLocation)
        {
            // First, get all chests in the player's current location.
            List<TypedChest> chests = GetLocationTypedChests(origin, gameLocation, orderByDistance: true);

            // Then, get all chests in all instanced interiors of the player's current location.
            IEnumerable<GameLocation> gameLocationInteriors = gameLocation.GetInstancedBuildingInteriors().Where(x => x.IsActiveLocation());
            foreach (GameLocation interiorLocation in gameLocationInteriors)
            {
                List<TypedChest> interiorChests = GetLocationTypedChests(Vector2.Zero, interiorLocation);
                chests.AddRange(interiorChests);
            }

            // Finally, get chests from all other locations and their instanced interiors.
            // If not main player, only get chests in active locations.
            IEnumerable<GameLocation> accessibleLocations = Context.IsMainPlayer
                ? Game1.locations.Concat(Game1.locations.SelectMany(x => x.GetInstancedBuildingInteriors()))
                : ModEntry.Instance.Helper.Multiplayer.GetActiveLocations();

            IEnumerable<GameLocation> otherLocations = accessibleLocations
                .Except(new[] { gameLocation })
                .Except(gameLocationInteriors);
            foreach (GameLocation otherLocation in otherLocations)
            {
                List<TypedChest> otherChests = GetLocationTypedChests(Vector2.Zero, otherLocation);
                chests.AddRange(otherChests);
            }

            return chests;
        }

        /// <summary>
        /// Returns all chests in the provided game location.
        /// If <paramref name="orderByDistance"/> is <see langword="true"/>, these chests are ordered by the point-distance from their tile-center to origin.
        /// </summary>
        private static List<TypedChest> GetLocationTypedChests(Vector2 origin, GameLocation gameLocation, bool orderByDistance = false)
        {
            List<TypedChestWithDistance> typedChestsWithDistance = orderByDistance ? new() : null;
            List<TypedChest> typedChests = orderByDistance ? null : new();

            foreach (Chest chest in gameLocation.Objects.Values.OfType<Chest>())
            {
                if (!ShouldQuickStackInto(chest, out ChestType chestType))
                {
                    continue;
                }

                if (orderByDistance)
                {
                    Vector2 chestTileCenterPosition = GetTileCenterPosition((int)chest.TileLocation.X, (int)chest.TileLocation.Y);
                    int chestPosX = (int)chestTileCenterPosition.X;
                    int chestPosY = (int)chestTileCenterPosition.Y;
                    AddChestToList(chest, typedChestsWithDistance, true, gameLocation, chestPosX, chestPosY, origin, chestType);
                }
                else
                {
                    AddChestToList(chest, typedChests, false, gameLocation, chestType: chestType);
                }
            }

            // Kitchen fridge
            if (gameLocation is FarmHouse farmHouse)
            {
                Chest fridgeChest = farmHouse.GetFridge();
                if (ShouldQuickStackInto(fridgeChest, out _))
                {
                    Point fridgeTileLocPoint = farmHouse.GetFridgePosition().Value;
                    Vector2 fridgeTileLoc = new(
                        fridgeTileLocPoint.X,
                        fridgeTileLocPoint.Y);

                    if (orderByDistance)
                    {
                        Vector2 fridgeTileCenterPosition = GetTileCenterPosition(fridgeTileLocPoint);
                        int fridgePosX = (int)fridgeTileCenterPosition.X;
                        int fridgePosY = (int)fridgeTileCenterPosition.Y;
                        AddChestToList(fridgeChest, typedChestsWithDistance, true, gameLocation, fridgePosX, fridgePosY, origin, ChestType.Fridge, fridgeTileLoc);
                    }
                    else
                    {
                        AddChestToList(fridgeChest, typedChests, false, gameLocation, chestType: ChestType.Fridge, visualTileLoc: fridgeTileLoc);
                    }
                }
            }

            // Island kitchen fridge
            if (gameLocation is IslandFarmHouse islandFarmHouse)
            {
                Chest islandFridgeChest = islandFarmHouse.GetFridge();
                if (ShouldQuickStackInto(islandFridgeChest, out _))
                {
                    Point islandFridgeTileLocPoint = islandFarmHouse.GetFridgePosition().Value;
                    Vector2 islandFridgeTileLoc = new(
                        islandFridgeTileLocPoint.X,
                        islandFridgeTileLocPoint.Y);

                    if (orderByDistance)
                    {
                        Vector2 islandFridgeTileCenterPosition = GetTileCenterPosition(islandFridgeTileLocPoint);
                        int fridgePosX = (int)islandFridgeTileCenterPosition.X;
                        int fridgePosY = (int)islandFridgeTileCenterPosition.Y;
                        AddChestToList(islandFridgeChest, typedChestsWithDistance, true, gameLocation, fridgePosX, fridgePosY, origin, ChestType.IslandFridge, islandFridgeTileLoc);
                    }
                    else
                    {
                        AddChestToList(islandFridgeChest, typedChests, false, gameLocation, chestType: ChestType.IslandFridge, visualTileLoc: islandFridgeTileLoc);
                    }
                }
            }

            // Buildings
            if (ModEntry.Config.QuickStack.IntoBuildingsWithInventories)
            {
                foreach (Building building in gameLocation.buildings)
                {
                    if (ModEntry.Config.QuickStack.IntoJunimoHuts && building is JunimoHut junimoHut)
                    {
                        Chest hutChest = junimoHut.GetOutputChest();
                        if (!ShouldQuickStackInto(hutChest, out _))
                        {
                            continue;
                        }

                        Vector2 hutVisualTileLoc = new(
                            junimoHut.tileX.Value + junimoHut.tilesWide.Value / 2,
                            junimoHut.tileY.Value + junimoHut.tilesHigh.Value - 1);

                        if (orderByDistance)
                        {
                            Vector2 hutTileCenterPosition = GetTileCenterPosition(building.tileX.Value, building.tileY.Value);
                            int hutPosX = (int)hutTileCenterPosition.X;
                            int hutPosY = (int)hutTileCenterPosition.Y;
                            AddChestToList(hutChest, typedChestsWithDistance, true, gameLocation, hutPosX, hutPosY, origin, ChestType.JunimoHut, hutVisualTileLoc);
                        }
                        else
                        {
                            AddChestToList(hutChest, typedChests, false, gameLocation, chestType: ChestType.JunimoHut, visualTileLoc: hutVisualTileLoc);
                        }

                    }
                    else if (ModEntry.Config.QuickStack.IntoMills && building.buildingType.Value == "Mill")
                    {
                        if (building.isUnderConstruction() || building.GetBuildingChest("Input").GetMutex().IsLocked())
                        {
                            continue;
                        }

                        Chest millChest = building.GetBuildingChest("Input");
                        Vector2 millVisualTileLoc = new(
                            building.tileX.Value + building.tilesWide.Value / 2,
                            building.tileY.Value + building.tilesHigh.Value - 1);

                        if (orderByDistance)
                        {
                            Vector2 millTileCenterPosition = GetTileCenterPosition(building.tileX.Value, building.tileY.Value);
                            int millPosX = (int)millTileCenterPosition.X;
                            int millPosY = (int)millTileCenterPosition.Y;
                            AddChestToList(millChest, typedChestsWithDistance, true, gameLocation, millPosX, millPosY, origin, ChestType.Mill, millVisualTileLoc);
                        }
                        else
                        {
                            AddChestToList(millChest, typedChests, false, gameLocation, chestType: ChestType.Mill, visualTileLoc: millVisualTileLoc);
                        }
                    }
                }
            }

            // Dressers
            if (ModEntry.Config.QuickStack.IntoDressers)
            {
                foreach (StorageFurniture dresser in gameLocation.furniture.Where(x => x is StorageFurniture and not FishTankFurniture).Cast<StorageFurniture>())
                {
                    if (dresser.QualifiedItemId == "(F)CCFishTank" && gameLocation is CommunityCenter)
                    {
                        // Do not consider temporary Community Center fish tank.
                        continue;
                    }

                    if (dresser.mutex.IsLocked())
                    {
                        continue;
                    }

                    Vector2 dresserVisualTileLoc = new(
                        dresser.TileLocation.X + (dresser.getTilesWide() - 1) / 2f,
                        dresser.TileLocation.Y + dresser.getTilesHigh() - 1);

                    Chest dresserFakeChest = new DresserFakeChest(dresser);
                    if (orderByDistance)
                    {
                        Vector2 dresserTileCenterPosition = GetTileCenterPosition((int)dresser.TileLocation.X, (int)dresser.TileLocation.Y);
                        int dresserPosX = (int)dresserTileCenterPosition.X;
                        int dresserPosY = (int)dresserTileCenterPosition.Y;
                        AddChestToList(dresserFakeChest, typedChestsWithDistance, true, gameLocation, dresserPosX, dresserPosY, origin, ChestType.Dresser, dresserVisualTileLoc);
                    }
                    else
                    {
                        AddChestToList(dresserFakeChest, typedChests, false, gameLocation, chestType: ChestType.Dresser, visualTileLoc: dresserVisualTileLoc);
                    }
                }
            }

            return orderByDistance
                ? typedChestsWithDistance.OrderBy(x => x.Distance).Select(x => x.TypedChest).ToList()
                : typedChests;
        }

        /// <summary>
        /// From origin, return all nearby chests, including the point-distance from their tile-center to origin, within a given square range.
        /// </summary>
        private static List<TypedChestWithDistance> GetNearbyTypedChestsWithDistance(Vector2 origin, int range, GameLocation gameLocation)
        {
            var typedChestsWithDistance = new List<TypedChestWithDistance>((2 * range + 1) * (2 * range + 1));

            AddNearbyChestsToList(
                chestList: typedChestsWithDistance,
                withDist: true,
                range: range,
                gameLocation: gameLocation,
                originPosition: origin);

            return typedChestsWithDistance;
        }

        /// <summary>
        /// From originTile, return all nearby chests within a given square range.
        /// </summary>
        private static List<TypedChest> GetNearbyTypedChests(Point originTile, int range, GameLocation gameLocation)
        {
            var typedChests = new List<TypedChest>((2 * range + 1) * (2 * range + 1));

            AddNearbyChestsToList(
                chestList: typedChests,
                withDist: false,
                range: range,
                gameLocation: gameLocation,
                originTile: originTile);

            return typedChests;
        }

        private static void AddNearbyChestsToList(
            IList chestList,
            bool withDist,
            int range,
            GameLocation gameLocation,
            Vector2? originPosition = null,
            Point? originTile = null)
        {
            Vector2 originTileCenterPosition = originTile == null
                ? GetTileCenterPosition(GetTileLocation(originPosition.Value))
                : default;
            Vector2 tileLocation = Vector2.Zero;
            int tx = 0, ty = 0;

            // Chests (includes mini fridge, stone chest, mini shipping bin, junimo chest, ...)
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    if (originTile == null)
                    {
                        // We are dealing with actual positions
                        tx = (int)originTileCenterPosition.X + x * Game1.tileSize;
                        ty = (int)originTileCenterPosition.Y + y * Game1.tileSize;

                        tileLocation.X = tx / Game1.tileSize;
                        tileLocation.Y = ty / Game1.tileSize;
                    }
                    else
                    {
                        // We have a specified tile origin point
                        tileLocation = new Vector2(originTile.Value.X + x, originTile.Value.Y + y);
                    }

                    if (gameLocation.objects.TryGetValue(tileLocation, out StardewValley.Object obj) && obj is Chest chest)
                    {
                        if (!ShouldQuickStackInto(chest, out ChestType chestType))
                        {
                            continue;
                        }

                        AddChestToList(chest, chestList, withDist, gameLocation, tx, ty, originPosition ?? default, chestType);
                    }
                }
            }

            // Kitchen fridge
            if (gameLocation is FarmHouse farmHouse)
            {
                Chest fridgeChest = farmHouse.GetFridge();
                if (ShouldQuickStackInto(fridgeChest, out _))
                {
                    Point fridgeTileLocPoint = farmHouse.GetFridgePosition().Value;
                    Vector2 fridgeTileLoc = new(
                        fridgeTileLocPoint.X,
                        fridgeTileLocPoint.Y);

                    if (originTile == null)
                    {
                        // We are dealing with actual positions
                        Vector2 fridgeTileCenterPosition = GetTileCenterPosition(fridgeTileLocPoint);
                        if (IsPositionWithinRange(originPosition.Value, fridgeTileCenterPosition, range))
                        {
                            tx = (int)fridgeTileCenterPosition.X;
                            ty = (int)fridgeTileCenterPosition.Y;
                            AddChestToList(fridgeChest, chestList, withDist, gameLocation, tx, ty, originPosition.Value, ChestType.Fridge, fridgeTileLoc);
                        }
                    }
                    else
                    {
                        // We have a specified tile origin point
                        if (IsTileWithinRange(originTile.Value, fridgeTileLocPoint, range))
                        {
                            AddChestToList(fridgeChest, chestList, withDist, gameLocation, chestType: ChestType.Fridge, visualTileLoc: fridgeTileLoc);
                        }
                    }
                }
            }

            // Island kitchen fridge
            if (gameLocation is IslandFarmHouse islandFarmHouse)
            {
                Chest islandFridgeChest = islandFarmHouse.GetFridge();
                if (ShouldQuickStackInto(islandFridgeChest, out _))
                {
                    Point islandFridgeTileLocPoint = islandFarmHouse.GetFridgePosition().Value;
                    Vector2 islandFridgeTileLoc = new(
                        islandFridgeTileLocPoint.X,
                        islandFridgeTileLocPoint.Y);

                    if (originTile == null)
                    {
                        // We are dealing with actual positions
                        Vector2 fridgeTileCenterPosition = GetTileCenterPosition(islandFridgeTileLocPoint);
                        if (IsPositionWithinRange(originPosition.Value, fridgeTileCenterPosition, range))
                        {
                            tx = (int)fridgeTileCenterPosition.X;
                            ty = (int)fridgeTileCenterPosition.Y;
                            AddChestToList(islandFridgeChest, chestList, withDist, gameLocation, tx, ty, originPosition.Value, ChestType.IslandFridge, islandFridgeTileLoc);
                        }
                    }
                    else
                    {
                        // We have a specified tile origin point
                        if (IsTileWithinRange(originTile.Value, islandFarmHouse.fridgePosition, range))
                        {
                            AddChestToList(islandFridgeChest, chestList, withDist, gameLocation, chestType: ChestType.IslandFridge, visualTileLoc: islandFridgeTileLoc);
                        }
                    }
                }
            }

            // Buildings
            if (ModEntry.Config.QuickStack.IntoBuildingsWithInventories)
            {
                foreach (Building building in gameLocation.buildings)
                {
                    if (originTile == null)
                    {
                        // We are dealing with actual positions
                        Vector2 buildingTileCenterPosition = GetTileCenterPosition(building.tileX.Value, building.tileY.Value);
                        if (IsPositionWithinRange(originPosition.Value, buildingTileCenterPosition, range))
                        {
                            if (ModEntry.Config.QuickStack.IntoJunimoHuts && building is JunimoHut junimoHut)
                            {
                                Chest hutChest = junimoHut.GetOutputChest();
                                if (!ShouldQuickStackInto(hutChest, out _))
                                {
                                    continue;
                                }

                                Vector2 hutVisualTileLoc = new(
                                    junimoHut.tileX.Value + junimoHut.tilesWide.Value / 2,
                                    junimoHut.tileY.Value + junimoHut.tilesHigh.Value - 1);

                                tx = (int)buildingTileCenterPosition.X;
                                ty = (int)buildingTileCenterPosition.Y;
                                AddChestToList(hutChest, chestList, withDist, gameLocation, tx, ty, originPosition.Value, ChestType.JunimoHut,  hutVisualTileLoc);
                            }
                            else if (ModEntry.Config.QuickStack.IntoMills && building.buildingType.Value == "Mill")
                            {
                                if (building.isUnderConstruction() || building.GetBuildingChest("Input").GetMutex().IsLocked())
                                {
                                    continue;
                                }

                                Vector2 millVisualTileLoc = new(
                                    building.tileX.Value + building.tilesWide.Value / 2,
                                    building.tileY.Value + building.tilesHigh.Value - 1);

                                tx = (int)buildingTileCenterPosition.X;
                                ty = (int)buildingTileCenterPosition.Y;
                                Chest millChest = building.GetBuildingChest("Input");
                                AddChestToList(millChest, chestList, withDist, gameLocation, tx, ty, originPosition.Value, ChestType.Mill, millVisualTileLoc);
                            }
                        }
                    }
                    else
                    {
                        // We have a specified tile origin point
                        if (IsTileWithinRange(originTile.Value, building.tileX.Value, building.tileY.Value, range))
                        {
                            if (ModEntry.Config.QuickStack.IntoJunimoHuts && building is JunimoHut junimoHut)
                            {
                                Chest hutChest = junimoHut.GetOutputChest();
                                if (!ShouldQuickStackInto(hutChest, out _))
                                {
                                    continue;
                                }

                                Vector2 hutVisualTileLoc = new(
                                    junimoHut.tileX.Value + junimoHut.tilesWide.Value / 2,
                                    junimoHut.tileY.Value + junimoHut.tilesHigh.Value - 1);

                                AddChestToList(hutChest, chestList, withDist, gameLocation, chestType: ChestType.JunimoHut, visualTileLoc: hutVisualTileLoc);
                            }
                            else if (ModEntry.Config.QuickStack.IntoMills && building.buildingType.Value == "Mill")
                            {
                                if (building.isUnderConstruction() || building.GetBuildingChest("Input").GetMutex().IsLocked())
                                {
                                    continue;
                                }

                                Vector2 millVisualTileLoc = new(
                                    building.tileX.Value + building.tilesWide.Value / 2,
                                    building.tileY.Value + building.tilesHigh.Value - 1);

                                Chest millChest = building.GetBuildingChest("Input");
                                AddChestToList(millChest, chestList, withDist, gameLocation, chestType: ChestType.Mill, visualTileLoc: millVisualTileLoc);
                            }
                        }
                    }
                }
            }

            // Dressers
            if (ModEntry.Config.QuickStack.IntoDressers)
            {
                foreach (StorageFurniture dresser in gameLocation.furniture.Where(x => x is StorageFurniture and not FishTankFurniture).Cast<StorageFurniture>())
                {
                    if (dresser.QualifiedItemId == "(F)CCFishTank" && gameLocation is CommunityCenter)
                    {
                        // Do not consider temporary Community Center fish tank.
                        continue;
                    }

                    if (dresser.mutex.IsLocked())
                    {
                        continue;
                    }

                    Vector2 dresserVisualTileLoc = new(
                        dresser.TileLocation.X + (dresser.getTilesWide() - 1) / 2f,
                        dresser.TileLocation.Y + dresser.getTilesHigh() - 1);

                    if (originTile == null)
                    {
                        // We are dealing with actual positions.
                        Vector2 dresserTileCenterPosition = GetTileCenterPosition((int)dresser.TileLocation.X, (int)dresser.TileLocation.Y);
                        if (IsPositionWithinRange(originPosition.Value, dresserTileCenterPosition, range))
                        {
                            tx = (int)dresserTileCenterPosition.X;
                            ty = (int)dresserTileCenterPosition.Y;
                            Chest dresserFakeChest = new DresserFakeChest(dresser);
                            AddChestToList(dresserFakeChest, chestList, withDist, gameLocation, tx, ty, originPosition.Value, ChestType.Dresser, dresserVisualTileLoc);
                        }
                    }
                    else
                    {
                        // We have a specified tile origin point
                        if (IsTileWithinRange(originTile.Value, (int)dresser.TileLocation.X, (int)dresser.TileLocation.Y, range))
                        {
                            Chest dresserFakeChest = new DresserFakeChest(dresser);
                            AddChestToList(dresserFakeChest, chestList, withDist, gameLocation, chestType: ChestType.Dresser, visualTileLoc: dresserVisualTileLoc);
                        }
                    }
                }
            }
        }

        private static void AddChestToList(
            Chest chest,
            IList chestList,
            bool withDist,
            GameLocation chestGameLocation,
            int posX = default,
            int posY = default,
            Vector2 origin = default,
            ChestType chestType = default,
            Vector2? visualTileLoc = default)
        {
            if (!withDist)
            {
                // TypedChest
                var typedChest = new TypedChest(chest, chestType, chestGameLocation, visualTileLoc);
                chestList.Add(typedChest);
            }
            else
            {
                // TypedChestWithDistance
                int dx = posX - (int)origin.X;
                int dy = posY - (int)origin.Y;

                var typedChest = new TypedChest(chest, chestType, chestGameLocation, visualTileLoc);
                var typedChestWithDistance = new TypedChestWithDistance(typedChest, Math.Sqrt(((long)dx * dx) + ((long)dy * dy)));
                chestList.Add(typedChestWithDistance);
            }
        }

        private static Point GetTileLocation(Vector2 position)
        {
            return new Point(
                (int)position.X / Game1.tileSize,
                (int)position.Y / Game1.tileSize);
        }

        private static Vector2 GetTileCenterPosition(Point tileLocation)
        {
            return new Vector2(
                tileLocation.X * Game1.tileSize + Game1.tileSize / 2,
                tileLocation.Y * Game1.tileSize + Game1.tileSize / 2);
        }

        private static Vector2 GetTileCenterPosition(int tileX, int tileY)
        {
            return new Vector2(
                tileX * Game1.tileSize + Game1.tileSize / 2,
                tileY * Game1.tileSize + Game1.tileSize / 2);
        }

        private static bool IsTileWithinRange(Point origin, Point target, int range)
        {
            return Math.Abs(origin.X - target.X) <= range && Math.Abs(origin.Y - target.Y) <= range;
        }

        private static bool IsTileWithinRange(Point origin, int targetX, int targetY, int range)
        {
            return Math.Abs(origin.X - targetX) <= range && Math.Abs(origin.Y - targetY) <= range;
        }

        private static bool IsPositionWithinRange(Vector2 origin, Vector2 target, int range)
        {
            return Math.Abs(origin.X - target.X) <= range * Game1.tileSize && Math.Abs(origin.Y - target.Y) <= range * Game1.tileSize;
        }

        // Taken from `Item.canStackWith`, removing quality check.
        private static bool CanStackWithIgnoreQuality(Item item, ISalable other)
        {
            if (other is not Item otherItem || other.GetType() != item.GetType())
            {
                return false;
            }

            if (item is ColoredObject coloredObj)
            {
                if (other is ColoredObject otherColoredObj && !coloredObj.color.Value.Equals(otherColoredObj.color.Value))
                {
                    return false;
                }
            }

            if (item.maximumStackSize() <= 1 || other.maximumStackSize() <= 1)
            {
                return false;
            }

            if (item is StardewValley.Object obj)
            {
                if (other is StardewValley.Object otherObj && otherObj.orderData.Value != obj.orderData.Value)
                {
                    return false;
                }
            }

            if (item.QualifiedItemId != otherItem.QualifiedItemId)
            {
                return false;
            }

            if (!item.Name.Equals(other.Name))
            {
                return false;
            }

            return true;
        }
    }
}
