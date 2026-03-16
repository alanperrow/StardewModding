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

        public static Texture2D FillStacksQuickStackButtonIcon { get; private set; }

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
            ChestQuickStackDisabledButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackDisabledIcon");
            ChestQuickStackEnabledButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackEnabledIcon");
            ChestQuickStackPriority1ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority1Icon");
            ChestQuickStackPriority2ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority2Icon");
            ChestQuickStackPriority3ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority3Icon");
            FillStacksQuickStackButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "fillStacksQuickStackIcon");
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
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackDisabledIcon"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackDisabledIcon.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackEnabledIcon"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackEnabledIcon.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority1Icon"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackPriority1Icon.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority2Icon"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackPriority2Icon.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority3Icon"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\chestQuickStackPriority3Icon.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "fillStacksQuickStackIcon"))
            {
                e.LoadFromModFile<Texture2D>(@"assets\fillStacksQuickStackIcon.png", AssetLoadPriority.Medium);
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
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackDisabledIcon"))
            {
                ChestQuickStackDisabledButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackDisabledIcon");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackEnabledIcon"))
            {
                ChestQuickStackEnabledButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackEnabledIcon");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority1Icon"))
            {
                ChestQuickStackPriority1ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority1Icon");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority2Icon"))
            {
                ChestQuickStackPriority2ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority2Icon");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "chestQuickStackPriority3Icon"))
            {
                ChestQuickStackPriority3ButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "chestQuickStackPriority3Icon");
            }
            else if (e.Name.IsEquivalentTo(ModAssetPrefix + "fillStacksQuickStackIcon"))
            {
                FillStacksQuickStackButtonIcon = Game1.content.Load<Texture2D>(ModAssetPrefix + "fillStacksQuickStackIcon");
            }
        }
    }
}
