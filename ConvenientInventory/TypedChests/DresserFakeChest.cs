using StardewValley;
using StardewValley.Objects;

namespace ConvenientInventory.TypedChests
{
    public class DresserFakeChest : Chest
    {
        public DresserFakeChest(StorageFurniture dresser)
            : base()
        {
            Dresser = dresser;
        }

        /// <summary>
        /// Gets the original <see cref="StorageFurniture"/> instance used to create this fake dresser chest instance.
        /// </summary>
        public StorageFurniture Dresser { get; }

        public override Item addItem(Item item)
        {
            Dresser.onDresserItemDeposited(item);
            return null;
        }

        public override int GetActualCapacity()
        {
            // 36 is the hardcoded dresser capacity, and is referenced in SDV source code `StorageFurniture.AddItem`.
            // However, the method `StorageFurniture.onDresserItemDeposited` does not check for capacity before adding an item, resulting in an infinite capacity.
            return int.MaxValue;
        }
    }
}
