using HarmonyLib;
using StardewModdingAPI;
using System;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FasterPathSpeed
{
    [HarmonyPatch]
    public class FlooringPatches
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        [HarmonyPatch(typeof(Flooring), nameof(Flooring.doCollisionAction))]
        public static void DoCollisionAction_Postfix(Flooring __instance, Character who, GameLocation location)
        {
            try
            {
                if (who != null && who is Farmer && (!Config.IsPathSpeedBuffOnlyOnTheFarm || location is Farm))
                {
                    if (Config.IsUseCustomPathSpeedBuffValues)
                    {
                        CustomPathSpeedBuffValues vals = Config.CustomPathSpeedBuffValues;

                        switch (__instance.whichFloor.Value)
                        {
                            case Flooring.wood:
                                (who as Farmer).temporarySpeedBuff = vals.Wood;
                                break;
                            case Flooring.stone:
                                (who as Farmer).temporarySpeedBuff = vals.Stone;
                                break;
                            case Flooring.ghost:
                                (who as Farmer).temporarySpeedBuff = vals.Ghost;
                                break;
                            case Flooring.iceTile:
                                (who as Farmer).temporarySpeedBuff = vals.IceTile;
                                break;
                            case Flooring.straw:
                                (who as Farmer).temporarySpeedBuff = vals.Straw;
                                break;
                            case Flooring.gravel:
                                (who as Farmer).temporarySpeedBuff = vals.Gravel;
                                break;
                            case Flooring.boardwalk:
                                (who as Farmer).temporarySpeedBuff = vals.Boardwalk;
                                break;
                            case Flooring.colored_cobblestone:
                                (who as Farmer).temporarySpeedBuff = vals.ColoredCobblestone;
                                break;
                            case Flooring.cobblestone:
                                (who as Farmer).temporarySpeedBuff = vals.Cobblestone;
                                break;
                            case Flooring.steppingStone:
                                (who as Farmer).temporarySpeedBuff = vals.SteppingStone;
                                break;
                            case Flooring.brick:
                                (who as Farmer).temporarySpeedBuff = vals.Brick;
                                break;
                            case Flooring.plankFlooring:
                                (who as Farmer).temporarySpeedBuff = vals.PlankFlooring;
                                break;
                            case Flooring.townFlooring:
                                (who as Farmer).temporarySpeedBuff = vals.TownFlooring;
                                break;
                            default:
                                // Future-proofing in case more paths are added in the future
                                (who as Farmer).temporarySpeedBuff = Config.DefaultPathSpeedBuff;
                                break;
                        }
                    }
                    else
                    {
                        (who as Farmer).temporarySpeedBuff = Config.DefaultPathSpeedBuff;
                    }
                }
            }
            catch (Exception e)
            {
                Monitor.Log($"Failed in {nameof(DoCollisionAction_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}