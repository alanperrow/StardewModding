using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SplitscreenImproved.Layout;
using SplitscreenImproved.MusicFix;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace SplitscreenImproved
{
    [HarmonyPatch(typeof(Game1))]
    public class Game1Patches
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Game1.SetWindowSize))]
        public static IEnumerable<CodeInstruction> SetWindowSize_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo getScreenSplitMethod = AccessTools.DeclaredMethod(typeof(Game1Patches), nameof(GetScreenSplit));

            bool patchApplied = false;
            List<CodeInstruction> instructionsList = instructions.ToList();
            for (int i = 0; i < instructionsList.Count; i++)
            {
                // Find instruction for `Vector4 current_screen_split = screen_splits[Game1.game1.instanceIndex];`
                // IL_0316 (instructionsList[?])
                if (!patchApplied
                    && i > 3 && i < instructionsList.Count
                    && instructionsList[i - 4].opcode == OpCodes.Ldloc_1
                    && instructionsList[i - 3].opcode == OpCodes.Ldsfld
                    && instructionsList[i - 2].opcode == OpCodes.Ldfld
                    && instructionsList[i - 1].opcode == OpCodes.Callvirt
                    && instructionsList[i].opcode == OpCodes.Stloc_2)
                {
                    // NOTE: At this point, we have the result of `screen_splits[Game1.game1.instanceIndex]` on the eval stack.
                    yield return new CodeInstruction(OpCodes.Call, getScreenSplitMethod); // call helper method `GetScreenSplit`

                    patchApplied = true;
                }

                yield return instructionsList[i];
            }

            if (!patchApplied)
            {
                ModEntry.Instance.Monitor.Log(
                    $"{nameof(Game1Patches)}.{nameof(SetWindowSize_Transpiler)} could not find target instruction(s) in {nameof(Game1.SetWindowSize)}, so no changes were made.",
                    LogLevel.Error);
            }

            yield break;
        }


        /// <summary>
        /// Calculates the custom screen split for the current game instance, dependent on the current layout.
        /// </summary>
        /// <param name="originalScreenSplit">The original screen split.</param>
        /// <returns>The screen split.</returns>
        private static Vector4 GetScreenSplit(Vector4 originalScreenSplit)
        {
            if (!ModEntry.Config.IsModEnabled
                || !ModEntry.Config.LayoutFeature.IsFeatureEnabled)
            {
                return originalScreenSplit;
            }

            SplitscreenLayout currentLayout = ModEntry.Config.LayoutFeature.GetSplitscreenLayoutByPreset(ModEntry.Config.LayoutFeature.PresetChoice);
            return currentLayout.GetScreenSplits(GameRunner.instance.gameInstances.Count)[Game1.game1.instanceIndex];
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Game1.isMusicContextActiveButNotPlaying))]
        public static void IsMusicContextActiveButNotPlaying_Postfix(ref bool __result, MusicContext music_context)
        {
            if (!ModEntry.Config.IsModEnabled
                || !ModEntry.Config.MusicFixFeature.IsFeatureEnabled)
            {
                return;
            }

            try
            {
                if (Game1.game1.IsMainInstance)
                {
                    return;
                }

                bool newResult = MusicFixHelper.IsMusicContextActiveButNotPlaying(music_context);
                if (newResult != __result)
                {
                    __result = newResult;
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(IsMusicContextActiveButNotPlaying_Postfix)}:\n{e}", LogLevel.Error);
            }
        }


    }
}
