using StardewValley.Menus;

namespace ConvenientInventory.Compatibility
{
    public interface ICustomBackpackApi
    {
        public bool SetPlayerSlots(int slots, bool force);

        public bool ChangeScroll(InventoryMenu menu, int delta);

        /// <summary>
        /// Gets the per-screen scroll amount of the custom backpack inventory menu.
        /// Each increment represents one scrolled row; 0 indicates no scroll (default).
        /// </summary>
        /// <returns>The current scroll amount.</returns>
        public int GetScroll();
    }
}