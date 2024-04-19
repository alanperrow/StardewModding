using System;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;

namespace SplitscreenImproved.HudTweaks
{
    [HarmonyPatch(typeof(BuffsDisplay))]
    public class BuffsDisplayPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BuffsDisplay.draw))]
        public static bool Draw_Prefix(BuffsDisplay __instance, SpriteBatch b)
        {
            try
            {
                //if (HudTweaksHelper.HasToolbarPositionChanged())
                //{
                //    HudTweaksHelper.OffsetBuffsDisplayFromToolbar(__instance);
                //}
                
                HudTweaksHelper.OffsetBuffsDisplayFromToolbar(__instance, b);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Draw_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("updatePosition")]
        public static void UpdatePosition_Postfix(BuffsDisplay __instance)
        {
            try
            {
                // DEBUG: This is the method that should be used. Only using draw() to have access to a SpriteBatch instance.
                //HudTweaksHelper.OffsetBuffsDisplayFromToolbar(__instance);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(UpdatePosition_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
