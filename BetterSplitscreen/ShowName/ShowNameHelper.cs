using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace SplitscreenImproved.ShowName
{
    internal class ShowNameHelper
    {
        public static void DrawPlayerNameScroll(SpriteBatch sb)
        {
            // TODO: Maybe use transpiler instead of post-render call.
            //       I don't want the player name scroll to block useful information if the screen is very small.

            if (!ModEntry.Config.IsModEnabled || !ModEntry.Config.ShowNameFeature.IsFeatureEnabled)
            {
                return;
            }

            // TODO: Do not draw scroll for singleplayer. (Maybe config for "Draw only in splitscreen" / "Always draw"?)
            /*if (!Game1.IsMultiplayer || (Game1.IsMultiplayer && Game1.local))
            {
                // We are either playing singleplayer, or online multiplayer without any local instances.
                // We do not need to draw the player name scroll if there is only one screen being displayed, so return early.
                return;
            }*/

            var menu = Game1.activeClickableMenu;
            if (menu is null)
            {
                return;
            }

            if (menu is ShippingMenu or LevelUpMenu)
            {
                int posY = ModEntry.Config.ShowNameFeature.Position switch
                {
                    ShowNamePosition.Bottom => Game1.uiViewport.Height - 70,
                    _ => 30,
                };

                SpriteText.drawStringWithScrollCenteredAt(sb, Game1.player.Name, Game1.uiViewport.Width / 2, posY);

                return;
            }
        }
    }
}
