﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.FloorsAndPaths;
using StardewValley.TerrainFeatures;

namespace FasterPathSpeed
{
    internal class FasterPathSpeed
    {
        public static void PostGetFarmerMovementSpeed(Farmer who, ref float refMovementSpeed)
        {
            if (who == null || Game1.currentLocation == null)
            {
                return;
            }

            if (ModEntry.Config.IsPathSpeedBuffOnlyOnTheFarm && !Game1.currentLocation.IsFarm)
            {
                return;
            }

            if (Game1.CurrentEvent == null && who.hasBuff("19"))
            {
                return;
            }

            if (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence)
            {
                if (Game1.currentLocation.terrainFeatures.TryGetValue(who.Tile, out TerrainFeature terrainFeature)
                    && terrainFeature is Flooring flooring)
                {
                    float pathSpeedBoost = GetPathSpeedBoostByFlooring(flooring);
                    float mult = GetHorseSpeedMultiplier(who);

                    refMovementSpeed += (who.movementDirections.Count > 1) ? (0.7f * pathSpeedBoost * mult) : (pathSpeedBoost * mult);
                }
                else if (ModEntry.Config.IsTownPathSpeedBuff && IsFarmerOnTownPathTile(who))
                {
                    float pathSpeedBoost = ModEntry.Config.DefaultPathSpeedBuff;
                    float mult = GetHorseSpeedMultiplier(who);

                    refMovementSpeed += (who.movementDirections.Count > 1) ? (0.7f * pathSpeedBoost * mult) : (pathSpeedBoost * mult);
                }
            }
        }

        public static float GetPathSpeedBoostByFlooring(Flooring flooring)
        {
            if (!ModEntry.Config.IsUseCustomPathSpeedBuffValues)
            {
                return ModEntry.Config.DefaultPathSpeedBuff;
            }

            return flooring.whichFloor.Value switch
            {
                Flooring.wood => ModEntry.Config.CustomPathSpeedBuffValues.WoodFloor,
                Flooring.stone => ModEntry.Config.CustomPathSpeedBuffValues.StoneFloor,
                Flooring.ghost => ModEntry.Config.CustomPathSpeedBuffValues.WeatheredFloor,
                Flooring.iceTile => ModEntry.Config.CustomPathSpeedBuffValues.CrystalFloor,
                Flooring.straw => ModEntry.Config.CustomPathSpeedBuffValues.StrawFloor,
                Flooring.gravel => ModEntry.Config.CustomPathSpeedBuffValues.GravelPath,
                Flooring.boardwalk => ModEntry.Config.CustomPathSpeedBuffValues.WoodPath,
                Flooring.colored_cobblestone => ModEntry.Config.CustomPathSpeedBuffValues.CrystalPath,
                Flooring.cobblestone => ModEntry.Config.CustomPathSpeedBuffValues.CobblestonePath,
                Flooring.steppingStone => ModEntry.Config.CustomPathSpeedBuffValues.SteppingStonePath,
                Flooring.brick => ModEntry.Config.CustomPathSpeedBuffValues.BrickFloor,
                Flooring.plankFlooring => ModEntry.Config.CustomPathSpeedBuffValues.RusticPlankFloor,
                Flooring.townFlooring => ModEntry.Config.CustomPathSpeedBuffValues.StoneWalkwayFloor,
                _ => ModEntry.Config.DefaultPathSpeedBuff,
            };
        }

        public static float GetHorseSpeedMultiplier(Farmer who) => (!Game1.eventUp && who.isRidingHorse())
            ? (ModEntry.Config.IsPathAffectHorseSpeed ? ModEntry.Config.HorsePathSpeedBuffModifier : 0f)
            : 1f;

        public static bool IsFarmerOnTownPathTile(Farmer who)
        {
            // First, check if farmer is located on a tile from the Town tile sheet.
            string tileSheetID = Game1.currentLocation.getTileSheetIDAt((int)who.Tile.X, (int)who.Tile.Y, "Back");
            if (tileSheetID != "Town")
            {
                return false;
            }

            // HACK: Check if the Town tile that the farmer is standing on has a property "Type" with a value of "Stone".
            //       This returns true for the stone town path tiles (also returns true for the stone bridges in town, but that's fine).
            return Game1.currentLocation.doesEitherTileOrTileIndexPropertyEqual((int)who.Tile.X, (int)who.Tile.Y, "Type", "Back", "Stone");
        }

        public static void PostObjectPlacementAction(Object obj, ref bool refSuccess, GameLocation location, int x, int y, Farmer who)
        {
            if (!ModEntry.Config.IsEnablePathReplace)
            {
                return;
            }

            Vector2 placementTile = new(x / 64, y / 64);
            Dictionary<string, string> floorPathItemLookup = Flooring.GetFloorPathItemLookup();

            if (obj.IsFloorPathItem()
                && location.terrainFeatures.TryGetValue(placementTile, out TerrainFeature terrainFeature)
                && terrainFeature is Flooring flooring
                && floorPathItemLookup[obj.ItemId] != flooring.whichFloor.Value)
            {
                // Remove the existing path and try to add it to the player inventory.
                location.terrainFeatures.Remove(placementTile);

                FloorPathData flooringPathData = flooring.GetData();
                Object replacedPath = new(flooringPathData.ItemId, 1);
                if (!who.addItemToInventoryBool(replacedPath))
                {
                    // Inventory was full, so drop the replaced path instead.
                    who.dropItem(replacedPath);
                }

                // Place the new path and play its placement sound.
                Flooring newFlooring = new(floorPathItemLookup[obj.ItemId]);
                location.terrainFeatures.Add(placementTile, newFlooring);

                FloorPathData newFlooringPathData = newFlooring.GetData();
                location.playSound(newFlooringPathData.PlacementSound);

                refSuccess = true;
            }
        }
    }
}
