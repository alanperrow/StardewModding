﻿using StardewValley;
using System;

namespace ConvenientInventory.Compatibility
{
    public class InventoryExpansions
    {
        /// <summary> Checks if the player's max number of items differs from the length of favoriteItemSlots. </summary>
        public static bool IsPlayerMaxItemsChanged(bool[] favoriteItemSlots) => Math.Max(Game1.player.MaxItems, Game1.player.Items.Count) != favoriteItemSlots?.Length;

        /// <summary> Expands/trims favoriteItemSlots to have a length of newSize. </summary>
        public static bool[] ResizeFavoriteItemSlots(bool[] favoriteItemSlots, int newSize)
        {
            if (favoriteItemSlots?.Length < newSize)
            {
                // Expand
                bool[] favoriteItemSlotsExpanded = new bool[newSize];
                favoriteItemSlots.CopyTo(favoriteItemSlotsExpanded, 0);

                return favoriteItemSlotsExpanded;
            }

            if (favoriteItemSlots?.Length > newSize)
            {
                // Trim
                bool[] favoriteItemSlotsTrimmed = favoriteItemSlots[0..newSize];

                return favoriteItemSlotsTrimmed;
            }

            return favoriteItemSlots;
        }
    }
}
