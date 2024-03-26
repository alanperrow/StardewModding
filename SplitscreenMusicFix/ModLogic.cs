using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace BetterSplitscreen
{
    public static class ModLogic
    {
        /// <summary>
        /// Calculates the screen split locations for each splitscreen player.
        /// TODO: , dependent on the selected layout from ModConfig
        /// , and overwrites the values in the input list.
        /// </summary>
        /// <param name="screenSplits">Original instance.</param>
        /// <returns>The list of screen split locations.</returns>
        public static void SetScreenSplits(List<Vector4> screenSplits)
        {
            // TODO: Conditional logic based on ModConfig.
            // IDEA: Make a nice pretty graphic with red/blue/green/yellow boxes representing each individual splitscreen position.
            bool isDefaultLayout = false;

            screenSplits.Clear();

            ModEntry.Instance.Monitor.Log("SetScreenSplits hit", LogLevel.Debug);

            if (isDefaultLayout)
            {
                // DEFAULT GAME LAYOUT
                if (GameRunner.instance.gameInstances.Count <= 1)
                {
                    screenSplits.Add(new Vector4(0f, 0f, 1f, 1f));
                }
                else
                {
                    switch (GameRunner.instance.gameInstances.Count)
                    {
                        case 2:
                            screenSplits.Add(new Vector4(0f, 0f, 0.5f, 1f));
                            screenSplits.Add(new Vector4(0.5f, 0f, 0.5f, 1f));
                            break;
                        case 3:
                            screenSplits.Add(new Vector4(0f, 0f, 1f, 0.5f));
                            screenSplits.Add(new Vector4(0f, 0.5f, 0.5f, 0.5f));
                            screenSplits.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
                            break;
                        case 4:
                            screenSplits.Add(new Vector4(0f, 0f, 0.5f, 0.5f));
                            screenSplits.Add(new Vector4(0.5f, 0f, 0.5f, 0.5f));
                            screenSplits.Add(new Vector4(0f, 0.5f, 0.5f, 0.5f));
                            screenSplits.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
                            break;
                    }
                }
            }
            else
            {
                // CUSTOM LAYOUT
                if (GameRunner.instance.gameInstances.Count <= 1)
                {
                    screenSplits.Add(new Vector4(0f, 0f, 1f, 1f));
                }
                else
                {
                    switch (GameRunner.instance.gameInstances.Count)
                    {
                        case 2:
                            screenSplits.Add(new Vector4(0.5f, 0f, 0.5f, 1f));
                            screenSplits.Add(new Vector4(0f, 0f, 0.5f, 1f));
                            break;
                        case 3:
                            screenSplits.Add(new Vector4(0f, 0.5f, 0.5f, 0.5f));
                            screenSplits.Add(new Vector4(0f, 0f, 1f, 0.5f));
                            screenSplits.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
                            break;
                        case 4:
                            screenSplits.Add(new Vector4(0.5f, 0f, 0.5f, 0.5f));
                            screenSplits.Add(new Vector4(0f, 0f, 0.5f, 0.5f));
                            screenSplits.Add(new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
                            screenSplits.Add(new Vector4(0f, 0.5f, 0.5f, 0.5f));
                            break;
                    }
                }
            }
        }
    }
}
