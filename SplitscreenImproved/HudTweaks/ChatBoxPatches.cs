using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Menus;

namespace SplitscreenImproved.HudTweaks
{
    [HarmonyPatch(typeof(ChatBox))]
    public class ChatBoxPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ChatBox.draw))]
        public static bool Draw_Prefix(ChatBox __instance)
        {
            try
            {
                if (HudTweaksHelper.HasToolbarPositionChanged())
                {
                    HudTweaksHelper.OffsetChatBoxFromToolbar(__instance);
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Draw_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("updatePosition")]
        public static void UpdatePosition_Postfix(ChatBox __instance)
        {
            try
            {
                HudTweaksHelper.OffsetChatBoxFromToolbar(__instance);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(UpdatePosition_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
