﻿using System;
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

namespace ConvenientInventory
{
    public static class QuickStackLogic
    {
        private class ChestWithDistance
        {
            public Chest Chest { get; private set; }

            public double Distance { get; private set; }

            public ChestWithDistance(Chest chest, double distance)
            {
                Chest = chest;
                Distance = distance;
            }
        }

        private class TypedChestWithDistance
        {
            public ITypedChest TypedChest { get; private set; }

            public double Distance { get; private set; }

            public TypedChestWithDistance(ITypedChest chest, double distance)
            {
                TypedChest = chest;
                Distance = distance;
            }
        }

        public static bool StackToNearbyChests(int range, InventoryPage inventoryPage)
        {
            if (inventoryPage == null)
            {
                return false;
            }

            bool movedAtLeastOneTotal = false;
            Farmer who = Game1.player;

            List<Chest> chests = GetChestsAroundFarmer(who, range, true);

            foreach (Chest chest in chests)
            {
                List<Item> stackOverflowItems = new();

                IInventory chestItems = (chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin || chest.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
                        ? chest.GetItemsForPlayer(who.UniqueMultiplayerID)
                        : chest.Items;

                // Fill chest stacks with player inventory items
                foreach (Item chestItem in chestItems)
                {
                    if (chestItem is null)
                    {
                        continue;
                    }

                    IList<Item> playerInventory = inventoryPage.inventory.actualInventory;

                    foreach (Item playerItem in playerInventory)
                    {
                        if (playerItem is null || !playerItem.canStackWith(chestItem))
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

                        movedAtLeastOneTotal = movedAtLeastOneTotal || movedAtLeastOne;

                        if (movedAtLeastOne)
                        {
                            ClickableComponent inventoryComponent = inventoryPage.inventory.inventory[playerInventory.IndexOf(playerItem)];

                            ConvenientInventory.AddTransferredItemSprite(new ItemGrabMenu.TransferredItemSprite(
                                playerItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y)
                            );

                            if (playerItem.Stack == 0)
                            {
                                who.removeItemFromInventory(playerItem);
                            }
                        }

                        if (chestItem.Stack == chestItem.maximumStackSize())
                        {
                            if (ModEntry.Config.IsQuickStackOverflowItems)
                            {
                                stackOverflowItems.Add(chestItem.getOne());
                            }

                            inventoryPage.inventory.ShakeItem(playerItem);
                            break;
                        }
                    }
                }

                // Add overflow stacks to chest when applicable
                if (ModEntry.Config.IsQuickStackOverflowItems && chestItems.Count < chest.GetActualCapacity())
                {
                    IList<Item> playerInventory = inventoryPage.inventory.actualInventory;

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

                            movedAtLeastOneTotal = movedAtLeastOneTotal || movedAtLeastOne;

                            if (movedAtLeastOne)
                            {
                                ClickableComponent inventoryComponent = inventoryPage.inventory.inventory[playerInventory.IndexOf(playerItem)];

                                ConvenientInventory.AddTransferredItemSprite(new ItemGrabMenu.TransferredItemSprite(
                                    playerItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y)
                                );
                            }

                            if (leftoverItem is null)
                            {
                                who.removeItemFromInventory(playerItem);
                            }
                            else
                            {
                                inventoryPage.inventory.ShakeItem(playerItem);
                            }
                        }
                    }
                }
            }

            Game1.playSound(movedAtLeastOneTotal ? "Ship" : "cancel");

            return movedAtLeastOneTotal;
        }

