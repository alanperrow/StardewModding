namespace ConvenientInventory.API
{
    public interface IConvenientInventoryAPI
    {
        /// <summary>
        /// Whether the item is a favourite item.
        /// </summary>
        /// <param name="index">The index of item in player inventory.</param>
        bool IsFavouriteItem(int index);
    }
}