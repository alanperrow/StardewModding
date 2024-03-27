using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BetterSplitscreen.Layout
{
    internal static class LayoutManager
    {
        /// <summary>
        /// Modified version of <see cref="Game1.SetWindowSize"/> as the original method gets inlined by the compiler and cannot be modified.
        /// Sets the window size upon being changed, and if the game is local multiplayer, applies a splitscreen layout via <see cref="GetScreenSplits"/>.
        /// </summary>
        /// <remarks>Avoid making changes to this method, as it is almost entirely copy-pasted from the decompiled game source code.</remarks>
        public static void SetWindowSize(Game1 instance, int w, int h)
        {
            Rectangle oldWindow = new Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (w < 1280 && !Game1.graphics.IsFullScreen)
                {
                    w = 1280;
                }
                if (h < 720 && !Game1.graphics.IsFullScreen)
                {
                    h = 720;
                }
            }
            if (!Game1.graphics.IsFullScreen && instance.Window.AllowUserResizing)
            {
                Game1.graphics.PreferredBackBufferWidth = w;
                Game1.graphics.PreferredBackBufferHeight = h;
            }
            if (instance.IsMainInstance && Game1.graphics.SynchronizeWithVerticalRetrace != Game1.options.vsyncEnabled)
            {
                Game1.graphics.SynchronizeWithVerticalRetrace = Game1.options.vsyncEnabled;

                // TODO: Get `log` FieldInfo via Reflection and cache the result, then we should be able to access the field.
                //       Performance hit must be assessed to see if this is worth it.
                //       Commented out for now.

                // Game1.log.Verbose("Vsync toggled: " + Game1.graphics.SynchronizeWithVerticalRetrace);
            }
            Game1.graphics.ApplyChanges();
            try
            {
                if (Game1.graphics.IsFullScreen)
                {
                    instance.localMultiplayerWindow = new Rectangle(0, 0, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
                }
                else
                {
                    instance.localMultiplayerWindow = new Rectangle(0, 0, w, h);
                }
            }
            catch (Exception)
            {
            }
            Game1.defaultDeviceViewport = new Viewport(instance.localMultiplayerWindow);

            // Replaced base game code for `screen_splits` to instead call custom `GetScreenSplits` method.
            Vector4[] screen_splits = GetScreenSplits(GameRunner.instance.gameInstances.Count);

            if (GameRunner.instance.gameInstances.Count <= 1)
            {
                instance.zoomModifier = 1f;
            }
            else
            {
                instance.zoomModifier = 0.5f;
            }
            Vector4 current_screen_split = screen_splits[Game1.game1.instanceIndex];
            Vector2? old_ui_dimensions = null;
            if (instance.uiScreen != null)
            {
                old_ui_dimensions = new Vector2(instance.uiScreen.Width, instance.uiScreen.Height);
            }
            instance.localMultiplayerWindow.X = (int)(w * current_screen_split.X);
            instance.localMultiplayerWindow.Y = (int)(h * current_screen_split.Y);
            instance.localMultiplayerWindow.Width = (int)Math.Ceiling(w * current_screen_split.Z);
            instance.localMultiplayerWindow.Height = (int)Math.Ceiling(h * current_screen_split.W);
            try
            {
                int sw = (int)Math.Ceiling(instance.localMultiplayerWindow.Width * (1f / Game1.options.zoomLevel));
                int sh = (int)Math.Ceiling(instance.localMultiplayerWindow.Height * (1f / Game1.options.zoomLevel));
                instance.screen = new RenderTarget2D(Game1.graphics.GraphicsDevice, sw, sh, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                instance.screen.Name = "Screen";
                int uw = (int)Math.Ceiling(instance.localMultiplayerWindow.Width / Game1.options.uiScale);
                int uh = (int)Math.Ceiling(instance.localMultiplayerWindow.Height / Game1.options.uiScale);
                instance.uiScreen = new RenderTarget2D(Game1.graphics.GraphicsDevice, uw, uh, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                instance.uiScreen.Name = "UI Screen";
            }
            catch (Exception)
            {
            }
            Game1.updateViewportForScreenSizeChange(fullscreenChange: false, instance.localMultiplayerWindow.Width, instance.localMultiplayerWindow.Height);
            if (old_ui_dimensions.HasValue && old_ui_dimensions.Value.X == instance.uiScreen.Width && old_ui_dimensions.Value.Y == instance.uiScreen.Height)
            {
                return;
            }
            Game1.PushUIMode();
            Game1.textEntry?.gameWindowSizeChanged(oldWindow, new Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
            foreach (IClickableMenu onScreenMenu in Game1.onScreenMenus)
            {
                onScreenMenu.gameWindowSizeChanged(oldWindow, new Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
            }
            Game1.currentMinigame?.changeScreenSize();
            Game1.activeClickableMenu?.gameWindowSizeChanged(oldWindow, new Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
            GameMenu gameMenu = Game1.activeClickableMenu as GameMenu;
            if (gameMenu != null)
            {
                (gameMenu.GetCurrentPage() as OptionsPage)?.preWindowSizeChange();
                gameMenu = (GameMenu)(Game1.activeClickableMenu = new GameMenu(gameMenu.currentTab));
                (gameMenu.GetCurrentPage() as OptionsPage)?.postWindowSizeChange();
            }
            Game1.PopUIMode();
        }
        
        /// <summary>
        /// Calculates the custom screen split locations for each game instance.
        /// [TODO: , dependent on the selected layout from ModConfig]
        /// </summary>
        /// <param name="screenSplits">Original instance.</param>
        /// <returns>The array of screen split locations.</returns>
        private static Vector4[] GetScreenSplits(int numScreens)
        {
            ModEntry.Instance.Monitor.Log($"{nameof(GetScreenSplits)} hit", LogLevel.Debug);

            return ModEntry.Config.LayoutFeature.CurrentLayout.GetScreenSplits(numScreens);
        }
    }
}
