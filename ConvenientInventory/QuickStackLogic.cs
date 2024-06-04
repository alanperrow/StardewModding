using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConvenientInventory.TypedChests;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using static StardewValley.Minigames.CraneGame;

namespace ConvenientInventory
{
    public static class QuickStackLogic
    {
        public static bool StackToNearbyChests(int range, InventoryPage inventoryPage = null)
        {
            bool movedAtLeastOneTotal = false;
            Farmer who = Game1.player;

            List<Chest> chests = GetChestsAroundFarmer(who, range, true);

            IList<Item> playerInventory = who.Items;

            int numItemsQuickStackAnimation = 0;
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
                            if (ModEntry.Config.IsEnableQuickStackAnimation)
                            {
                                // TODO 1: See FishPond.showObjectThrownIntoPondAnimation()
                                // Seems like you can create a time interval using kinematic equations solved for t.
                                // This quick stack animation should be very similar. We are "lobbing" the items into chests, which could be modeled as a projectile motion equation.
                                /*
			                    float horizontalDistance = this.GetCenterTile().X * 64f - who.position.X;
			                    float gravity2 = 0.0025f;
			                    float velocity2 = (float)Math.Sqrt(2f * gravity2 * height);
			                    float t2 = (float)(Math.Sqrt(2f * (height - distance2) / gravity2) + (double)(velocity2 / gravity2));
			                    t2 *= 1.05f;
			                    t2 = ((who.FacingDirection != 0) ? (t2 * 2.5f) : (t2 * 0.7f));
			                    t2 -= Math.Abs(horizontalDistance) / ((who.FacingDirection == 0) ? 100f : 2f);
			                    Game1.playSound("throwDownITem");
			                    TemporaryAnimatedSpriteList fishTossSprites2 = new TemporaryAnimatedSpriteList();
			                    ParsedItemData itemData2 = ItemRegistry.GetDataOrErrorItem(whichObject.QualifiedItemId);
			                    fishTossSprites2.Add(new TemporaryAnimatedSprite(itemData2.GetTextureName(), itemData2.GetSourceRect(), who.Position + new Vector2(0f, -64f), flipped: false, 0f, Color.White)
			                    {
				                    scale = 4f,
				                    layerDepth = 1f,
				                    totalNumberOfLoops = 1,
				                    interval = t2,
				                    motion = new Vector2(horizontalDistance / ((who.FacingDirection == 0) ? 900f : 1000f), 0f - velocity2),
				                    acceleration = new Vector2(0f, gravity2),
				                    timeBasedMotion = true
			                    });
			                    fishTossSprites2.Add(new TemporaryAnimatedSprite(28, 100f, 2, 1, this.GetCenterTile() * 64f, flicker: false, flipped: false)
			                    {
				                    delayBeforeAnimationStart = (int)t2,
				                    layerDepth = (((float)(int)base.tileY + 0.5f) * 64f + 2f) / 10000f
			                    });
                                */

                                // TODO 2: When quick stack animation begins, visually open all chests which were quick stacked into, then visually close them after animation ends.

                                // Get direction vector
                                // TODO: Optimization: This only needs to be calculated once per chest.
                                Vector2 chestPosition = (chest.TileLocation * Game1.tileSize);// + new Vector2(Game1.tileSize / 2);
                                Vector2 farmerPosition = who.Position;// + new Vector2(Game1.tileSize / 2);
                                Vector2 directionToChest = (chestPosition - farmerPosition) / Game1.tileSize / 2;

                                // Add item sprite to quick stack animation.
                                ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem(playerItem.QualifiedItemId);
                                var sprite = new TemporaryAnimatedSprite(itemData.GetTextureName(), itemData.GetSourceRect(), who.Position + new Vector2(0, -64), flipped: false, 0, Color.White)
                                {
                                    layerDepth = 1,// + 0.01f * (1 + numItemsQuickStackAnimation),
                                    scale = 4,
                                    delayBeforeAnimationStart = numItemsQuickStackAnimation * 50, // For each item in the animation, delay animation start by a couple frames to stagger each item
                                    //ticksBeforeAnimationStart = delay,
                                    motion = directionToChest,
                                    xStopCoordinate = (int)chestPosition.X,
                                    yStopCoordinate = (int)chestPosition.Y,
                                    //scaleChangeChange = -0.001f,
                                    //animationLength = 2,
                                    alphaFadeFade = -0.0005f,
                                };

                                ConvenientInventory.AddQuickStackAnimationItemSprite(sprite);
                                numItemsQuickStackAnimation++;
                            }

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

                            movedAtLeastOneTotal = movedAtLeastOneTotal || movedAtLeastOne;

                            if (movedAtLeastOne)
                            {
                                // TODO: Add item to quick stack bundle animation here.
                                //...

                                if (inventoryPage != null)
                                {
                                    ClickableComponent inventoryComponent = inventoryPage.inventory.inventory[playerInventory.IndexOf(playerItem)];
                                    ConvenientInventory.AddTransferredItemSprite(
                                        new ItemGrabMenu.TransferredItemSprite(playerItem.getOne(), inventoryComponent.bounds.X, inventoryComponent.bounds.Y));
                                }
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

            // Broadcast any item sprites for quick stack animation.
            if (numItemsQuickStackAnimation > 0)
            {
                ConvenientInventory.BroadcastQuickStackAnimationItemSprites(who);
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
            var chestsWithDistance = new List<ChestWithDistance>((2 * range + 1) * (2 * range + 1));

            AddNearbyChestsToList(
                chestList: chestsWithDistance,
                withType: false,
                withDist: true,
                range: range,
                gameLocation: gameLocation,
                originPosition: origin);

            return chestsWithDistance;
        }

        private static List<TypedChestWithDistance> GetNearbyTypedChestsWithDistance(Vector2 origin, int range, GameLocation gameLocation)
        {
            var typedChestsWithDistance = new List<TypedChestWithDistance>((2 * range + 1) * (2 * range + 1));

            AddNearbyChestsToList(
                chestList: typedChestsWithDistance,
                withType: true,
                withDist: true,
                range: range,
                gameLocation: gameLocation,
                originPosition: origin);

            return typedChestsWithDistance;
        }

        private static List<Chest> GetNearbyChests(Point originTile, int range, GameLocation gameLocation)
        {
            var chests = new List<Chest>((2 * range + 1) * (2 * range + 1));

            AddNearbyChestsToList(
                chestList: chests,
                withType: false,
                withDist: false,
                range: range,
                gameLocation: gameLocation,
                originTile: originTile);

            return chests;
        }

        private static List<ITypedChest> GetNearbyTypedChests(Point originTile, int range, GameLocation gameLocation)
        {
            var typedChests = new List<ITypedChest>((2 * range + 1) * (2 * range + 1));

            AddNearbyChestsToList(
                chestList: typedChests,
                withType: true,
                withDist: false,
                range: range,
                gameLocation: gameLocation,
                originTile: originTile);

            return typedChests;
        }

        private static void AddNearbyChestsToList(
            IList chestList,
            bool withType,
            bool withDist,
            int range,
            GameLocation gameLocation,
            Vector2? originPosition = null,
            Point? originTile = null)
        {
            Vector2 originTileCenterPosition = (originTile == null)
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
                        tx = (int)originTileCenterPosition.X + (x * Game1.tileSize);
                        ty = (int)originTileCenterPosition.Y + (y * Game1.tileSize);

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

                        AddChestToList(chest, chestList, withType, withDist, tx, ty, originPosition ?? default, chestType);
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
                            tx = (int)fridgeTileCenterPosition.X;
                            ty = (int)fridgeTileCenterPosition.Y;
                            Chest fridgeChest = farmHouse.fridge.Value;
                            AddChestToList(fridgeChest, chestList, withType, withDist, tx, ty, originPosition.Value, ChestType.Fridge);
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
                            Chest fridgeChest = farmHouse.fridge.Value;
                            AddChestToList(fridgeChest, chestList, withType, withDist, chestType: ChestType.Fridge);
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
                            tx = (int)fridgeTileCenterPosition.X;
                            ty = (int)fridgeTileCenterPosition.Y;
                            Chest fridgeChest = islandFarmHouse.fridge.Value;
                            AddChestToList(fridgeChest, chestList, withType, withDist, tx, ty, originPosition.Value, ChestType.Fridge);
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
                            Chest fridgeChest = islandFarmHouse.fridge.Value;
                            AddChestToList(fridgeChest, chestList, withType, withDist, chestType: ChestType.Fridge);
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

                                tx = (int)buildingTileCenterPosition.X;
                                ty = (int)buildingTileCenterPosition.Y;
                                Chest hutChest = junimoHut.GetOutputChest();
                                AddChestToList(hutChest, chestList, withType, withDist, tx, ty, originPosition.Value, ChestType.JunimoHut);
                            }
                            else if (building.buildingType.Value == "Mill")
                            {
                                if (building.isUnderConstruction() || building.GetBuildingChest("Input").GetMutex().IsLocked())
                                {
                                    continue;
                                }

                                tx = (int)buildingTileCenterPosition.X;
                                ty = (int)buildingTileCenterPosition.Y;
                                Chest millChest = building.GetBuildingChest("Input");
                                AddChestToList(millChest, chestList, withType, withDist, tx, ty, originPosition.Value, ChestType.Mill);
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

                                Chest hutChest = junimoHut.GetOutputChest();
                                AddChestToList(hutChest, chestList, withType, withDist, chestType: ChestType.JunimoHut);
                            }
                            else if (building.buildingType.Value == "Mill")
                            {
                                if (building.isUnderConstruction() || building.GetBuildingChest("Input").GetMutex().IsLocked())
                                {
                                    continue;
                                }

                                Chest millChest = building.GetBuildingChest("Input");
                                AddChestToList(millChest, chestList, withType, withDist, chestType: ChestType.Mill);
                            }
                        }
                    }
                }
            }
        }

        private static void AddChestToList(
            Chest chest,
            IList chestList,
            bool withType,
            bool withDist,
            int tx = default,
            int ty = default,
            Vector2 origin = default,
            ChestType chestType = default)
        {
            if (!withType && !withDist)
            {
                // Chest
                chestList.Add(chest);
            }
            else if (withType && !withDist)
            {
                // TypedChest
                var typedChest = new TypedChest(chest, chestType);
                chestList.Add(typedChest);
            }
            else if (!withType && withDist)
            {
                // ChestWithDistance
                int dx = tx - (int)origin.X;
                int dy = ty - (int)origin.Y;

                var chestWithDistance = new ChestWithDistance(chest, Math.Sqrt(dx * dx + dy * dy));
                chestList.Add(chestWithDistance);
            }
            else
            {
                // TypedChestWithDistance
                int dx = tx - (int)origin.X;
                int dy = ty - (int)origin.Y;

                var typedChest = new TypedChest(chest, chestType);
                var typedChestWithDistance = new TypedChestWithDistance(typedChest, Math.Sqrt(dx * dx + dy * dy));
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
