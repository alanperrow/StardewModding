using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Menus;

namespace SplitscreenImproved.HudTweaks
{
    internal static class HudTweaksHelper
    {
        static readonly PerScreen<int> prevToolbarYPosition = new();
        static readonly PerScreen<bool> isBuffsDisplayPositionChanged = new();

        internal static bool HasToolbarPositionChanged()
        {
            if (!IsEnabled())
            {
                return false;
            }

            Toolbar toolbar = GetToolbar();
            if (toolbar is null)
            {
                return false;
            }

            bool hasChanged = toolbar.yPositionOnScreen != prevToolbarYPosition.Value;

            prevToolbarYPosition.Value = toolbar.yPositionOnScreen;
            return hasChanged;
        }

        internal static void OffsetChatBoxFromToolbar(ChatBox instance)
        {
            if (!IsEnabled() || !ModEntry.Config.HudTweaksFeature.IsToolbarHudOffsetEnabled)
            {
                return;
            }

            Toolbar toolbar = GetToolbar();
            if (toolbar is null)
            {
                return;
            }

            // Base game logic.
            instance.yPositionOnScreen = Game1.uiViewport.Height - instance.chatBox.Height;

            if (!IsToolbarTopAligned(toolbar))
            {
                // Toolbar is at the bottom of the screen, so we offset our chatbox to avoid it obstructing the toolbar.
                instance.yPositionOnScreen -= toolbar.height / 2;
            }

            // Base game logic.
            Utility.makeSafe(ref instance.xPositionOnScreen, ref instance.yPositionOnScreen, instance.chatBox.Width, instance.chatBox.Height);
            instance.chatBox.X = instance.xPositionOnScreen;
            instance.chatBox.Y = instance.yPositionOnScreen;
            instance.chatBoxCC.bounds = new Rectangle(instance.chatBox.X, instance.chatBox.Y, instance.chatBox.Width, instance.chatBox.Height);
            instance.emojiMenuIcon.bounds.Y = instance.chatBox.Y + 8;
            instance.emojiMenuIcon.bounds.X = instance.chatBox.Width - instance.emojiMenuIcon.bounds.Width - 8;
            if (instance.emojiMenu != null)
            {
                instance.emojiMenu.xPositionOnScreen = instance.emojiMenuIcon.bounds.Center.X - 146;
                instance.emojiMenu.yPositionOnScreen = instance.emojiMenuIcon.bounds.Y - 248;
            }
        }

