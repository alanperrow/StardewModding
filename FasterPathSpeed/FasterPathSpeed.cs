using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace FasterPathSpeed
{
    internal class FasterPathSpeed
    {
        #region Farmer Methods
        public static void GetFarmerMovementSpeed(Farmer who, ref float refMovementSpeed)
        {
            // TODO: if (who == null) { return; }

            if ((Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence)
                && (!(Game1.CurrentEvent == null && who.hasBuff("19")))
                && (!ModEntry.Config.IsPathSpeedBuffOnlyOnTheFarm || Game1.currentLocation.IsFarm)) // TODO: (Game1.currentLocation?.IsFarm ?? false)
            {
                bool isOnFeature = Game1.currentLocation.terrainFeatures.TryGetValue(who.Tile, out TerrainFeature terrainFeature);

                if (isOnFeature && terrainFeature is Flooring)
                {
                    float pathSpeedBoost = GetPathSpeedBoostByFlooringType(terrainFeature as Flooring);

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
        #endregion

        #region Object Methods
        public static void ObjectPlacementAction(Object obj, ref bool refSuccess, GameLocation location, int x, int y, Farmer who)
        {
            if (ObjectIsPath(obj) && ModEntry.Config.IsEnablePathReplace)
            {
                Vector2 placementTile = new(x / 64, y / 64);

                // TODO: Investigate if this would do the same thing, but cleaner:
                var debug = Flooring.GetFloorPathItemLookup()/*[base.ItemId]*/;
                //if (Game1.floorPathData.TryGetValue(key, out var floorData) && floorData.PlacementSound != null)
                //{
                //    location.playSound(floorData.PlacementSound);
                //}

                if (!obj.bigCraftable.Value && obj is not Furniture
                    && location.terrainFeatures.TryGetValue(placementTile, out TerrainFeature terrainFeature)
                    && terrainFeature is Flooring flooring
                    && PathIds.WhichIds[System.Convert.ToInt32(flooring.whichFloor.Value)] != obj.ItemId) // TODO: Verify System.Convert.ToInt32() works.
                {
                    location.terrainFeatures.Remove(placementTile);
                    location.terrainFeatures.Add(placementTile, new Flooring(PathIds.WhichIds.IndexOf(obj.ItemId).ToString())); // TODO: Verify .ToString() works.

                    var replacedPath = new Object(PathIds.WhichIds[System.Convert.ToInt32(flooring.whichFloor.Value)], 1); // TODO: Verify System.Convert.ToInt32() works.
                    if (!who.addItemToInventoryBool(replacedPath))
                    {
                        who.dropItem(replacedPath);
                    }

                    location.playSound(GetPathSoundStringByPathId(obj.ItemId));

                    refSuccess = true;
                }
            }
        }

        private static bool ObjectIsPath(Object obj)
        {
            return PathIds.WhichIds.Contains(obj.ItemId);
        }

        private static string GetPathSoundStringByPathId(string id)
        {
            return id switch
            {
                PathIds.Wood => "axchop",
                PathIds.Stone => "thudStep",
                PathIds.Ghost => "axchop",
                PathIds.IceTile => "thudStep",
                PathIds.Straw => "thudStep",
                PathIds.Brick => "thudStep",
                PathIds.Boardwalk => "woodyStep",
                PathIds.Gravel => "dirtyHit",
                PathIds.ColoredCobblestone => "stoneStep",
                PathIds.SteppingStone => "stoneStep",
                PathIds.Cobblestone => "stoneStep",
                PathIds.PlankFlooring => "stoneStep",
                PathIds.TownFlooring => "stoneStep",
                _ => "stoneStep",
            };
        }
        #endregion
    }
}
