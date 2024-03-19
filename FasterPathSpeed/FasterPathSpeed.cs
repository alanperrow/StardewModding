using System.Collections.Generic;
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

            if ((Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence)
                && (!(Game1.CurrentEvent == null && who.hasBuff("19")))
                && (!ModEntry.Config.IsPathSpeedBuffOnlyOnTheFarm || Game1.currentLocation.IsFarm))
            {
                bool isOnFeature = Game1.currentLocation.terrainFeatures.TryGetValue(who.Tile, out TerrainFeature terrainFeature);

                if (isOnFeature && terrainFeature is Flooring)
                {
                    float pathSpeedBoost = GetPathSpeedBoostByFlooringType(terrainFeature as Flooring);

                    // TODO: Remove top line: "who.movementMultiplier * Game1.currentGameTime.ElapsedGameTime.Milliseconds *"
                    //       This logic is already handled in Farmer.getMovementSpeed(), so doing it again here is unnecessary.
                    //       i.e. If we have a large frame delta, say 1000ms, our speed would already have been multiplied coming into this method,
                    //            therefore multiplying the delta again would exponentially increase the movement speed unintentionally.
                    float mult = who.movementMultiplier * Game1.currentGameTime.ElapsedGameTime.Milliseconds *
                        ((!Game1.eventUp && who.isRidingHorse()) ? (ModEntry.Config.IsPathAffectHorseSpeed ? ModEntry.Config.HorsePathSpeedBuffModifier : 0) : 1);

                    refMovementSpeed += (who.movementDirections.Count > 1) ? (0.7f * pathSpeedBoost * mult) : (pathSpeedBoost * mult);
                }
            }
        }

        public static float GetPathSpeedBoostByFlooringType(Flooring flooring)
        {
            if (!ModEntry.Config.IsUseCustomPathSpeedBuffValues)
            {
                return ModEntry.Config.DefaultPathSpeedBuff;
            }

            return flooring.whichFloor.Value switch
            {
                Flooring.wood => ModEntry.Config.CustomPathSpeedBuffValues.Wood,
                Flooring.stone => ModEntry.Config.CustomPathSpeedBuffValues.Stone,
                Flooring.ghost => ModEntry.Config.CustomPathSpeedBuffValues.Ghost,
                Flooring.iceTile => ModEntry.Config.CustomPathSpeedBuffValues.IceTile,
                Flooring.straw => ModEntry.Config.CustomPathSpeedBuffValues.Straw,
                Flooring.gravel => ModEntry.Config.CustomPathSpeedBuffValues.Gravel,
                Flooring.boardwalk => ModEntry.Config.CustomPathSpeedBuffValues.Boardwalk,
                Flooring.colored_cobblestone => ModEntry.Config.CustomPathSpeedBuffValues.ColoredCobblestone,
                Flooring.cobblestone => ModEntry.Config.CustomPathSpeedBuffValues.Cobblestone,
                Flooring.steppingStone => ModEntry.Config.CustomPathSpeedBuffValues.SteppingStone,
                Flooring.brick => ModEntry.Config.CustomPathSpeedBuffValues.Brick,
                Flooring.plankFlooring => ModEntry.Config.CustomPathSpeedBuffValues.PlankFlooring,
                Flooring.townFlooring => ModEntry.Config.CustomPathSpeedBuffValues.TownFlooring,
                _ => ModEntry.Config.DefaultPathSpeedBuff,
            };
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
