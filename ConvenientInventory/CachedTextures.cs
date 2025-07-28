using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;

namespace ConvenientInventory
{
    internal static class CachedTextures
    {
        /// <summary>
        /// The asset name prefix for all custom assets created by this mod.
        /// </summary>
        public const string ModAssetPrefix = "Mods/gaussfire.ConvenientInventory/";

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

        public static Texture2D ChestQuickStackDisabledButtonIcon { get; private set; }

        public static Texture2D ChestQuickStackEnabledButtonIcon { get; private set; }

        public static Texture2D ChestQuickStackPriority1ButtonIcon { get; private set; }

        public static Texture2D ChestQuickStackPriority2ButtonIcon { get; private set; }

        public static Texture2D ChestQuickStackPriority3ButtonIcon { get; private set; }

        public static void LoadGameAssets()
        {
            Mill = Game1.content.Load<Texture2D>(@"Buildings\Mill");
            JunimoHut = Game1.content.Load<Texture2D>(@"Buildings\Junimo Hut");
            FarmHouse = Game1.content.Load<Texture2D>(@"Maps\farmhouse_tiles");
        }

        public static void LoadModAssets(ModConfig config)
        {
            QuickStackButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "quickStackIcon");
            FavoriteItemsCursor = Game1.content.Load<Texture2D>(ModAssetPrefix + "favoriteCursor");
            FavoriteItemsHighlight = Game1.content.Load<Texture2D>(ModAssetPrefix + $"favoriteHighlight_{config.FavoriteItems.HighlightTextureChoice}");
            FavoriteItemsBorder = Game1.content.Load<Texture2D>(ModAssetPrefix + "favoriteBorder");
            AutoOrganizeButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "autoOrganizeIcon");
            ChestQuickStackDisabledButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackDisabled");
            ChestQuickStackEnabledButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackEnabled");
            ChestQuickStackPriority1ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority1");
            ChestQuickStackPriority2ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority2");
            ChestQuickStackPriority3ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority3");
        }

        /// <summary>
        /// Define the custom asset based on the internal file.
        /// </summary>
        internal static void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo(ModAssetPrefix + "quickStackIcon"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\quickStackIcon.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "favoriteCursor"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\favoriteCursor.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.StartsWith(ModAssetPrefix + "favoriteHighlight_"))
            {
                string textureChoice = e.Name.BaseName.Split("favoriteHighlight_")[1];
                e.LoadFromModFile<Texture2D>($@"assets\favoriteHighlight_{textureChoice}.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "favoriteBorder"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\favoriteBorder.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "autoOrganizeIcon"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\autoOrganizeIcon.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackDisabled"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackDisabled.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackEnabled"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackEnabled.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority1"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackPriority1.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority2"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackPriority2.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority3"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackPriority3.png", AssetLoadPriority.Medium);
            }
        }

        /// <summary>
        /// Update the data when it's reloaded.
        /// </summary>
        internal static void OnAssetReady(AssetReadyEventArgs e)
        {
            if (e.Name.IsEquivalentTo(ModAssetPrefix + "quickStackIcon"))
            {
                QuickStackButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "quickStackIcon");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "favoriteCursor"))
            {
                FavoriteItemsCursor = Game1.content.Load<Texture2D>(ModAssetPrefix + "favoriteCursor");
            }
            else if (e.Name.StartsWith(ModAssetPrefix + "favoriteHighlight_"))
            {
                string textureChoice = e.Name.BaseName.Split("favoriteHighlight_")[1];
                FavoriteItemsHighlight = Game1.content.Load<Texture2D>(ModAssetPrefix + $"favoriteHighlight_{textureChoice}");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "favoriteBorder"))
            {
                FavoriteItemsBorder = Game1.content.Load<Texture2D>(ModAssetPrefix + "favoriteBorder");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "autoOrganizeIcon"))
            {
                AutoOrganizeButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "autoOrganizeIcon");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackDisabled"))
            {
                ChestQuickStackDisabledButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackDisabled");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackEnabled"))
            {
                ChestQuickStackEnabledButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackEnabled");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority1"))
            {
                ChestQuickStackPriority1ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority1");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority2"))
            {
                ChestQuickStackPriority2ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority2");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority3"))
            {
                ChestQuickStackPriority3ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority3");
            }
        }
    }
}
