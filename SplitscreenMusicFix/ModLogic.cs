﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace BetterSplitscreen
{
    public static class ModLogic
    {
        /// <summary>
        /// Calculates the custom screen split locations for each game instance.
        /// [TODO: , dependent on the selected layout from ModConfig]
        /// </summary>
        /// <param name="screenSplits">Original instance.</param>
        /// <returns>The list of screen split locations.</returns>
        public static List<Vector4> GetScreenSplits()
        {
            List<Vector4> screenSplits = new();

            // TODO: Conditional logic based on ModConfig.
            // IDEA: Make a nice pretty graphic with red/blue/green/yellow boxes representing each individual splitscreen position.
            bool isDefaultLayout = false;

            ModEntry.Instance.Monitor.Log($"{nameof(GetScreenSplits)} hit", LogLevel.Debug);

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

            return screenSplits;
        }

        /// <summary>
        /// Modified version of <see cref="Game1.SetWindowSize"/> as the original method gets inlined by the compiler and cannot be modified.
        /// Definitely not ideal, but there is no way to transpile an inlined method, so this is necessary to apply custom mod logic.
        /// </summary>
        /// <remarks>
        /// Avoid making changes to this method, as it is almost entirely copy-pasted from the decompiled game source code.
        /// </remarks>
        public static void SetWindowSize(Game1 instance, int w, int h)
        {
            Microsoft.Xna.Framework.Rectangle oldWindow = new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height);
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
                    instance.localMultiplayerWindow = new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
                }
                else
                {
                    instance.localMultiplayerWindow = new Microsoft.Xna.Framework.Rectangle(0, 0, w, h);
                }
            }
            catch (Exception)
            {
            }
            Game1.defaultDeviceViewport = new Viewport(instance.localMultiplayerWindow);

            // Replaced base game code for `screen_splits` to instead call custom `GetScreenSplits` method.
            List<Vector4> screen_splits = GetScreenSplits();

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
            instance.localMultiplayerWindow.X = (int)((float)w * current_screen_split.X);
            instance.localMultiplayerWindow.Y = (int)((float)h * current_screen_split.Y);
            instance.localMultiplayerWindow.Width = (int)Math.Ceiling((float)w * current_screen_split.Z);
            instance.localMultiplayerWindow.Height = (int)Math.Ceiling((float)h * current_screen_split.W);
            try
            {
                int sw = (int)Math.Ceiling((float)instance.localMultiplayerWindow.Width * (1f / Game1.options.zoomLevel));
                int sh = (int)Math.Ceiling((float)instance.localMultiplayerWindow.Height * (1f / Game1.options.zoomLevel));
                instance.screen = new RenderTarget2D(Game1.graphics.GraphicsDevice, sw, sh, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                instance.screen.Name = "Screen";
                int uw = (int)Math.Ceiling((float)instance.localMultiplayerWindow.Width / Game1.options.uiScale);
                int uh = (int)Math.Ceiling((float)instance.localMultiplayerWindow.Height / Game1.options.uiScale);
                instance.uiScreen = new RenderTarget2D(Game1.graphics.GraphicsDevice, uw, uh, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                instance.uiScreen.Name = "UI Screen";
            }
            catch (Exception)
            {
            }
            Game1.updateViewportForScreenSizeChange(fullscreenChange: false, instance.localMultiplayerWindow.Width, instance.localMultiplayerWindow.Height);
            if (old_ui_dimensions.HasValue && old_ui_dimensions.Value.X == (float)instance.uiScreen.Width && old_ui_dimensions.Value.Y == (float)instance.uiScreen.Height)
            {
                return;
            }
            Game1.PushUIMode();
            Game1.textEntry?.gameWindowSizeChanged(oldWindow, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
            foreach (IClickableMenu onScreenMenu in Game1.onScreenMenus)
            {
                onScreenMenu.gameWindowSizeChanged(oldWindow, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
            }
            Game1.currentMinigame?.changeScreenSize();
            Game1.activeClickableMenu?.gameWindowSizeChanged(oldWindow, new Microsoft.Xna.Framework.Rectangle(Game1.viewport.X, Game1.viewport.Y, Game1.viewport.Width, Game1.viewport.Height));
            GameMenu gameMenu = Game1.activeClickableMenu as GameMenu;
            if (gameMenu != null)
            {
                (gameMenu.GetCurrentPage() as OptionsPage)?.preWindowSizeChange();
                gameMenu = (GameMenu)(Game1.activeClickableMenu = new GameMenu(gameMenu.currentTab));
                (gameMenu.GetCurrentPage() as OptionsPage)?.postWindowSizeChange();
            }
            Game1.PopUIMode();
        }
    }
}
