NexusMods link: https://www.nexusmods.com/stardewvalley/mods/10384

# Convenient Inventory
Adds convenience features to the player's inventory, such as quick stack to nearby chests and favorited items.

## Preview
One of the most tedious parts of any game is inventory management, and this is especially true in Stardew Valley. Every day, whether it be from farming produce, mining rewards, or freshly caught fish, you will be emptying out your backpack to store items into chests on your farm. After a while, this process becomes all too familiar: open chest, deposit items, close chest, open chest, deposit items, close chest, open chest, deposit items, close chest, ... etc.

What if you could stow away all the items taking up space in your backpack, instantly, while keeping the important ones?

![](https://imgur.com/R4QWKVI.gif)

## Features
#### Quick Stack to Nearby Chests
Click the new "Quick Stack to Nearby Chests" button in the player's inventory UI to quickly deposit items into chests within a nearby range.

#### Favorite Items
Hold the favorite-hotkey (Left Alt, by default) and select items in the player's inventory to favorite them.

Favorited items are prevented from:
 - Being quick stacked
 - Being trashed
 - Being dropped
 - Being considered when using the "Organize" button in the player's inventory
 - Being considered when using the "Add to Existing Stacks" button in a chest

## Config
 - **IsEnableQuickStack**: If enabled, adds a "Quick Stack To Nearby Chests" button to your inventory menu. Pressing this button will stack items from your inventory to any nearby chests which contain that item.
 - **QuickStackRange**: How many tiles away from the player to search for nearby chests.
 - **IsQuickStackIntoBuildingsWithInventories**: If enabled, nearby buildings with inventories (such as Mills or Junimo Huts) will also be checked when quick stacking.
 - **IsQuickStackOverflowItems**: If enabled, quick stack will place as many items as possible into chests which contain that item, rather than just a single stack.
 - **IsQuickStackTooltipDrawNearbyChests**: If enabled, hovering over the quick stack button will show a preview of all nearby chests, ordered by distance.
 - **IsEnableFavoriteItems**: If enabled, items in your inventory can be favorited. Favorited items will be ignored when stacking into chests.
 - **FavoriteItemsHighlightTextureChoice**: Choose your preferred texture style for highlighting favorited items in your inventory.
   - ( 0: ![](https://i.imgur.com/fTMl0FT.png),  1: ![](https://i.imgur.com/NTlia1R.png),  2: ![](https://i.imgur.com/QGztt8Q.png),  3: ![](https://i.imgur.com/MBG2A6e.png),  4: ![](https://i.imgur.com/rZqklnN.png),  5: ![](https://i.imgur.com/FvKpyZV.png) )
 - FavoriteItemsKeyboardHotkey: Hold this key when selecting an item to favorite it.
 - FavoriteItemsControllerHotkey: Hold this button when selecting an item to favorite it. (For controller support)

## Compatibility
 - Supports single player, split-screen local multiplayer, and online multiplayer.
 - Supports controllers by using the left-stick button (configurable) for favoriting.
 - Supports [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) for in-game config editing.
 - Supports [Bigger Backpack](https://www.nexusmods.com/stardewvalley/mods/1845) and other inventory expansion mods (as of version 1.1.0).
