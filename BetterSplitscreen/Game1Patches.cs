using System;
using System.Reflection;
using HarmonyLib;
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
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Game1.SetWindowSize))]
        public static bool SetWindowSize_Prefix(Game1 __instance, int w, int h)
        {
            if (!ModEntry.Config.IsModEnabled
                || !ModEntry.Config.LayoutFeature.IsFeatureEnabled)
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

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Game1.UpdateRequestedMusicTrack))]
        public static void UpdateRequestedMusicTrack_Postfix()
        {
            try
            {
                MusicFixHelper.SetActiveMusicContextForImportantSplitScreenMusic();
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(UpdateRequestedMusicTrack_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
        /*
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Game1.changeMusicTrack))]
        public static bool ChangeMusicTrack_Prefix(string newTrackName, bool track_interruptable, ref MusicContext music_context)
        {
            if (!ModEntry.Config.IsModEnabled
                || !ModEntry.Config.MusicFixFeature.IsFeatureEnabled
                || GameRunner.instance.gameInstances.Count == 1)
            {
                return true;
            }

            try
            {
                return MusicFixHelper.OnChangeMusicTrack(newTrackName, track_interruptable, ref music_context);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ChangeMusicTrack_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }
        */
        /*
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
