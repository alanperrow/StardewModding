using ConvenientInventory.TypedChests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

namespace ConvenientInventory.Compatibility.TypedChests
{
    /// <summary>
    /// Wrapper for <see cref="StardewValley.Objects.Chest"/>s implemented with mod content packs, holding information about what type of chest this is.
    /// </summary>
    public class ContentPackTypedChest : ITypedChest
    {
        public Chest Chest { get; private set; }

        public ContentPackChestType ChestType { get; private set; }

        public ContentPackTypedChest(Chest chest, ContentPackChestType chestType)
        {
            Chest = chest;
            ChestType = chestType;
        }

        public static ContentPackChestType DetermineChestType(Chest chest)
        {
            return null;
        }

        public int DrawInToolTip(SpriteBatch spriteBatch, Point toolTipPosition, int posIndex)
        {
            return 0;
        }
    }
}
