using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using StardewValley.Tools;

namespace ConvenientInventory.QuickStack
{
    public static class NonStackableLogic
    {
        /// <summary>
        /// Determines whether the current config allows quick stacking of the provided non-stackable item.
        /// </summary>
        public static bool CanOverflowNonStackable(Item item) =>
            ModEntry.Config.QuickStack.NonStackableTypesToOverflow.HasFlag(GetNonStackableType(item));

        /// <summary>
        /// Determines whether two items represent the same "type" of non-stackable item.
        /// </summary>
        public static bool AreEquivalentNonStackables(Item item, Item other)
        {
            if (item is null || other is null)
            {
                return false;
            }

            if (object.ReferenceEquals(item, other))
            {
                // Shouldn't be possible, but avoid false positives if the same item instance is compared against itself.
                return false;
            }

            if (item.maximumStackSize() > 1 || other.maximumStackSize() > 1)
            {
                return false;
            }

            if (item.QualifiedItemId != other.QualifiedItemId)
            {
                return false;
            }

            if (!item.Name.Equals(other.Name))
            {
                return false;
            }

            if (!ModEntry.Config.QuickStack.IgnoreItemQuality
                && item.Quality != other.Quality)
            {
                return false;
            }

            if (other.GetType() != item.GetType())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines the non-stackable item type of the provided item.
        /// </summary>
        private static NonStackableTypes GetNonStackableType(Item item) => item switch
        {
            MeleeWeapon mw when mw.isScythe() => NonStackableTypes.Tools,
            MeleeWeapon or Slingshot => NonStackableTypes.Weapons,
            Tool => NonStackableTypes.Tools,
            Boots or Clothing or Hat or Ring or Trinket => NonStackableTypes.Equips,
            _ => NonStackableTypes.Other,
        };
    }
}
