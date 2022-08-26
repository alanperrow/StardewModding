using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;

namespace ConvenientInventory.TypedChests
{
    public interface ITypedChest
    {
        /// <summary>The actual chest which is being wrapped by this class.</summary>
        Chest Chest { get; }

        /// <summary>Draws <see cref="Chest"/> in the quick stack tooltip.</summary>
        /// <returns>The number of tooltip position indexes skipped while drawing (due to buildings occupying > 1 index).</returns>
        int DrawInToolTip(SpriteBatch spriteBatch, Point toolTipPosition, int posIndex);
    }
}
