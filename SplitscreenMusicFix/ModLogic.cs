using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace BetterSplitscreen
{
    public static class ModLogic
    {
        /// <summary>
        /// Modified version of <see cref="GameRunner.ExecuteForInstances"/> for calling <see cref="Game1.SetWindowSize"/> to avoid compiler inlining.
        /// </summary>
        public static void SetWindowSizeForInstances(Game1 game1)
        {
            // See comment in WindowClientSizeChanged_Transpiler about overwriting `w` and `h`.
            int w = Game1.graphics.IsFullScreen ? Game1.graphics.PreferredBackBufferWidth : game1.Window.ClientBounds.Width;
            int h = Game1.graphics.IsFullScreen ? Game1.graphics.PreferredBackBufferHeight : game1.Window.ClientBounds.Height;

            Game1 old_game1 = Game1.game1;
            if (old_game1 != null)
            {
                GameRunner.SaveInstance(old_game1);
            }
            foreach (Game1 instance in GameRunner.instance.gameInstances)
            {
                GameRunner.LoadInstance(instance);
                instance.SetWindowSize(w, h);
                GameRunner.SaveInstance(instance);
            }
            if (old_game1 != null)
            {
                GameRunner.LoadInstance(old_game1);
            }
            else
            {
                Game1.game1 = null;
            }
        }

        /// <summary>
        /// Calculates the custom screen split locations for each splitscreen player
        /// [TODO: , dependent on the selected layout from ModConfig]
        /// , and overwrites the values in the input list.
        /// </summary>
        /// <param name="screenSplits">Original instance.</param>
        /// <returns>The overwritten list of screen split locations.</returns>
        public static void OverwriteScreenSplits(List<Vector4> screenSplits)
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
