using StardewModdingAPI;
using System;
using StardewValley;

namespace FasterPathSpeed
{
    public class FarmerPatches
    {
        public static void GetMovementSpeed_Postfix(Farmer __instance, ref float __result)
        {
            try
            {
                FasterPathSpeed.GetFarmerMovementSpeed(__instance, ref __result);
            }
            catch (Exception e)
            {
                ModEntry.Context.Monitor.Log($"Failed in {nameof(GetMovementSpeed_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
