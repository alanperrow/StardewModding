using System.Text;
using ConvenientInventory.TypedChests;
using StardewValley;
using System.Collections.Generic;

namespace ConvenientInventory.QuickStack
{
    /// <summary>Tracks items moved by Quick Stack, grouped by chest; to be logged.</summary>
    public class QuickStackSummary
    {
        private readonly Dictionary<TypedChest, List<MovedItem>> _movedItemsByTypedChest = new();

        /// <summary>Adds an item to the summary of items moved by Quick Stack, grouped by chest.</summary>
        public void AddToSummary(TypedChest quickStackedChest, string itemName, int resultingStack, int beforeStack)
        {
            if (!_movedItemsByTypedChest.ContainsKey(quickStackedChest))
            {
                _movedItemsByTypedChest[quickStackedChest] = new List<MovedItem>();
            }

            _movedItemsByTypedChest[quickStackedChest].Add(new MovedItem(itemName, resultingStack, beforeStack));
        }

        /// <summary>Gets a summary message of the items moved by Quick Stack, grouped by chest.</summary>
        /// <returns>
        /// The summary message.
        /// <para/>Example:
        /// <br/>"Quick Stack deposited items into the following chests, in order:
        /// <br/>1.) Chest 'Big Chest' at location Farm {2, 3} received items: ['Wood' x 10, 'Stone' x 5]
        /// <br/>2.) Chest 'Stone Chest' at location Town {42, 6} received items: ['Gold Bar' x 77, 'Prize Ticket' x 1]"
        /// </returns>
        public string GetSummaryMessage()
        {
            if (_movedItemsByTypedChest.Count == 0)
            {
                return null;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Quick Stack deposited items into the following chests, in order:");

            int chestIndex = 0;
            foreach ((TypedChest quickStackedChest, List<MovedItem> movedItems) in _movedItemsByTypedChest)
            {
                _ = quickStackedChest.Chest != null
                    ? sb.Append($"\t{++chestIndex}.) Chest '{quickStackedChest.Chest.Name}' of type '{quickStackedChest.ChestType}' ")
                        .Append($"at location {quickStackedChest.ChestGameLocation.Name} {quickStackedChest.Chest.TileLocation} ")
                    : sb.Append($"\t{++chestIndex}.) Inventory '{quickStackedChest.InventoryName}' with context '{quickStackedChest.InventoryContext}' ")
                        .Append($"at location {quickStackedChest.InventoryLocation.Name} ");
                if (quickStackedChest.VisualTileLocation.HasValue)
                {
                    sb.Append($"(Visual {quickStackedChest.VisualTileLocation.Value}) ");
                }
                sb.Append("received items: ");

                sb.Append('[');
                for (int i = 0; i < movedItems.Count; i++)
                {
                    MovedItem movedItem = movedItems[i];
                    sb.Append($"'{movedItem.ItemName}' x {movedItem.AmountMoved}");
                    if (i < movedItems.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(']');

                if (chestIndex < _movedItemsByTypedChest.Count)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private record MovedItem
        {
            public string ItemName { get; }

            public int AmountMoved { get; }

            public MovedItem(string itemName, int resultingStack, int beforeStack)
            {
                ItemName = itemName;
                AmountMoved = beforeStack - resultingStack;
            }
        }
    }
}
