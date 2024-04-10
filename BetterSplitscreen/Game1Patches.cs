using System;
using HarmonyLib;
using SplitscreenImproved.Layout;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace SplitscreenImproved
{
    [HarmonyPatch(typeof(Game1))]
    public class Game1Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Game1.SetWindowSize))]
        public static bool SetWindowSize_Prefix(Game1 __instance, int w, int h)
        {
            if (!ModEntry.Config.IsModEnabled)
            {
                return true;
            }

            if (!ModEntry.Config.LayoutFeature.IsFeatureEnabled)
            {
                return true;
            }

            try
            {
                // Replace base game method call.
                LayoutManager.SetWindowSize(__instance, w, h);
                return false;
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(SetWindowSize_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        // TODO: Music bug - still investigating
        ///*
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
        //*/
    }
}
