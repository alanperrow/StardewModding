using System;
using StardewModdingAPI;

namespace ConvenientInventory
{
    /// <inheritdoc/>
    public class ConvenientInventoryApi : IConvenientInventoryApi
    {
        /// <inheritdoc/>
        public bool[] GetFavoriteItemSlots()
        {
            if (!Context.IsWorldReady)
            {
                return Array.Empty<bool>();
            }

            bool[] result = new bool[ConvenientInventory.FavoriteItemSlots.Length];
            if (ModEntry.Config.FavoriteItems.IsEnabled)
            {
                // We only copy the values if favorite items is enabled;
                // if it is disabled, then the default initialized array will already have the correct length with all values set to false.
                ConvenientInventory.FavoriteItemSlots.CopyTo(result, 0);
            }

            return result;
        }

        /// <inheritdoc/>
        public bool IsFavoriteItemSlot(int index)
        {
            if (!Context.IsWorldReady || !ModEntry.Config.FavoriteItems.IsEnabled)
            {
                return false;
            }

            if (index < 0 || index >= ConvenientInventory.FavoriteItemSlots.Length)
            {
                // Instead of throwing, simply return false to avoid any issues with other mods specifying an invalid index.
                return false;
            }

            return  ConvenientInventory.FavoriteItemSlots[index];
        }
    }
}