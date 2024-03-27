using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace BetterSplitscreen.Layout
{
    internal class LayoutPreviewHelper
    {
        private static Color P1Color { get; } = new Color(60, 90, 220);

        private static Color P2Color { get; } = new Color(255, 50, 50);

        private static Color P3Color { get; } = new Color(50, 230, 50);

        private static Color P4Color { get; } = new Color(255, 220, 30);

        public int PlayerCount { get; set; } = 2;

        /// <summary>
        /// Draws a preview of the current layout with the number of players provided by <see cref="PlayerCount"/>.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="p"></param>
        public void DrawPreview(SpriteBatch sb, Vector2 p)
        {
            int px = (int)p.X;
            int py = (int)p.Y;

            // Border
            sb.Draw(Game1.fadeToBlackRect, new Rectangle(px, py, 208, 208), Color.Black);

            // TODO: In LayoutManager, dynamically perform the following draw logic:

            // Player window locations
            // TODO: Draw layout preview based on current SplitscreenLayout
            int p1_x, p1_y, p2_x, p2_y;
            if (ModEntry.Config.LayoutFeature.PresetChoice == LayoutPreset.Default)
            {
                p1_x = px + 4;
                p1_y = py + 4;
                p2_x = px + 4 + 100;
                p2_y = py + 4;
            }
            else
            {
                p1_x = px + 4 + 100;
                p1_y = py + 4;
                p2_x = px + 4;
                p2_y = py + 4;
            }

            sb.Draw(Game1.fadeToBlackRect, new Rectangle(p1_x, p1_y, 100, 200), P1Color);
            sb.Draw(Game1.fadeToBlackRect, new Rectangle(p2_x, p2_y, 100, 200), P2Color);

            // Player indicators
            // TODO: P1, P2, P3, P4; dynamically
            sb.DrawString(Game1.dialogueFont, "P1", new Vector2(p1_x + 30, p1_y + 30), Color.Black);
            sb.DrawString(Game1.dialogueFont, "P2", new Vector2(p2_x + 30, p2_y + 30), Color.Black);
        }

        internal void OnPreviewChanged(string arg1, object arg2)
        {
            throw new NotImplementedException();
        }
    }
}
