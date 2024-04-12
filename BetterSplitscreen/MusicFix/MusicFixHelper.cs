using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SplitscreenImproved.MusicFix
{
    internal class MusicFixHelper
    {
        internal static void DrawDebugText(SpriteBatch sb)
        {
            Game1 thisInstance = Game1.game1;

            string playerNum = $"P{thisInstance.instanceIndex + 1}: ";
            string agg = string.Empty;
            foreach (MusicContext musicContext in Enum.GetValues<MusicContext>())
            {
                string key = Game1.getMusicTrackName(musicContext);
                agg += playerNum + musicContext.ToString() + ": " + key + "\n";
            }

            agg += "--------------\n";

            MusicContext activeMusicContext = (MusicContext)instanceActiveMusicContextField.GetValue(thisInstance);
            agg += playerNum + "ActiveMusicContext: " + activeMusicContext.ToString() + "\n";

            agg += playerNum + "IsMusicContextActiveButNotPlaying(): " + IsMusicContextActiveButNotPlaying(MusicContext.Default).ToString() + "\n";

            Game1 mainInstance = GameRunner.instance.gameInstances.Single(x => x.IsMainInstance);
            string mainCurrentTrack = GetMusicTrackNameOfInstance(mainInstance, MusicContext.Default);
            agg += playerNum + "Main music track: " + mainCurrentTrack + "\n";
            string mainCurrentTrackSplitscreen = GetMusicTrackNameOfInstance(mainInstance, MusicContext.ImportantSplitScreenMusic);
            agg += playerNum + "Main splitscreen music track: " + mainCurrentTrackSplitscreen + "\n";

            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(4, 2), Microsoft.Xna.Framework.Color.Black);
            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(4, 6), Microsoft.Xna.Framework.Color.Black);
            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(2, 4), Microsoft.Xna.Framework.Color.Black);
            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(6, 4), Microsoft.Xna.Framework.Color.Black);
            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(4, 4), Microsoft.Xna.Framework.Color.White);
        }

        private static readonly FieldInfo instanceActiveMusicContextField = AccessTools.DeclaredField(typeof(Game1), "_instanceActiveMusicContext");
        internal static bool IsMusicContextActiveButNotPlaying(MusicContext music_context)
        {
            Game1 thisInstance = Game1.game1;
            MusicContext this_activeMusicContext = (MusicContext)instanceActiveMusicContextField.GetValue(thisInstance);

            Game1 mainInstance = GameRunner.instance.gameInstances.Single(x => x.IsMainInstance);

            // Check if this is not main instance, and if so, perform separate logic.
            if (!thisInstance.IsMainInstance)
            {
                if (this_activeMusicContext != music_context)
                {
                    return false;
                }
                if (Game1.morningSongPlayAction != null)
                {
                    return false;
                }

                //DEBUG
                string thisCurrentTrack = Game1.getMusicTrackName(music_context);
                if (thisCurrentTrack != "none")
                {

                }
                //DEBUG

                string mainCurrentTrack = GetMusicTrackNameOfInstance(mainInstance, music_context);
                if (mainCurrentTrack == "none")
                {
                    mainCurrentTrack = GetMusicTrackNameOfInstance(mainInstance, music_context);
                    if (mainCurrentTrack == "none")
                    {
                        return true;
                    }
                }
                if (Game1.currentSong != null && Game1.currentSong.Name == mainCurrentTrack && !Game1.currentSong.IsPlaying)
                {
                    return true;
                }
                return false;
            }
            // DEBUG

            // Base game logic.
            if (this_activeMusicContext != music_context)
            {
                return false;
            }
            if (Game1.morningSongPlayAction != null)
            {
                return false;
            }
            string currentTrack = Game1.getMusicTrackName(music_context);
            if (currentTrack == "none")
            {
                return true;
            }
            if (Game1.currentSong != null && Game1.currentSong.Name == currentTrack && !Game1.currentSong.IsPlaying)
            {
                return true;
            }
            return false;
        }

        // Refactored base game method to include a Game1 instance parameter.
        private static readonly FieldInfo instanceRequestedMusicTracksField = AccessTools.DeclaredField(typeof(Game1), "_instanceRequestedMusicTracks");
        public static string GetMusicTrackNameOfInstance(Game1 instance, MusicContext music_context = MusicContext.Default)
        {
            var requestedMusicTracks = (Dictionary<MusicContext, KeyValuePair<string, bool>>)instanceRequestedMusicTracksField.GetValue(instance);

            if (requestedMusicTracks.TryGetValue(music_context, out var value))
            {
                return value.Key;
            }

            if (music_context == MusicContext.Default)
            {
                return GetMusicTrackNameOfInstance(instance, MusicContext.SubLocation);
            }

            return "none";
        }

        /*

        internal static void SetActiveMusicContextForImportantSplitScreenMusic()
        {
            if (Game1.game1.instanceIndex == 0)
            {
                // Ignore main player instance.
                return;
            }

            Game1 instance = Game1.game1;

            // DEBUG - temp disable this

            //// Replicated base game logic.
            //// Determines if the requested music track was at a context level of MusicContext.ImportantSplitScreenMusic.
            //// Normally, base game does not set active music context in this case; let's do it anyway.
            //var requestedMusicTracks = (Dictionary<MusicContext, KeyValuePair<string, bool>>)requestedMusicTracksField.GetValue(instance);
            //if (requestedMusicTracks.TryGetValue(MusicContext.ImportantSplitScreenMusic, out _))
            //{
            //    activeMusicContextField.SetValue(Game1.game1, MusicContext.ImportantSplitScreenMusic);
            //}
        }

        internal static bool OnChangeMusicTrack(string newTrackName, bool track_interruptable, ref MusicContext music_context)
        {
            if (Game1.game1.instanceIndex == 0)
            {
                // player1

                if (music_context == MusicContext.ImportantSplitScreenMusic)
                {
                }
            }
            else
            {

            }

            return true;

            string playerNum = $"P{Game1.game1.instanceIndex + 1}: ";
            ModEntry.Instance.Monitor.Log(
                 playerNum + $"\t{newTrackName ?? "NULL"}\t{track_interruptable}\t{music_context}\t({Game1.ticks})",
                LogLevel.Debug);
            string agg = string.Empty;
            foreach (MusicContext musicContext in Enum.GetValues<MusicContext>())
            {
                string key = Game1.getMusicTrackName(musicContext);
                agg += "\n" + playerNum + $"{musicContext}: " + key;
            }
            ModEntry.Instance.Monitor.Log(agg + "\n", LogLevel.Debug);

            // TODO: Check for config PrecedentPlayer
            if (Game1.player.IsMainPlayer)
            {
                return true;
            }

            if (music_context == MusicContext.Default)
            {
                //music_context = MusicContext.SubLocation;
            }

            return true;
        }
        */
    }
}