        internal static void OffsetBuffsDisplayFromToolbar(BuffsDisplay instance, SpriteBatch sb) // DEBUG: SpriteBatch used for debug drawing only.
        {
            if (!IsEnabled() || !ModEntry.Config.HudTweaksFeature.IsToolbarHudOffsetEnabled)
            {
                ResetBuffsDisplayPositionIfChanged(instance);
                return;
            }

            Toolbar toolbar = GetToolbar();
            if (toolbar is null)
            {
                return;
            }

            if (false/*!IsToolbarTopAligned(toolbar)*/)
            {
                ResetBuffsDisplayPositionIfChanged(instance);
            }
            else
            {
                // Toolbar is at the top of the screen, so we check if the buffs display is obstructing it.
                Rectangle tsarea = Game1.game1.GraphicsDevice.Viewport.GetTitleSafeArea();
                int actualDefaultBuffsXPos = tsarea.Right - 588;
                int actualBuffsWidth = 288;

                int actualToolbarXPos = Game1.uiViewport.Width / 2 - 384 - 32;
                int actualToolbarRightPos = actualToolbarXPos + toolbar.width - 64;//toolbar.xPositionOnScreen + toolbar.width;//Game1.uiViewport.Width / 2 + 384 + 64 + 16;
                int actualToolbarWidth = actualToolbarRightPos - actualToolbarXPos;
                int buffsToolbarRightOverlap = Math.Max(0, actualToolbarRightPos - actualDefaultBuffsXPos + 116);/*actualToolbarRightPos - actualDefaultBuffsXPos;*/
                int squeezedRightBuffsWidth = actualBuffsWidth - buffsToolbarRightOverlap;
                if (squeezedRightBuffsWidth >= 116/* && squeezedRightBuffsWidth < actualBuffsWidth*/)
                {
                    // Squeeze the width of the buffs display to fit between the right of the toolbar and the game clock.
                    buffsToolbarRightOverlap = (5 - ((squeezedRightBuffsWidth + 64) / 64)) * 64; // Work in intervals of 64.
                    int buffsXPos = actualDefaultBuffsXPos + buffsToolbarRightOverlap;

                    instance.arrangeTheseComponentsInThisRectangle(buffsXPos, instance.yPositionOnScreen, squeezedRightBuffsWidth / 64/* - 1*/, 64, 64, 8, rightToLeft: true);
                    isBuffsDisplayPositionChanged.Value = true;

                    // DEBUG
                    if (true/*ModEntry.Config.HudTweaksFeature.IsDebugMode*/)
                    {
                        sb.Draw(Game1.fadeToBlackRect, new Rectangle(toolbar.xPositionOnScreen, toolbar.yPositionOnScreen + 1, toolbar.width, toolbar.height / 2), new Color(0, 0, 255, 50)); //blue
                        sb.Draw(Game1.fadeToBlackRect, new Rectangle(actualToolbarXPos, 3, actualToolbarRightPos - actualToolbarXPos, toolbar.height / 2), new Color(0, 255, 0, 50)); //green

                        sb.Draw(Game1.fadeToBlackRect, new Rectangle(instance.xPositionOnScreen, instance.yPositionOnScreen + 1, instance.width, instance.height), new Color(255, 0, 0, 50)); //red
                        sb.Draw(Game1.fadeToBlackRect, new Rectangle(actualToolbarRightPos, instance.yPositionOnScreen + 2, squeezedRightBuffsWidth, instance.height), new Color(150, 150, 150, 50)); //ltgray
                        sb.Draw(Game1.fadeToBlackRect, new Rectangle(buffsXPos, instance.yPositionOnScreen + 3, squeezedRightBuffsWidth, instance.height), new Color(0, 0, 255, 50)); //blue
                    }
                    // DEBUG
                }
                else if (instance.xPositionOnScreen <= actualToolbarRightPos)
                {
                    // Move the buffs display to the left of the toolbar, squeezing to fit in the available width if necessary.
                    // If not enough width available, offset the buffs display below the toolbar and use default width.
                    int buffsXPos = tsarea.Left + 8;
                    int buffsYPos = actualToolbarXPos >= 200
                        ? tsarea.Top + 8
                        : tsarea.Top + 8 + (toolbar.height / 2);
                    int buffsWidth = actualToolbarXPos >= 200
                        ? Math.Min(actualToolbarXPos - buffsXPos - 64, instance.width)
                        : instance.width;

                    instance.arrangeTheseComponentsInThisRectangle(buffsXPos, buffsYPos, buffsWidth / 64, 64, 64, 8, rightToLeft: false);
                    isBuffsDisplayPositionChanged.Value = true;

                    // DEBUG
                    if (true/*ModEntry.Config.HudTweaksFeature.IsDebugMode*/)
                    {
                        sb.Draw(Game1.fadeToBlackRect, new Rectangle(toolbar.xPositionOnScreen, toolbar.yPositionOnScreen + 1, toolbar.width, toolbar.height / 2), new Color(0, 0, 255, 50)); //blue
                        sb.Draw(Game1.fadeToBlackRect, new Rectangle(actualToolbarXPos, 3, actualToolbarWidth, toolbar.height / 2), new Color(0, 255, 0, 50)); //green

                        sb.Draw(Game1.fadeToBlackRect, new Rectangle(instance.xPositionOnScreen, instance.yPositionOnScreen + 1, instance.width, instance.height), new Color(255, 0, 0, 50)); //red
                        sb.Draw(Game1.fadeToBlackRect, new Rectangle(buffsXPos, buffsYPos + 3, buffsWidth, instance.height), new Color(255, 0, 255, 50)); //magenta
                    }
                    // DEBUG
                }
            }
        }

        private static void ResetBuffsDisplayPositionIfChanged(BuffsDisplay instance)
        {
            if (isBuffsDisplayPositionChanged.Value)
            {
                // Reset our changes to the buffs display position.
                instance.arrangeTheseComponentsInThisRectangle(instance.xPositionOnScreen, instance.yPositionOnScreen, instance.width / 64, 64, 64, 8, rightToLeft: true);
                isBuffsDisplayPositionChanged.Value = false;
            }
        }

        private static bool IsEnabled()
        {
            if (!ModEntry.Config.IsModEnabled
                || !ModEntry.Config.HudTweaksFeature.IsFeatureEnabled)
            {
                // Mod and/or Feature is disabled.
                return false;
            }

            if (ModEntry.Config.HudTweaksFeature.IsSplitscreenOnly && GameRunner.instance.gameInstances.Count == 1)
            {
                // We are not currently playing splitscreen.
                return false;
            }

            return true;
        }

        private static Toolbar GetToolbar()
        {
            IClickableMenu toolbarMenu = Game1.onScreenMenus.FirstOrDefault(x => x is Toolbar);
            if (toolbarMenu == null)
            {
                // Toolbar not found in Game1.onScreenMenus.
                return null;
            }

            return (Toolbar)toolbarMenu;
        }

        private static bool IsToolbarTopAligned(Toolbar toolbar)
        {
            // Quick and easy solution: simply check whether the toolbar is closer to the top of the screen or the bottom.
            int distFromTop = toolbar.yPositionOnScreen;
            int distFromBottom = Game1.viewport.Height - toolbar.yPositionOnScreen;
            return distFromTop < distFromBottom;
        }
    }
}
