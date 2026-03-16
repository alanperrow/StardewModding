# Convenient Inventory
Adds convenience features to the player's inventory, such as quick stack to nearby chests and favorited items.

NexusMods link: https://www.nexusmods.com/stardewvalley/mods/10384

## Preview
One of the most tedious parts of any game is inventory management, and this is especially true in Stardew Valley. Every day, whether it be from farming produce, mining rewards, or freshly caught fish, you will be emptying out your backpack to store items into chests on your farm. After a while, this process becomes all too familiar: open chest, deposit items, close chest, open chest, deposit items, close chest, open chest, deposit items, close chest, ... etc.

What if you could stow away all the items taking up space in your backpack, instantly, while keeping the important ones?

![](https://imgur.com/7y6LhRf.gif)

![](https://imgur.com/5NQcpSj.gif)

## Features
### Quick Stack to Nearby Chests
Click the new "Quick Stack to Nearby Chests" button in the player's inventory UI to quickly deposit items from your inventory into any nearby chests which contain that item.

### Favorite Items
Hold the favorite-hotkey (Left Alt, by default) and select items in the player's inventory to favorite them.

Favorited items are prevented from:
 - Being quick stacked
 - Being trashed
 - Being dropped
 - Being considered when using the "Organize" button in the player's inventory
 - Being considered when using the "Add to Existing Stacks" button in a chest

### Inventory Page Side-Warp (as of version 1.3.0)
For players using controllers, getting to the right side of your inventory menu is a hassle, as your cursor always starts at the leftmost item slot when opening your inventory. This feature allows the cursor to "warp" side-to-side when at either end of the menu.

![](https://i.imgur.com/3IplIkH.gif)

### Take All But One (as of version 1.5.0)
Right-click (or press [X] on controller) on an item stack while holding the Take All But One hotkey ([Left Ctrl + Left Shift] by default, or [Left Trigger] on controller) to instantly take all but one item from the selected item stack, rather than taking a single item at a time.

### Auto Organize Chest (as of version 1.5.0)
Right-click (or press [X] on controller) on the Organize button in a chest to toggle Auto Organize mode for that chest. When a chest has Auto Organize ON, all of its items will remain organized, even when new items are added (including quick stack).

![](https://i.imgur.com/4Dls2ms.gif)

## Console Commands
 - **player_fixinventory**: Resizes the player's inventory to its correct maximum size, dropping any extra items contained in inventory.
   - Some mods directly modify the player's inventory size, causing compatibility issues and/or leaving extra null items when uninstalled; this command should fix these issues.
 - **convinv_clearmoddata**: Clears all mod data set by Convenient Inventory for the currently loaded save; no other mod data is removed. Changes to the save file will take effect the next time the game is saved.
   - This command is intended for players who want to remove any Convenient Inventory mod data from their save file for a complete uninstallation.

## Config
### Quick Stack to Nearby Chests
 - **IsEnableQuickStack**: If enabled, adds a "Quick Stack To Nearby Chests" button to your inventory menu. Pressing this button will stack items from your inventory to any nearby chests which contain that item.
 - **QuickStackRange**: How many tiles away from the player to search for nearby chests.
 - **IsEnableQuickStackHotkey**: If enabled, pressing either of the quick stack hotkeys specified below will quick stack your items, even outside of your inventory menu.
 - **QuickStackKeyboardHotkey**: Press this key to quick stack your items.
 - **QuickStackControllerHotkey**: Press this button to quick stack your items. (For controller support)
 - **IsQuickStackIntoBuildingsWithInventories**: If enabled, nearby buildings with inventories (such as Mills or Junimo Huts) will also be checked when quick stacking.
 - **IsQuickStackOverflowItems**: If enabled, quick stack will place as many items as possible into chests which contain that item, rather than just a single stack.
 - **IsQuickStackIgnoreItemQuality**: (Requires IsQuickStackOverflowItems to be enabled.) If enabled, quick stack will place items into chests which contain ANY quality of that same item.
   - <details><summary>Preview</summary> 
 
     Before:
 
     ![](https://i.imgur.com/AsA4COq.png)
 
     Quick stack, ignoring item quality:
 
     ![](https://i.imgur.com/HMtFqcE.gif)
 
     After:
 
     ![](https://i.imgur.com/yokobZ1.png)
     </details>
 - **IsQuickStackTooltipDrawNearbyChests**: If enabled, hovering over the quick stack button will show a preview of all nearby chests, ordered by distance.
 - **IsEnableQuickStackAnimation**: If enabled, an animation will play when items are quick stacked to visually show where each item was stored.
 - **IsEnableQuickStackChestAnimation**: If enabled, visually opens chests when items are quick stacked into them.
 - **QuickStackAnimationItemSpeed**: For quick stack animation: Adjusts the speed at which quick stacked items move toward their stored chest.
 - **QuickStackAnimationStackSpeed**: For quick stack animation: Adjusts the speed at which items are individually stacked into their stored chest.

### Favorite Items
 - **IsEnableFavoriteItems**: If enabled, items in your inventory can be favorited. Favorited items will be ignored when stacking into chests.
 - **FavoriteItemsHighlightTextureChoice**: Choose your preferred texture style for highlighting favorited items in your inventory.
 - **FavoriteItemsKeyboardHotkey**: Hold this key when selecting an item to favorite it.
 - **FavoriteItemsControllerHotkey**: Hold this button when selecting an item to favorite it. (For controller support)
 
### Take All But One
 - **IsEnableTakeAllButOne**: If enabled, while holding the keybind below, instantly take all but one item from the selected item stack, rather than taking a single item at a time.
 - **TakeAllButOneKeyboardHotkey**: Hold this key when taking an item from an item stack to instantly take all but one of that item.
 - **TakeAllButOneControllerHotkey**: Hold this button when taking an item from an item stack to instantly take all but one of that item. (For controller support)

### Auto Organize Chest
 - **IsEnableAutoOrganizeChest**: If enabled, right-clicking a chest menu's Organize button (or pressing (X) on gamepad) will toggle Auto Organize ON/OFF for that chest. When a chest has Auto Organize ON, all of its items will remain organized, even when new items are added.
 - **IsShowAutoOrganizeButtonInstructions**: If enabled, the hover text for the Auto Organize button will include an additional line explaining how to toggle off Auto Organize.

### Miscellaneous
 - **IsEnableInventoryPageSideWarp**: If enabled, moving your controller's cursor beyond either side of your inventory menu will warp the cursor to the opposite side.

## Compatibility
 - Supports single player, split-screen local multiplayer, and online multiplayer.
 - Supports controllers by using the left shoulder button (configurable) for favoriting.
 - Supports [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) for in-game config editing.
 - Supports [Bigger Backpack](https://www.nexusmods.com/stardewvalley/mods/1845) and other inventory expansion mods (as of version 1.1.0).
 - Supports mods which directly add new inventory slots (as of version 1.3.0).
 - Supports Chests Anywhere﻿ by adding a new "Global" quick stack range option when installed (as of version 1.5.0).
 - Supports Wear More Rings by fixing edge case with favorite item logic due to it renaming ring equipment slots (as of version 1.5.0).﻿
 - Supports Content Patcher by using the SMAPI content pipeline for custom image assets loaded by this mod (as of version 1.5.3).
   - This allows for content packs to overwrite the custom image assets loaded by this mod.
