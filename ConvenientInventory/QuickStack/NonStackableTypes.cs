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
        [Display(Name = "None")]
        None = 0,

        Tools = 1 << 0,
        Weapons = 1 << 1,
        Equips = 1 << 2,
        Other = 1 << 3,

        [Display(Name = "All")]
        All = Tools | Weapons | Equips | Other,

        [Display(Name = "Except Tools")]
        ExceptTools = Weapons | Equips | Other,

        [Display(Name = "Except Tools and Weapons")]
        ExceptToolsWeapons = Equips | Other,

        [Display(Name = "Except Tools, Weapons, and Equips")]
        ExceptToolsWeaponsEquips = Other,
    }
}
