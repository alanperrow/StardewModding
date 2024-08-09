using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConvenientInventory.TypedChests;
using Microsoft.Xna.Framework;
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
        public static bool StackToNearbyChests(string rangeStr, InventoryPage inventoryPage = null)
        {
            bool movedAtLeastOneTotal = false;
            Farmer who = Game1.player;

            List<TypedChest> chests = GetTypedChestsWithinRange(who, rangeStr, true);

            Inventory playerInventory = who.Items;

            QuickStackAnimation quickStackAnimation = null;
            if (ModEntry.Config.IsEnableQuickStackAnimation)
            {
                quickStackAnimation = new(who);
            }

            foreach (TypedChest typedChest in chests)
            {
                Chest chest = typedChest.Chest;

                List<Item> stackOverflowItems = new();

                IInventory chestItems = chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin || chest.SpecialChestType == Chest.SpecialChestTypes.JunimoChest
                    ? chest.GetItemsForPlayer(who.UniqueMultiplayerID)
                    : chest.Items;

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
                        
                        if (!playerItem.canStackWith(chestItem))
                        {
                            if (ModEntry.Config.IsQuickStackOverflowItems && ModEntry.Config.IsQuickStackIgnoreItemQuality && CanStackWithIgnoreQuality(playerItem, chestItem))
                            {
                                stackOverflowItems.Add(playerItem.getOne());
                            }

                            continue;
                        }

                        if (ModEntry.Config.IsEnableFavoriteItems && ConvenientInventory.FavoriteItemSlots[playerInventory.IndexOf(playerItem)])
                        {
                            // Skip favorited items
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
                        }

                        if (chestItem.Stack == chestItem.maximumStackSize())
                        {
                            if (ModEntry.Config.IsQuickStackOverflowItems)
                            {
                                stackOverflowItems.Add(chestItem.getOne());
                            }

                            inventoryPage?.inventory.ShakeItem(playerItem);
                            break;
                        }
                    }
                }

                // Add overflow stacks to chest when applicable
                if (ModEntry.Config.IsQuickStackOverflowItems && chestItems.Count < chest.GetActualCapacity())
                {
                    foreach (Item stackOverflowItem in stackOverflowItems)
                    {
                        if (stackOverflowItem is null)
                        {
                            continue;
                        }

                        foreach (Item playerItem in playerInventory)
                        {
                            if (playerItem is null || !playerItem.canStackWith(stackOverflowItem))
                            {
                                continue;
                            }

                            if (ModEntry.Config.IsEnableFavoriteItems && ConvenientInventory.FavoriteItemSlots[playerInventory.IndexOf(playerItem)])
                            {
                                // Skip favorited items
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
            Game1.playSound(movedAtLeastOneTotal ? "Ship" : "cancel");

            return movedAtLeastOneTotal;
        }

        public static List<TypedChest> GetTypedChestsWithinRange(Farmer who, string rangeStr, bool sorted = false)
        {
            if (who is null)
            {
                return new List<TypedChest>();
            }

            QuickStackRangeType rangeType = ConfigHelper.GetQuickStackRangeType(rangeStr);
            if (rangeType == QuickStackRangeType.Tile)
            {
                // Get all chests within the given tile range.
                int tileRange = ConfigHelper.GetQuickStackTileRange(rangeStr);

                Vector2 farmerPosition = who.getStandingPosition();
                Point farmerTileLocation = who.TilePoint;
                GameLocation gameLocation = who.currentLocation;

                return sorted
                    ? GetNearbyTypedChestsWithDistance(farmerPosition, tileRange, gameLocation).OrderBy(x => x.Distance).Select(x => x.TypedChest).ToList()
                    : GetNearbyTypedChests(farmerTileLocation, tileRange, gameLocation);
            }
            else if (rangeType == QuickStackRangeType.Location)
            {
                // Get all chests in the player's current location.
                Vector2 farmerPosition = who.getStandingPosition();
                GameLocation gameLocation = who.currentLocation;

                return GetLocationTypedChestsWithDistance(farmerPosition, gameLocation).OrderBy(x => x.Distance).Select(x => x.TypedChest).ToList();
            }
            else
            {
                // Get all chests in the world.
                // TODO: In multiplayer, if not main player, only get chests in active locations.

                // TODO: Logic
                //...

                return new List<TypedChest>();
            }
        }

        /// <summary>
        /// From origin, return all chests in the provided location, including the point-distance from their tile-center to origin.
        /// </summary>
        private static List<TypedChestWithDistance> GetLocationTypedChestsWithDistance(Vector2 origin, GameLocation gameLocation)
        {
            List<TypedChestWithDistance> typedChestsWithDistance = new();

            foreach (Chest chest in gameLocation.Objects.Values.OfType<Chest>())
            {
                // Make sure chest is not in-use by another player (easy fix to avoid concurrency issues and possibly item deletion/duplication)
                if (chest.GetMutex().IsLocked())
                {
                    continue;
                }

                ChestType chestType = TypedChest.DetermineChestType(chest);
                if (chestType == ChestType.Package)
                {
                    // Do not consider new farmer packages as chests for quick stack
                    continue;
                }

                Vector2 chestTileCenterPosition = GetTileCenterPosition((int)chest.TileLocation.X, (int)chest.TileLocation.Y);

                int chestPosX = (int)chestTileCenterPosition.X;
                int chestPosY = (int)chestTileCenterPosition.Y;

                AddChestToList(chest, typedChestsWithDistance, true, chestPosX, chestPosY, origin, chestType);
            }

            // TODO: Add kitchen fridge chests
            //...

            // TODO: Add building chests
            //...

            return typedChestsWithDistance;
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
                        // Make sure chest is not in-use by another player (easy fix to avoid concurrency issues and possibly item deletion/duplication)
                        if (chest.GetMutex().IsLocked())
                        {
                            continue;
                        }

                        ChestType chestType = TypedChest.DetermineChestType(chest);
                        if (chestType == ChestType.Package)
                        {
                            // Do not consider new farmer packages as chests for quick stack
                            continue;
                        }

                        AddChestToList(chest, chestList, withDist, tx, ty, originPosition ?? default, chestType);
                    }
                }
            }

            // Kitchen fridge
            if (gameLocation is FarmHouse farmHouse && farmHouse.upgradeLevel > 0)  // Kitchen only exists when upgradeLevel > 0
            {
                if (originTile == null)
                {
                    // We are dealing with actual positions
                    Vector2 fridgeTileCenterPosition = GetTileCenterPosition(farmHouse.fridgePosition);
                    if (IsPositionWithinRange(originPosition.Value, fridgeTileCenterPosition, range))
                    {
                        if (farmHouse.fridge.Value != null && !farmHouse.fridge.Value.GetMutex().IsLocked())
                        {
                            Vector2 fridgeTileLoc = new(
                                farmHouse.fridgePosition.X,
                                farmHouse.fridgePosition.Y);

                            tx = (int)fridgeTileCenterPosition.X;
                            ty = (int)fridgeTileCenterPosition.Y;
                            Chest fridgeChest = farmHouse.fridge.Value;
                            AddChestToList(fridgeChest, chestList, withDist, tx, ty, originPosition.Value, ChestType.Fridge, visualTileLoc: fridgeTileLoc);
                        }
                    }
                }
                else
                {
                    // We have a specified tile origin point
                    if (IsTileWithinRange(originTile.Value, farmHouse.fridgePosition, range))
                    {
                        if (farmHouse.fridge.Value != null && !farmHouse.fridge.Value.GetMutex().IsLocked())
                        {
                            Vector2 fridgeTileLoc = new(
                                farmHouse.fridgePosition.X,
                                farmHouse.fridgePosition.Y);

                            Chest fridgeChest = farmHouse.fridge.Value;
                            AddChestToList(fridgeChest, chestList, withDist, chestType: ChestType.Fridge, visualTileLoc: fridgeTileLoc);
                        }
                    }
                }
            }

            // Island kitchen fridge
            if (gameLocation is IslandFarmHouse islandFarmHouse)
            {
                if (originTile == null)
                {
                    // We are dealing with actual positions
                    Vector2 fridgeTileCenterPosition = GetTileCenterPosition(islandFarmHouse.fridgePosition);
                    if (IsPositionWithinRange(originPosition.Value, fridgeTileCenterPosition, range))
                    {
                        if (islandFarmHouse.fridge.Value != null && !islandFarmHouse.fridge.Value.GetMutex().IsLocked())
                        {
                            Vector2 islandFridgeTileLoc = new(
                                islandFarmHouse.fridgePosition.X,
                                islandFarmHouse.fridgePosition.Y);

                            tx = (int)fridgeTileCenterPosition.X;
                            ty = (int)fridgeTileCenterPosition.Y;
                            Chest fridgeChest = islandFarmHouse.fridge.Value;
                            AddChestToList(fridgeChest, chestList, withDist, tx, ty, originPosition.Value, ChestType.Fridge, visualTileLoc: islandFridgeTileLoc);
                        }
                    }
                }
                else
                {
                    // We have a specified tile origin point
                    if (IsTileWithinRange(originTile.Value, islandFarmHouse.fridgePosition, range))
                    {
                        if (islandFarmHouse.fridge.Value != null && !islandFarmHouse.fridge.Value.GetMutex().IsLocked())
                        {
                            Vector2 islandFridgeTileLoc = new(
                                islandFarmHouse.fridgePosition.X,
                                islandFarmHouse.fridgePosition.Y);

                            Chest fridgeChest = islandFarmHouse.fridge.Value;
                            AddChestToList(fridgeChest, chestList, withDist, chestType: ChestType.Fridge, visualTileLoc: islandFridgeTileLoc);
                        }
                    }
                }
            }

            // Buildings
            if (ModEntry.Config.IsQuickStackIntoBuildingsWithInventories)
            {
                foreach (Building building in gameLocation.buildings)
                {
                    if (originTile == null)
                    {
                        // We are dealing with actual positions
                        Vector2 buildingTileCenterPosition = GetTileCenterPosition(building.tileX.Value, building.tileY.Value);
                        if (IsPositionWithinRange(originPosition.Value, buildingTileCenterPosition, range))
                        {
                            if (building is JunimoHut junimoHut)
                            {
                                if (junimoHut.GetOutputChest().GetMutex().IsLocked())
                                {
                                    continue;
                                }

                                Vector2 hutVisualTileLoc = new(
                                    junimoHut.tileX.Value + junimoHut.tilesWide.Value / 2,
                                    junimoHut.tileY.Value + junimoHut.tilesHigh.Value - 1);

                                tx = (int)buildingTileCenterPosition.X;
                                ty = (int)buildingTileCenterPosition.Y;
                                Chest hutChest = junimoHut.GetOutputChest();
                                AddChestToList(hutChest, chestList, withDist, tx, ty, originPosition.Value, ChestType.JunimoHut, visualTileLoc: hutVisualTileLoc);
                            }
                            else if (building.buildingType.Value == "Mill")
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
                                AddChestToList(millChest, chestList, withDist, tx, ty, originPosition.Value, ChestType.Mill, visualTileLoc: millVisualTileLoc);
                            }
                        }
                    }
                    else
                    {
                        // We have a specified tile origin point
                        if (IsTileWithinRange(originTile.Value, building.tileX.Value, building.tileY.Value, range))
                        {
                            if (building is JunimoHut junimoHut)
                            {
                                if (junimoHut.GetOutputChest().GetMutex().IsLocked())
                                {
                                    continue;
                                }

                                Vector2 hutVisualTileLoc = new(
                                    junimoHut.tileX.Value + junimoHut.tilesWide.Value / 2,
                                    junimoHut.tileY.Value + junimoHut.tilesHigh.Value - 1);

                                Chest hutChest = junimoHut.GetOutputChest();
                                AddChestToList(hutChest, chestList, withDist, chestType: ChestType.JunimoHut, visualTileLoc: hutVisualTileLoc);
                            }
                            else if (building.buildingType.Value == "Mill")
                            {
                                if (building.isUnderConstruction() || building.GetBuildingChest("Input").GetMutex().IsLocked())
                                {
                                    continue;
                                }

                                Vector2 millVisualTileLoc = new(
                                    building.tileX.Value + building.tilesWide.Value / 2,
                                    building.tileY.Value + building.tilesHigh.Value - 1);

                                Chest millChest = building.GetBuildingChest("Input");
                                AddChestToList(millChest, chestList, withDist, chestType: ChestType.Mill, visualTileLoc: millVisualTileLoc);
                            }
                        }
                    }
                }
            }
        }

        private static void AddChestToList(
            Chest chest,
            IList chestList,
            bool withDist,
            int posX = default,
            int posY = default,
            Vector2 origin = default,
            ChestType chestType = default,
            Vector2? visualTileLoc = default)
        {
            if (!withDist)
            {
                // TypedChest
                var typedChest = new TypedChest(chest, chestType, visualTileLoc);
                chestList.Add(typedChest);
            }
            else
            {
                // TypedChestWithDistance
                int dx = posX - (int)origin.X;
                int dy = posY - (int)origin.Y;

                var typedChest = new TypedChest(chest, chestType, visualTileLoc);
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

        // Taken from Item.canStackWith, removing quality check.
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
