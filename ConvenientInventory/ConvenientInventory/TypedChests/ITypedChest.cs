using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

namespace ConvenientInventory.TypedChests
{
    /// <summary>
    /// Interface for a wrapper for <see cref="StardewValley.Objects.Chest"/>, holding information about what type of chest this is.
    /// </summary>
    public interface ITypedChest
    {
        /// <summary>The actual chest which is being wrapped by this class.</summary>
        Chest Chest { get; }

        /// <summary>Draws <see cref="Chest"/> in the quick stack tooltip.</summary>
        /// <returns>The number of tooltip position indexes skipped while drawing (due to buildings occupying > 1 index).</returns>
        int DrawInToolTip(SpriteBatch spriteBatch, Point toolTipPosition, int posIndex);

        /// <summary>Determines whether this typed chest is that of a building, such as a Mill or Junimo Hut.</summary>
        /// <returns>Whether this chest type is that of a building.</returns>
        bool IsBuildingChestType();
    }
}
