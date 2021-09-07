# StardewModding
### Mod development for Stardew Valley using SMAPI

This repository contains all of the mods that I have created and/or am currently developing for Stardew Valley.

**NexusMods username**: gaussfire

## Mod Ideas

### Controller Convenience
Reposition some of the UI buttons when using a controller to be easier, quicker, or otherwise more convenient to use.

Examples:
  * Chest quick-stack (and the other buttons)
  * ...

Implementation:
  * Maybe just add leftNeighbor to left "invisible" position that maps to the right-most button of the interface? And vice versa.

### Deposit to Nearby Chests
Adds a button on side of player backpack UI to quickly deposit items to chests within a nearby range.

When button is selected/hovered over, show a tooltip that says "Deposit to: _ _ _", where each _ would be a render of a nearby chest (including its color)

Config:
  * Deposit all backpack rows, including first row
  * Nearby range
  * Render nearby chests in button tooltip

### Chest Upgrades
Adds four craftable "upgrade" items that can be used on chests to expand their storage capacity (think Iron Chests mod).

Each successive upgrade requires the previous: Original -> Copper (+1 row) -> Iron (+2) -> Gold (+3) -> Iridium (+4).

Will require Expanded Storage mod.

<!--
===== SCRAPPED =====
### Outgoing Mail
Adds the ability to send your own mail to other people in town. You can either write a message (counts as talking with recipient), attach items as gifts (counts as gifting the item to the recipient), or both. 

Your mail will need time to be picked up and delivered, so these actions will be performed the day *after* you put your outgoing mail in the mailbox. 

Mailing a letter does not cost anything. Mailing a gift with the letter should cost a slight mailing fee, maybe 5-50g, scaling with the price of the gift.

Why send mail? Simply put, for convenience:
  * Can gain friendship with townsfolk easier, at the expense of some slight mailing fees.
  * If someone's birthday is coming up, instead of waiting until the day of to stand outside their bedroom door for hours, you can just send them their favorite gift through the mail! Be sure to send it on the day before their birthday, though, so the mail has time to be delivered.

For implementation, not sure which one of these would work better:
  * Use a stamp, which is a new craftable/purchasable item, on your mailbox, or
  * Require an upgrade for your mailbox from Robin. This upgrade would be instant upon purchase.
====================
-->
