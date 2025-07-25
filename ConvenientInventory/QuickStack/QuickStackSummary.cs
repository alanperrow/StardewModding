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
        public void AddToSummary(TypedChest quickStackedChest, Item quickStackedItem, int beforeStack)
        {
            if (!_movedItemsByTypedChest.ContainsKey(quickStackedChest))
            {
                _movedItemsByTypedChest[quickStackedChest] = new List<MovedItem>();
            }

            _movedItemsByTypedChest[quickStackedChest].Add(new MovedItem(quickStackedItem, beforeStack));
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
                sb.Append($"\t{++chestIndex}.) Chest '{quickStackedChest.Chest.Name}' of type '{quickStackedChest.ChestType}' ");
                sb.Append($"at location {quickStackedChest.ChestGameLocation.Name} {quickStackedChest.Chest.TileLocation} ");
                if (quickStackedChest.VisualTileLocation.HasValue)
                {
                    sb.Append($"(Visual {quickStackedChest.VisualTileLocation.Value})");
                }
                sb.Append("received items: ");

                sb.Append('[');
                for (int i = 0; i < movedItems.Count; i++)
                {
                    MovedItem movedItem = movedItems[i];
                    sb.Append($"'{movedItem.Item.Name}' x {movedItem.AmountMoved}");
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
            public Item Item { get; }

            public int AmountMoved { get; }

            public MovedItem(Item item, int beforeStack)
            {
                Item = item;
                AmountMoved = beforeStack - item.Stack;
            }
        }
    }
}
