using System;
using System.ComponentModel.DataAnnotations;

namespace ConvenientInventory.QuickStack
{
    /// <summary>
    /// Flags enum representing types of non-stackable items.
    /// </summary>
    [Flags]
    public enum NonStackableTypes
    {
        None = 0,
        Tools = 1 << 0,
        Weapons = 1 << 1,
        Equips = 1 << 2,
        Other = 1 << 3,

        All = Tools | Weapons | Equips | Other,
        ExceptTools = Weapons | Equips | Other,
        ExceptToolsWeapons = Equips | Other,
        ExceptToolsWeaponsEquips = Other,
    }
}
