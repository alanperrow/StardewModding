namespace ConvenientInventory
{
    /// <summary>
    /// The API provided by the Convenient Inventory mod.
    /// </summary>
    public interface IConvenientInventoryApi
    {
        /// <summary>
        /// Gets an array representing all item slots in the player's inventory and whether they are favorited.
        /// If an item slot is favorited, the value at that item slot's index in the array will be <see langword="true"/>;
        /// otherwise, it will be <see langword="false"/>.
        /// If favorite items is disabled in the mod's config, all values in the returned array will be <see langword="false"/>.
        /// </summary>
        /// <remarks>This is computed per-screen.</remarks>
        /// <returns>A <see langword="bool"/> array representing the player's favorite item slots.</returns>
        bool[] GetFavoriteItemSlots();

        /// <summary>
        /// Gets a value indicating whether the item slot at the specified index is favorited.
        /// If favorite items is disabled in the mod's config, this will always return <see langword="false"/>.
        /// </summary>
        /// <remarks>This is computed per-screen.</remarks>
        /// <param name="index">The index of the item slot in the player's inventory.</param>
        /// <returns><see langword="true"/> if the item slot at the specified index is favorited; otherwise, <see langword="false"/>.</returns>
        bool IsFavoriteItemSlot(int index);
    }
}