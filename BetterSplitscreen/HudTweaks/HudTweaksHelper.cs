using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace SplitscreenImproved.HudTweaks
{
    internal static class HudTweaksHelper
    {
        static readonly PerScreen<int> prevToolbarYPosition = new();

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
            if (!IsEnabled())
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
                // Toolbar is at the bottom of the screen, so we offset it to avoid being obstructed by chatbox.
                instance.yPositionOnScreen -= toolbar.height / 2;
            }

            // TODO: Copy-pasted base game logic. Maybe update this to a transpiler call?
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
