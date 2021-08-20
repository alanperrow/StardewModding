using HarmonyLib;
using StardewModdingAPI;
using System;
using StardewValley;

namespace FasterPathSpeed
{
    [HarmonyPatch]
    public class FarmerPatches
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        [HarmonyPatch(typeof(Farmer), nameof(Farmer.getMovementSpeed))]
        public static void GetMovementSpeed_Postfix(Farmer __instance, ref float __result)
        {
            try
            {
                if ((Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence) && !Game1.eventUp && __instance.isRidingHorse())
                {
                    if (!(Game1.CurrentEvent == null && __instance.hasBuff(19)))
                    {
                        // Adapted from original GetMovementSpeed calculation
                        float mult = Config.HorsePathSpeedBuffModifier * __instance.movementMultiplier * Game1.currentGameTime.ElapsedGameTime.Milliseconds;

                        __result += (__instance.movementDirections.Count > 1) ? (0.7f * __instance.temporarySpeedBuff * mult) : (__instance.temporarySpeedBuff * mult);
                    }
                }
                // Don't mess with ELSE for now, don't want to make any unintentional errors dealing with Event movement
            }
            catch (Exception e)
            {
                Monitor.Log($"Failed in {nameof(GetMovementSpeed_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
