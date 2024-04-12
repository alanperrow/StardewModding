using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SplitscreenImproved.MusicFix
{
    internal class MusicFixHelper
    {
        private static readonly FieldInfo activeMusicContextField = AccessTools.DeclaredField(typeof(Game1), "_instanceActiveMusicContext");
        private static readonly FieldInfo requestedMusicTracksField = AccessTools.DeclaredField(typeof(Game1), "_instanceRequestedMusicTracks");

        internal static void SetActiveMusicContextForImportantSplitScreenMusic()
        {
            if (Game1.game1.instanceIndex == 0)
            {
                // Ignore main player instance.
                return;
            }

            Game1 instance = Game1.game1;

            // DEBUG - temp disable this
            /*

            // Replicated base game logic.
            // Determines if the requested music track was at a context level of MusicContext.ImportantSplitScreenMusic.
            // Normally, base game does not set active music context in this case; let's do it anyway.
            var requestedMusicTracks = (Dictionary<MusicContext, KeyValuePair<string, bool>>)requestedMusicTracksField.GetValue(instance);
            if (requestedMusicTracks.TryGetValue(MusicContext.ImportantSplitScreenMusic, out _))
            {
                activeMusicContextField.SetValue(Game1.game1, MusicContext.ImportantSplitScreenMusic);
            }
            */
        }

        internal static void DrawDebugText(SpriteBatch sb)
        {
            string playerNum = $"P{Game1.game1.instanceIndex + 1}: ";
            string agg = string.Empty;
            foreach (MusicContext musicContext in Enum.GetValues<MusicContext>())
            {
                string key = Game1.getMusicTrackName(musicContext);
                agg += playerNum + musicContext.ToString() + ": " + key + "\n";
            }

            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(4, 6), Microsoft.Xna.Framework.Color.Black);
            sb.DrawString(Game1.smallFont, agg, new Microsoft.Xna.Framework.Vector2(4, 4), Microsoft.Xna.Framework.Color.White);
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
    }
}
