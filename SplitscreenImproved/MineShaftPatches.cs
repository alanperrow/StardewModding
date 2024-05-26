using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;

namespace SplitscreenImproved
{
    // TODO: Separate mod: Mine Music Fix.

    [HarmonyPatch(typeof(MineShaft))]
    public class MineShaftPatches
    {
        /*
        [HarmonyPrefix]
        [HarmonyPatch(nameof(MineShaft.checkForMusic))]
        public static bool CheckForMusic_Prefix(MineShaft __instance, GameTime time)
        {
            try
            {
                // DEBUG: Base game logic
                if (Game1.player.freezePause <= 0 && !__instance.isFogUp && __instance.mineLevel != 120)
                {
                    string trackName = null;
                    switch (__instance.getMineArea())
                    {
                        case 0:
                        case 10:
                        case 121:
                        case 77377:
                            trackName = "Upper_Ambient";
                            break;
                        case 40:
                            trackName = "Frost_Ambient";
                            break;
                        case 80:
                            trackName = "Lava_Ambient";
                            break;
                    }
                    if (__instance.GetAdditionalDifficulty() > 0 && __instance.getMineArea() == 40 && __instance.mineLevel < 70)
                    {
                        trackName = "jungle_ambience";
                    }
                    if (Game1.getMusicTrackName() == "none" || Game1.isMusicContextActiveButNotPlaying() || (Game1.getMusicTrackName().EndsWith("_Ambient") && Game1.getMusicTrackName() != trackName))
                    {
                        //Game1.changeMusicTrack(trackName);
                    }
                    MineShaft.timeSinceLastMusic = Math.Min(335000, MineShaft.timeSinceLastMusic + time.ElapsedGameTime.Milliseconds);
                }

            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(CheckForMusic_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("resetLocalState")]
        public static bool ResetLocalState_Prefix(MineShaft __instance)
        {
            try
            {
                // DEBUG: Base game logic
                if (Game1.IsPlayingBackgroundMusic)
                {
                    //Game1.changeMusicTrack("none");
                }

                int a_1 = __instance.getMineArea(__instance.mineLevel);
                int a_2 = __instance.getMineArea(__instance.mineLevel - 1);
                bool b = __instance.mineLevel == 120;
                bool c = __instance.isPlayingSongFromDifferentArea();
                if (a_1 != a_2 || b || c)
                {
                    // DEBUG:
                    //MineShaft.timeSinceLastMusic = 150001;

                    //Game1.changeMusicTrack("none");
                }

                //if (Game1.getMusicTrackName() == "none" && __instance.mineLevel > 120)
                //{
                //    __instance.playMineSong();
                //}

                //// Fix for skull cavern music rarely playing.
                //if (__instance.mineLevel > 120 && (__instance.mineLevel == 121 || (MineShaft.timeSinceLastMusic > 150000 && Game1.random.NextBool())))
                //{
                //    __instance.playMineSong();
                //}
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ResetLocalState_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }
        */
    }
}
