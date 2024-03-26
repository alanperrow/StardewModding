using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace BetterSplitscreen
{
    [HarmonyPatch(typeof(Game1))]
    public class Game1Patches
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Game1.SetWindowSize))]
        [HarmonyPatch(new Type[] { typeof(int), typeof(int) })]
        public static IEnumerable<CodeInstruction> SetWindowSize_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo getScreenSplitsMethod = AccessTools.Method(typeof(ModLogic), nameof(ModLogic.GetScreenSplits));
            //MethodInfo getScreenSplitsMethod = AccessTools.Method(typeof(ModLogic), nameof(DebugLog1));

            List<CodeInstruction> instructionsList = instructions.ToList();

            bool foundInstruction = false;

            for (int i = 0; i < instructionsList.Count; i++)
            {
                // Find instruction: `if (GameRunner.instance.gameInstances.Count <= 1)`
                // IL_02dc (instructionsList[182])
                if (i > 0 && i < instructionsList.Count - 1
                    && instructionsList[i - 1].opcode == OpCodes.Callvirt // && operand is method {Void Add(Microsoft.Xna.Framework.Vector4}
                    && instructionsList[i].opcode == OpCodes.Ldsfld
                    && instructionsList[i + 1].opcode == OpCodes.Ldfld)
                {
                    //yield return new CodeInstruction(OpCodes.Ldloc_1); // load local List<Vector4> `screen_splits`

                    yield return new CodeInstruction(OpCodes.Call, getScreenSplitsMethod);  // call GetScreenSplits()
                    yield return new CodeInstruction(OpCodes.Stloc_1);                      // store result in local List<Vector4> `screen_splits` (this overwrites base game values)

                    foundInstruction = true;
                }

                yield return instructionsList[i];
            }

            if (!foundInstruction)
            {
                ModEntry.Instance.Monitor.Log(
                    $"{nameof(SetWindowSize_Transpiler)} could not find target instruction(s) in {nameof(Game1.SetWindowSize)}, so no changes were made.",
                    LogLevel.Error);
            }

            yield break;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Game1.SetWindowSize))]
        public static bool SetWindowSize_Prefix(Game1 __instance, int w, int h)
        {

            var window = __instance.localMultiplayerWindow;
            var player = Game1.player;

            try
            {
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(SetWindowSize_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Game1.SetWindowSize))]
        public static void SetWindowSize_Postfix(Game1 __instance, int w, int h)
        {
            var window = __instance.localMultiplayerWindow;
            var player = Game1.player;

            try
            {
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(SetWindowSize_Postfix)}:\n{e}", LogLevel.Error);
            }
        }




        // TODO: Music bug - still investigating
        /*
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Game1.changeMusicTrack))]
        public static bool ChangeMusicTrack_Prefix(string newTrackName, bool track_interruptable, MusicContext music_context)
        {
            //var debug_Game1 = Game1.game1;

            //var sf = new StackFrame();
            //StackTrace st = new StackTrace(1, true);
            //StackFrame stFrame1 = st.GetFrame(1); // prev frame
            //string invkMethodName = stFrame1.GetMethod().Name;
            string invkMethodName = string.Empty;

            string pref1 = Game1.player.IsMainPlayer ? "P1: " : "P2: ";

            ModEntry.Instance.Monitor.Log(
                //$"newTrackName={newTrackName ?? "NULL"}\t\ttrack_interruptable={track_interruptable}\t\tmusic_context={music_context}\t\t(tick={Game1.ticks})",
                 pref1 + $"{newTrackName ?? "NULL"}\t{track_interruptable}\t{music_context}\t({Game1.ticks})\t{invkMethodName}",
                LogLevel.Debug);

            try
            {
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ChangeMusicTrack_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Game1.changeMusicTrack))]
        public static void ChangeMusicTrack_Postfix(string newTrackName, bool track_interruptable, MusicContext music_context)
        {
            //var debug_Game1 = Game1.game1;
            //ModEntry.Instance.Monitor.Log(
            //    $"newTrackName={newTrackName ?? "NULL"}\t\ttrack_interruptable={track_interruptable}\t\tmusic_context={music_context}",
            //    LogLevel.Debug);

            try
            {
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ChangeMusicTrack_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
        */
    }
}
