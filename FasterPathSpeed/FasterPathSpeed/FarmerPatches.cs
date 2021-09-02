using StardewModdingAPI;
using System;
using StardewValley;

namespace FasterPathSpeed
{
    public class FarmerPatches
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        public static void GetMovementSpeed_Postfix(Farmer __instance, ref float __result)
        {
            try
            {
                if ((Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence) && (!(Game1.CurrentEvent == null && __instance.hasBuff(19))) &&
                    (!Config.IsPathSpeedBuffOnlyOnTheFarm || Game1.currentLocation.IsFarm))
                {
                    bool isOnFeature = Game1.currentLocation.terrainFeatures.TryGetValue(__instance.getTileLocation(), out StardewValley.TerrainFeatures.TerrainFeature terrainFeature);

                    if (isOnFeature && terrainFeature is StardewValley.TerrainFeatures.Flooring)
                    {
                        float pathSpeedBoost = GetPathSpeedBoostByFlooringType(terrainFeature as StardewValley.TerrainFeatures.Flooring);

                        float mult = __instance.movementMultiplier * Game1.currentGameTime.ElapsedGameTime.Milliseconds *
                            ((!Game1.eventUp && __instance.isRidingHorse()) ? (Config.IsPathAffectHorseSpeed ? Config.HorsePathSpeedBuffModifier : 1) : 1);

                        __result += (__instance.movementDirections.Count > 1) ? (0.7f * pathSpeedBoost * mult) : (pathSpeedBoost * mult);
                    }
                }
            }
            catch (Exception e)
            {
                Monitor.Log($"Failed in {nameof(GetMovementSpeed_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        public static float GetPathSpeedBoostByFlooringType(StardewValley.TerrainFeatures.Flooring flooring)
        {
            if (Config.IsUseCustomPathSpeedBuffValues)
            {
                switch (flooring.whichFloor.Value)
                {
                    case StardewValley.TerrainFeatures.Flooring.wood:
                        return Config.CustomPathSpeedBuffValues.Wood;
                    case StardewValley.TerrainFeatures.Flooring.stone:
                        return Config.CustomPathSpeedBuffValues.Stone;
                    case StardewValley.TerrainFeatures.Flooring.ghost:
                        return Config.CustomPathSpeedBuffValues.Ghost;
                    case StardewValley.TerrainFeatures.Flooring.iceTile:
                        return Config.CustomPathSpeedBuffValues.IceTile;
                    case StardewValley.TerrainFeatures.Flooring.straw:
                        return Config.CustomPathSpeedBuffValues.Straw;
                    case StardewValley.TerrainFeatures.Flooring.gravel:
                        return Config.CustomPathSpeedBuffValues.Gravel;
                    case StardewValley.TerrainFeatures.Flooring.boardwalk:
                        return Config.CustomPathSpeedBuffValues.Boardwalk;
                    case StardewValley.TerrainFeatures.Flooring.colored_cobblestone:
                        return Config.CustomPathSpeedBuffValues.ColoredCobblestone;
                    case StardewValley.TerrainFeatures.Flooring.cobblestone:
                        return Config.CustomPathSpeedBuffValues.Cobblestone;
                    case StardewValley.TerrainFeatures.Flooring.steppingStone:
                        return Config.CustomPathSpeedBuffValues.SteppingStone;
                    case StardewValley.TerrainFeatures.Flooring.brick:
                        return Config.CustomPathSpeedBuffValues.Brick;
                    case StardewValley.TerrainFeatures.Flooring.plankFlooring:
                        return Config.CustomPathSpeedBuffValues.PlankFlooring;
                    case StardewValley.TerrainFeatures.Flooring.townFlooring:
                        return Config.CustomPathSpeedBuffValues.TownFlooring;
                }
            }

            return Config.DefaultPathSpeedBuff;
        }
    }
}
