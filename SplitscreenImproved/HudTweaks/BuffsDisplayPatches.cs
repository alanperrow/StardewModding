using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Menus;

namespace SplitscreenImproved.HudTweaks
{
    [HarmonyPatch(typeof(BuffsDisplay))]
    public class BuffsDisplayPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("updatePosition")]
        public static void UpdatePosition_Postfix(BuffsDisplay __instance)
        {
            try
            {
                HudTweaksHelper.OffsetBuffsDisplayFromToolbar(__instance);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(UpdatePosition_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
