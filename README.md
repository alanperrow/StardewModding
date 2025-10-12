# StardewModding

This repository contains all of the mods that I have created and/or am currently developing for [Stardew Valley](https://www.stardewvalley.net/).
Development typically involves the usage of the [Harmony](https://github.com/pardeike/Harmony) library for patching the base game code.

The mods can be loaded and run alongside the base game by using [SMAPI](https://smapi.io/).
See the [player's guide to using mods](https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started) on the official wiki for detailed installation instructions.

You can find a list of my published mods on [my NexusMods profile](https://next.nexusmods.com/profile/gaussfire/mods).
I prefer to play with mods that are intentionally balanced and/or focus on quality-of-life improvements, so that's what you'll find here.

## Future Mod Ideas

### Contextual Button Overlay for Controller/Gamepad
Adds a visual overlay to the bottom-left of the screen when using a controller or gamepad,
indicating which buttons to press for the currently available contextual actions.
The overlay would update dynamically based on the player's current context.

For example, when standing next to a chest, the overlay would show which button to press to open the chest.
If holding a tool that can hit the chest, that button would be displayed as well.

 <!-- SCRAPPED
### Outgoing Mail
Adds the ability to send your own mail to other people in town. You can either write a message (counts as talking with recipient), attach items as gifts (counts as gifting the item to the recipient), or both. 

Your mail will need time to be picked up and delivered, so these actions will be performed the day *after* you put your outgoing mail in the mailbox. 

Mailing a letter does not cost anything. Mailing a gift with the letter should cost a slight mailing fee, maybe 5-50g, scaling with the price of the gift.

Why send mail? Simply put, for convenience:
 - Can gain friendship with townsfolk easier, at the expense of some slight mailing fees.
 - If someone's birthday is coming up, instead of waiting until the day of to stand outside their bedroom door for hours, you can just send them their favorite gift through the mail! Be sure to send it on the day before their birthday, though, so the mail has time to be delivered.

For implementation, not sure which one of these would work better:
 - Use a stamp, which is a new craftable/purchasable item, on your mailbox, or
 - Require an upgrade for your mailbox from Robin. This upgrade would be instant upon purchase.
 
### Chest Upgrades
Adds four craftable "upgrade" items that can be used on chests to expand their storage capacity (think Iron Chests mod).

Each successive upgrade requires the previous: Original -> Copper (+1 row) -> Iron (+2) -> Gold (+3) -> Iridium (+4).

Will require Expanded Storage mod.

### Seed Planter
Adds a new "Seed Planter" tool that can be used to plant seeds in a large area more quickly.

The Seed Planter can be purchased from Clint's shop and upgraded like any other tool (Original -> Copper -> Iron -> Gold -> Iridium).

Features:
 - How to know which seeds to use?
   - Ammo?
   - Per-use, first seed(s) of same type found in inventory?
 - How the tool should be used:
   - Using the tool should not stop player movement, as that would feel a bit clunky and honestly would not be worth getting.
   - Hold the tool to show highlighted area, press left-click to use.
   - Show tool range via highlighted squares, displaying the tiles in which seeds will be planted.
     - Only show highlighted squares up to the number of seeds the player has available to plant, so there is no confusion as to where the seeds will be going.
   - Pressing left-click will shoot the seeds out into the highlighted tiles, planting them into any vacant hoed dirt.
     - Maybe make a quick little animation of the seeds arcing into their respective tiles? Each seed should have a few ms differential, so a satisfying sound can be played when the seeds enter the dirt.
 - Tool should require a brief recharge after each use, ~1s.
   - Left-click can be held down to automatically use the tool again after the recharge period ends.
   - Play a shuffling, mechanical reload sound during this time.
   - For displaying the tool icon "recharging", try to reuse SDV weapon right-click cooldown meter, as this is the same concept.
 - The planting range should make purhcasing/using the tool worthwhile.
   - Max range increases with each tool upgrade.
     - Original: 1x3 (ie: copper watering can)
     - Copper: 1x5 (ie: iron watering can)
     - Iron: 3x3 (ie: gold watering can)
     - Gold: 3x5 (ie: iridium watering can)
     - Iridium: Two ranges unlocked!
       - (1) 5x6
       - (2) 5 steps; trapezoidal shape, each step increasing height by 1 on both sides. (useful for pivoting around a single point, planting in all directions)
         - 3x1, 5x1, 7x1, 9x1, 11x1
   - While holding the tool, pressing right-click toggles the tool's range, so the player has more control over where they are planting.
 -->