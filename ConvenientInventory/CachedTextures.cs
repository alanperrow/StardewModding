using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace ConvenientInventory
{
    internal static class CachedTextures
    {
        // Base Game Textures
        public static Texture2D Mill { get; private set; }

        public static Texture2D JunimoHut { get; private set; }

        public static Texture2D FarmHouse { get; private set; }

        // Mod Textures
        public static Texture2D QuickStackButtonIcon { get; private set; }

        public static Texture2D FavoriteItemsCursor { get; private set; }

        public static Texture2D FavoriteItemsHighlight { get; set; } // Public setter so GMCM can update this texture in-game.

        public static Texture2D FavoriteItemsBorder { get; private set; }

        public static Texture2D AutoOrganizeButtonIcon { get; private set; }

        public static void LoadCachedTextures(IModHelper helper, ModConfig config)
        {
            Mill = helper.GameContent.Load<Texture2D>(@"Buildings\Mill");
            JunimoHut = helper.GameContent.Load<Texture2D>(@"Buildings\Junimo Hut");
            FarmHouse = helper.GameContent.Load<Texture2D>(@"Maps\farmhouse_tiles");

            QuickStackButtonIcon = helper.ModContent.Load<Texture2D>(@"assets\quickStackIcon.png");
            FavoriteItemsCursor = helper.ModContent.Load<Texture2D>(@"assets\favoriteCursor.png");
            FavoriteItemsHighlight = helper.ModContent.Load<Texture2D>($@"assets\favoriteHighlight_{config.FavoriteItemsHighlightTextureChoice}.png");
            FavoriteItemsBorder = helper.ModContent.Load<Texture2D>(@"assets\favoriteBorder.png");
            AutoOrganizeButtonIcon = helper.ModContent.Load<Texture2D>(@"assets\autoOrganizeIcon.png");
        }
    }
}
