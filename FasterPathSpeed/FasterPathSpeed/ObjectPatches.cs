using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace FasterPathSpeed
{
    public class ObjectPatches
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        public static void PlacementAction_Postfix(Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            try
            {
                if (ObjectIsPath(__instance) && Config.IsEnablePathReplace)
                {
                    Vector2 placementTile = new Vector2(x/64, y/64);

                    if (!__instance.bigCraftable.Value && !(__instance is Furniture)
                        && location.terrainFeatures.TryGetValue(placementTile, out TerrainFeature terrainFeature)
                        && terrainFeature is Flooring flooring
                        && PathIds.WhichIds[flooring.whichFloor.Value] != __instance.ParentSheetIndex)
                    {
                        location.terrainFeatures.Remove(placementTile);
                        location.terrainFeatures.Add(placementTile, new Flooring(PathIds.WhichIds.IndexOf(__instance.ParentSheetIndex)));

                        var replacedPath = new Object(PathIds.WhichIds[flooring.whichFloor.Value], 1);
                        if (!who.addItemToInventoryBool(replacedPath))
                        {
                            who.dropItem(replacedPath);
                        }

                        location.playSound(GetPathSoundStringByPathId(__instance.ParentSheetIndex));

                        __result = true;
                    }
                }
            }
            catch (System.Exception e)
            {
                Monitor.Log($"Failed in {nameof(PlacementAction_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        private static bool ObjectIsPath(Object obj)
        {
            return PathIds.WhichIds.Contains(obj.ParentSheetIndex);
        }

        private static string GetPathSoundStringByPathId(int id)
        {
            switch (id)
            {
                case PathIds.Wood: return "axchop";
                case PathIds.Stone: return "thudStep";
                case PathIds.Ghost: return "axchop";
                case PathIds.IceTile: return "thudStep";
                case PathIds.Straw: return "thudStep";
                case PathIds.Brick: return "thudStep";
                case PathIds.Boardwalk: return "woodyStep";
                case PathIds.Gravel: return "dirtyHit";
                case PathIds.ColoredCobblestone: return "stoneStep";
                case PathIds.SteppingStone: return "stoneStep";
                case PathIds.Cobblestone: return "stoneStep";
                case PathIds.PlankFlooring: return "stoneStep";
                case PathIds.TownFlooring: return "stoneStep";
            }

            return "stoneStep";
        }
    }
}
