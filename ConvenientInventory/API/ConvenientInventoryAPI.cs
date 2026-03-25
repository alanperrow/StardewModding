namespace ConvenientInventory.API
{
    public class ConvenientInventoryAPI : IConvenientInventoryAPI
    {
        /// <summary>Get the exclusive instance of the ConvenientInventoryAPI.</summary>
        public static readonly ConvenientInventoryAPI Instance = new();

        private ConvenientInventoryAPI() { }

        /// <inheritdoc/>
        public bool IsFavouriteItem(int index)
        {
            return ModEntry.Config.FavoriteItems.IsEnabled && ConvenientInventory.FavoriteItemSlots[index];
        }
    }
}