        // To be used when not in the InventoryPage, accessed via hotkey.
        public static bool StackToNearbyChests(int range)
        {
            bool movedAtLeastOneTotal = false;
            Farmer who = Game1.player;

            List<Chest> chests = GetChestsAroundFarmer(who, range, true);

            foreach (Chest chest in chests)
            {
                List<Item> stackOverflowItems = new();

                IInventory chestItems = (chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin || chest.SpecialChestType == Chest.SpecialChestTypes.JunimoChest)
                        ? chest.GetItemsForPlayer(who.UniqueMultiplayerID)
                        : chest.Items;

                // Fill chest stacks with player inventory items
                foreach (Item chestItem in chestItems)
                {
                    if (chestItem is null)
                    {
                        continue;
                    }

                    IList<Item> playerInventory = who.Items;

                    foreach (Item playerItem in playerInventory)
                    {
                        if (playerItem is null || !playerItem.canStackWith(chestItem))
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

                        movedAtLeastOneTotal = movedAtLeastOneTotal || movedAtLeastOne;

                        if (movedAtLeastOne && playerItem.Stack == 0)
                        {
                            who.removeItemFromInventory(playerItem);
                        }

                        if (chestItem.Stack == chestItem.maximumStackSize())
                        {
                            if (ModEntry.Config.IsQuickStackOverflowItems)
                            {
                                stackOverflowItems.Add(chestItem.getOne());
                            }

                            break;
                        }
                    }
                }

                // Add overflow stacks to chest when applicable
                if (ModEntry.Config.IsQuickStackOverflowItems && chestItems.Count < chest.GetActualCapacity())
                {
                    IList<Item> playerInventory = who.Items;

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

                            movedAtLeastOneTotal = movedAtLeastOneTotal || movedAtLeastOne;

                            if (leftoverItem is null)
                            {
                                who.removeItemFromInventory(playerItem);
                            }
                        }
                    }
                }
            }

            Game1.playSound(movedAtLeastOneTotal ? "Ship" : "cancel");

            return movedAtLeastOneTotal;
        }

        public static List<Chest> GetChestsAroundFarmer(Farmer who, int range, bool sorted = false)
        {
            if (who is null)
            {
                return new List<Chest>();
            }

            Vector2 farmerPosition = who.getStandingPosition();
            Point farmerTileLocation = who.TilePoint;
            GameLocation gameLocation = who.currentLocation;

            return sorted
                ? GetNearbyChestsWithDistance(farmerPosition, range, gameLocation).OrderBy(x => x.Distance).Select(x => x.Chest).ToList()
                : GetNearbyChests(farmerTileLocation, range, gameLocation);
        }

        public static List<ITypedChest> GetTypedChestsAroundFarmer(Farmer who, int range, bool sorted = false)
        {
            if (who is null)
            {
                return new List<ITypedChest>();
            }

            Vector2 farmerPosition = who.getStandingPosition();
            Point farmerTileLocation = who.TilePoint;
            GameLocation gameLocation = who.currentLocation;

            return sorted
                ? GetNearbyTypedChestsWithDistance(farmerPosition, range, gameLocation).OrderBy(x => x.Distance).Select(x => x.TypedChest).ToList()
                : GetNearbyTypedChests(farmerTileLocation, range, gameLocation);
        }

        // From origin, return all nearby chests, including the point-distance from their tile-center to origin, within a given square range.
        private static List<ChestWithDistance> GetNearbyChestsWithDistance(Vector2 origin, int range, GameLocation gameLocation)
        {
            var dChests = new List<ChestWithDistance>((2 * range + 1) * (2 * range + 1));
            Vector2 originTileCenterPosition = GetTileCenterPosition(GetTileLocation(origin));
            Vector2 tileLocation = Vector2.Zero;
            int tx, ty, dx, dy;

            // Chests (includes mini fridge, stone chest, mini shipping bin, junimo chest, ...)
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    tx = (int)originTileCenterPosition.X + (x * Game1.tileSize);
                    ty = (int)originTileCenterPosition.Y + (y * Game1.tileSize);

                    tileLocation.X = tx / Game1.tileSize;
                    tileLocation.Y = ty / Game1.tileSize;

                    if (gameLocation.objects.TryGetValue(tileLocation, out StardewValley.Object obj) && obj is Chest chest)
                    {
                        // Make sure chest is not in-use by another player (easy fix to avoid item deletion)
                        if (chest.GetMutex().IsLocked())
                        {
                            continue;
                        }

                        dx = tx - (int)origin.X;
                        dy = ty - (int)origin.Y;

                        dChests.Add(new ChestWithDistance(chest, Math.Sqrt(dx * dx + dy * dy)));
                    }
                }
            }

            // Kitchen fridge
            if (gameLocation is FarmHouse farmHouse && farmHouse.upgradeLevel > 0)  // Kitchen only exists when upgradeLevel > 0
            {
                Vector2 fridgeTileCenterPosition = GetTileCenterPosition(farmHouse.fridgePosition);

                if (IsPositionWithinRange(origin, fridgeTileCenterPosition, range))
                {
                    if (farmHouse.fridge.Value != null && !farmHouse.fridge.Value.GetMutex().IsLocked())
                    {
                        dx = (int)fridgeTileCenterPosition.X - (int)origin.X;
                        dy = (int)fridgeTileCenterPosition.Y - (int)origin.Y;

                        dChests.Add(new ChestWithDistance(farmHouse.fridge.Value, Math.Sqrt(dx * dx + dy * dy)));
                    }
                }
            }

            // Island kitchen fridge
            if (gameLocation is IslandFarmHouse islandFarmHouse)
            {
                Vector2 fridgeTileCenterPosition = GetTileCenterPosition(islandFarmHouse.fridgePosition);

                if (IsPositionWithinRange(origin, fridgeTileCenterPosition, range))
                {
                    if (islandFarmHouse.fridge.Value != null && !islandFarmHouse.fridge.Value.GetMutex().IsLocked())
                    {
                        dx = (int)fridgeTileCenterPosition.X - (int)origin.X;
                        dy = (int)fridgeTileCenterPosition.Y - (int)origin.Y;

                        dChests.Add(new ChestWithDistance(islandFarmHouse.fridge.Value, Math.Sqrt(dx * dx + dy * dy)));
                    }
                }
            }

            // Buildings
            if (ModEntry.Config.IsQuickStackIntoBuildingsWithInventories)
            {
                foreach (Building building in gameLocation.buildings)
                {
                    Vector2 buildingTileCenterPosition = GetTileCenterPosition(building.tileX.Value, building.tileY.Value);

                    if (IsPositionWithinRange(origin, buildingTileCenterPosition, range))
                    {
                        if (building is JunimoHut junimoHut)
                        {
                            if (junimoHut.GetOutputChest().GetMutex().IsLocked())
                            {
                                continue;
                            }

                            dx = (int)buildingTileCenterPosition.X - (int)origin.X;
                            dy = (int)buildingTileCenterPosition.Y - (int)origin.Y;

                            dChests.Add(new ChestWithDistance(junimoHut.GetOutputChest(), Math.Sqrt(dx * dx + dy * dy)));
                        }
                        else if (building.buildingType.Value == "Mill")
                        {
                            if (building.isUnderConstruction() || building.GetBuildingChest("Input").GetMutex().IsLocked())
                            {
                                continue;
                            }

                            dx = (int)buildingTileCenterPosition.X - (int)origin.X;
                            dy = (int)buildingTileCenterPosition.Y - (int)origin.Y;

                            dChests.Add(new ChestWithDistance(building.GetBuildingChest("Input"), Math.Sqrt(dx * dx + dy * dy)));
                        }
                    }
                }
            }

            return dChests;
        }

        private static List<TypedChestWithDistance> GetNearbyTypedChestsWithDistance(Vector2 origin, int range, GameLocation gameLocation)
        {
            var tdChests = new List<TypedChestWithDistance>((2 * range + 1) * (2 * range + 1));
            Vector2 originTileCenterPosition = GetTileCenterPosition(GetTileLocation(origin));
            Vector2 tileLocation = Vector2.Zero;
            int tx, ty, dx, dy;

            // Chests (includes mini fridge, stone chest, mini shipping bin, junimo chest, ...)
            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    tx = (int)originTileCenterPosition.X + (x * Game1.tileSize);
                    ty = (int)originTileCenterPosition.Y + (y * Game1.tileSize);

                    tileLocation.X = tx / Game1.tileSize;
                    tileLocation.Y = ty / Game1.tileSize;

                    if (gameLocation.objects.TryGetValue(tileLocation, out StardewValley.Object obj) && obj is Chest chest)
                    {
                        // Make sure chest is not in-use by another player (easy fix to avoid item deletion)
                        if (chest.GetMutex().IsLocked())
                        {
                            continue;
                        }

                        dx = tx - (int)origin.X;
                        dy = ty - (int)origin.Y;

                        ChestType chestType = TypedChest.DetermineChestType(chest);
                        if (chestType == ChestType.Package)
                        {
                            continue; // Do not consider new farmer packages as chests for quick stack
                        }

                        var typedChest = new TypedChest(chest, chestType);
                        tdChests.Add(new TypedChestWithDistance(typedChest, Math.Sqrt(dx * dx + dy * dy)));
                    }
                }
            }

            // Kitchen fridge
            if (gameLocation is FarmHouse farmHouse && farmHouse.upgradeLevel > 0)  // Kitchen only exists when upgradeLevel > 0
            {
                Vector2 fridgeTileCenterPosition = GetTileCenterPosition(farmHouse.fridgePosition);

                if (IsPositionWithinRange(origin, fridgeTileCenterPosition, range))
                {
                    if (farmHouse.fridge.Value != null && !farmHouse.fridge.Value.GetMutex().IsLocked())
                    {
                        dx = (int)fridgeTileCenterPosition.X - (int)origin.X;
                        dy = (int)fridgeTileCenterPosition.Y - (int)origin.Y;

                        var typedChest = new TypedChest(farmHouse.fridge.Value, ChestType.Fridge);

                        tdChests.Add(new TypedChestWithDistance(typedChest, Math.Sqrt(dx * dx + dy * dy)));
                    }
                }
            }

            // Island kitchen fridge
            if (gameLocation is IslandFarmHouse islandFarmHouse)
            {
                Vector2 fridgeTileCenterPosition = GetTileCenterPosition(islandFarmHouse.fridgePosition);

                if (IsPositionWithinRange(origin, fridgeTileCenterPosition, range))
                {
                    if (islandFarmHouse.fridge.Value != null && !islandFarmHouse.fridge.Value.GetMutex().IsLocked())
                    {
                        dx = (int)fridgeTileCenterPosition.X - (int)origin.X;
                        dy = (int)fridgeTileCenterPosition.Y - (int)origin.Y;

                        var typedChest = new TypedChest(islandFarmHouse.fridge.Value, ChestType.Fridge);

                        tdChests.Add(new TypedChestWithDistance(typedChest, Math.Sqrt(dx * dx + dy * dy)));
                    }
                }
            }

            // Buildings
            if (ModEntry.Config.IsQuickStackIntoBuildingsWithInventories)
            {
                foreach (Building building in gameLocation.buildings)
                {
                    Vector2 buildingTileCenterPosition = GetTileCenterPosition(building.tileX.Value, building.tileY.Value);

                    if (IsPositionWithinRange(origin, buildingTileCenterPosition, range))
                    {
                        if (building is JunimoHut junimoHut)
                        {
                            if (junimoHut.GetOutputChest().GetMutex().IsLocked())
                            {
                                continue;
                            }

                            dx = (int)buildingTileCenterPosition.X - (int)origin.X;
                            dy = (int)buildingTileCenterPosition.Y - (int)origin.Y;

                            var typedChest = new TypedChest(junimoHut.GetOutputChest(), ChestType.JunimoHut);

                            tdChests.Add(new TypedChestWithDistance(typedChest, Math.Sqrt(dx * dx + dy * dy)));
                        }
                        else if (building.buildingType.Value == "Mill")
                        {
                            if (building.isUnderConstruction() || building.GetBuildingChest("Input").GetMutex().IsLocked())
                            {
                                continue;
                            }

                            dx = (int)buildingTileCenterPosition.X - (int)origin.X;
                            dy = (int)buildingTileCenterPosition.Y - (int)origin.Y;

                            var typedChest = new TypedChest(building.GetBuildingChest("Input"), ChestType.Mill);

                            tdChests.Add(new TypedChestWithDistance(typedChest, Math.Sqrt(dx * dx + dy * dy)));
                        }
                    }
                }
            }

            return tdChests;
        }

        private static List<Chest> GetNearbyChests(Point originTile, int range, GameLocation gameLocation)
        {
            var chests = new List<Chest>((2 * range + 1) * (2 * range + 1));

            // Chests
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    Vector2 tileLocation = new(originTile.X + dx, originTile.Y + dy);

                    if (gameLocation.objects.TryGetValue(tileLocation, out StardewValley.Object obj) && obj is Chest chest)
                    {
                        if (chest.GetMutex().IsLocked())
                        {
                            continue;
                        }

                        chests.Add(chest);
                    }
                }
            }

            // Kitchen fridge
            if (gameLocation is FarmHouse farmHouse && farmHouse.upgradeLevel >= 1) //Lvl 1,2,3 is where you have fridge upgrade
            {
                Point kitchenStandingSpot = farmHouse.getKitchenStandingSpot();
                Point fridgeTileLocation = new(kitchenStandingSpot.X + 2, kitchenStandingSpot.Y - 1); //Fridge spot relative to kitchen spot

                if (IsTileWithinRange(originTile, fridgeTileLocation, range))
                {
                    if (farmHouse.fridge.Value != null && !farmHouse.fridge.Value.GetMutex().IsLocked())
                    {
                        chests.Add(farmHouse.fridge.Value);
                    }
                }
            }

            // Island kitchen fridge
            if (gameLocation is IslandFarmHouse islandFarmHouse)
            {
                Point fridgeTileLocation = islandFarmHouse.fridgePosition;

                if (IsTileWithinRange(originTile, fridgeTileLocation, range))
                {
                    if (islandFarmHouse.fridge.Value != null && !islandFarmHouse.fridge.Value.GetMutex().IsLocked())
                    {
                        chests.Add(islandFarmHouse.fridge.Value);
                    }
                }
            }

            // Buildings
            if (ModEntry.Config.IsQuickStackIntoBuildingsWithInventories)
            {
                foreach (Building building in gameLocation.buildings)
                {
                    if (IsTileWithinRange(originTile, building.tileX.Value, building.tileY.Value, range))
                    {
                        if (building is JunimoHut junimoHut)
                        {
                            if (junimoHut.GetOutputChest().GetMutex().IsLocked())
                            {
                                continue;
                            }

                            chests.Add(junimoHut.GetOutputChest());
                        }
                        else if (building.buildingType.Value == "Mill")
                        {
                            if (building.GetBuildingChest("Input").GetMutex().IsLocked())
                            {
                                continue;
                            }

                            chests.Add(building.GetBuildingChest("Input"));
                        }
                    }
                }
            }

            return chests;
        }

        private static List<ITypedChest> GetNearbyTypedChests(Point originTile, int range, GameLocation gameLocation)
        {
            var tChests = new List<ITypedChest>((2 * range + 1) * (2 * range + 1));

            // Chests
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    Vector2 tileLocation = new(originTile.X + dx, originTile.Y + dy);

                    if (gameLocation.objects.TryGetValue(tileLocation, out StardewValley.Object obj) && obj is Chest chest)
                    {
                        if (chest.GetMutex().IsLocked())
                        {
                            continue;
                        }

                        ChestType chestType = TypedChest.DetermineChestType(chest);
                        if (chestType == ChestType.Package)
                        {
                            continue; // Do not consider new farmer packages as chests for quick stack
                        }

                        tChests.Add(new TypedChest(chest, chestType));
                    }
                }
            }

            // Kitchen fridge
            if (gameLocation is FarmHouse farmHouse && farmHouse.upgradeLevel >= 1) //Lvl 1,2,3 is where you have fridge upgrade
            {
                Point kitchenStandingSpot = farmHouse.getKitchenStandingSpot();
                Point fridgeTileLocation = new(kitchenStandingSpot.X + 2, kitchenStandingSpot.Y - 1); //Fridge spot relative to kitchen spot

                if (IsTileWithinRange(originTile, fridgeTileLocation, range))
                {
                    if (farmHouse.fridge.Value != null && !farmHouse.fridge.Value.GetMutex().IsLocked())
                    {
                        tChests.Add(new TypedChest(farmHouse.fridge.Value, ChestType.Fridge));
                    }
                }
            }

            // Island kitchen fridge
            if (gameLocation is IslandFarmHouse islandFarmHouse)
            {
                Point fridgeTileLocation = islandFarmHouse.fridgePosition;

                if (IsTileWithinRange(originTile, fridgeTileLocation, range))
                {
                    if (islandFarmHouse.fridge.Value != null && !islandFarmHouse.fridge.Value.GetMutex().IsLocked())
                    {
                        tChests.Add(new TypedChest(islandFarmHouse.fridge.Value, ChestType.Fridge));
                    }
                }
            }

            // Buildings
            if (ModEntry.Config.IsQuickStackIntoBuildingsWithInventories)
            {
                foreach (Building building in gameLocation.buildings)
                {
                    if (IsTileWithinRange(originTile, building.tileX.Value, building.tileY.Value, range))
                    {
                        if (building is JunimoHut junimoHut)
                        {
                            if (junimoHut.GetOutputChest().GetMutex().IsLocked())
                            {
                                continue;
                            }

                            tChests.Add(new TypedChest(junimoHut.GetOutputChest(), ChestType.JunimoHut));
                        }
                        else if (building.buildingType.Value == "Mill")
                        {
                            if (building.GetBuildingChest("Input").GetMutex().IsLocked())
                            {
                                continue;
                            }

                            tChests.Add(new TypedChest(building.GetBuildingChest("Input"), ChestType.Mill));
                        }
                    }
                }
            }

            return tChests;
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
                (tileLocation.X * Game1.tileSize) + (Game1.tileSize / 2),
                (tileLocation.Y * Game1.tileSize) + (Game1.tileSize / 2));
        }

        private static Vector2 GetTileCenterPosition(int tileX, int tileY)
        {
            return new Vector2(
                (tileX * Game1.tileSize) + (Game1.tileSize / 2),
                (tileY * Game1.tileSize) + (Game1.tileSize / 2));
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
            return Math.Abs(origin.X - target.X) <= (range * Game1.tileSize) && Math.Abs(origin.Y - target.Y) <= (range * Game1.tileSize);
        }

        // Taken from Item.canStackWith, removing quality check.
        private static bool CanStackWithIgnoreQuality(Item item, ISalable other)
        {
            if (item == null || other == null)
            {
                return false;
            }

            if ((other is StardewValley.Object && item is StardewValley.Object) || (other is ColoredObject && item is ColoredObject))
            {
                if ((other as StardewValley.Object).orderData.Value != (item as StardewValley.Object).orderData.Value)
                {
                    return false;
                }

                if (item is ColoredObject && other is ColoredObject && !(item as ColoredObject).color.Value.Equals((other as ColoredObject).color.Value))
                {
                    return false;
                }

                if (item.maximumStackSize() > 1 && other.maximumStackSize() > 1
                    && (item as StardewValley.Object).ParentSheetIndex == (other as StardewValley.Object).ParentSheetIndex
                    && (item as StardewValley.Object).bigCraftable.Value == (other as StardewValley.Object).bigCraftable.Value)
                {
                    return item.Name.Equals(other.Name);
                }
            }

            return false;
        }
    }
}
