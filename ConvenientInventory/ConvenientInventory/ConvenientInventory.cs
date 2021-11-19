using ConvenientInventory.TypedChests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.Menus.ItemGrabMenu;

namespace ConvenientInventory
{
	internal static class CachedTextures
	{
		public static Texture2D Mill { get; } = Game1.content.Load<Texture2D>(@"Buildings\Mill");

		public static Texture2D JunimoHut { get; } = Game1.content.Load<Texture2D>(@"Buildings\Junimo Hut");

		public static Texture2D FarmHouse { get; } = Game1.content.Load<Texture2D>(@"Maps\farmhouse_tiles");
	}

	/*
	 * TODO: 
	 *	- Implement favorited items
	 *		- (DONE) Will be ignored by "Quick Stack To Nearby Chests" button.
	 *		- Patch the following methods which may interfere with favorite items functionality:
	 *			- Chest: "Add To Existing Stacks" button
	 *				- ignore favorited items
	 *			- Inventory: "Organize" button
	 *				- ignore favorited items
	 *				- Prefix/Transpile ItemGrabMenu.organizeItemsInList
	 *			- ...
	 *		- (ABANDONED) Prevents being sold to shops
	 *		- (DONE) Prevents being dropped/trashed in inventory
	 *			- Prefix Item.canBeTrashed
	 *		- (DONE) Prevent click action being performed on item when toggling favorite
	 *		- (DONE) Draw an icon in item tooltip post-draw, to better convey an item is favorited (especially if the item is large and covers most of its item slot)
	 *		- (DONE) Only allow favoriting item slots containing an item.
	 *			- Still allow un-favoriting empty item slots, as they may appear unintentionally (I'm not perfect)
	 *		- (2/3) Favorited item slot should "stick" to item in inventory.
	 *			- (DONE) If item is selected, temporarily remove (and track) favorited item slot.
	 *			- (DONE) When item is placed into a different slot, the favorited item slot should be reapplied to the new slot.
	 *			- Handle shift left-click; it moves item to first available slot in inventory.
	 *		- Find cases that might remove item from inventory. If item is removed, its respective favorited item slot should also be removed.
	 *			- i.e.:
	 *				- Eating
	 *				- Gifting
	 *				- ...
	 *			- (ABANDONED) Maybe just patch Item method that reduces stack count, if possible
	 *				- Try patching Item.Stack's Set method
	 *	- Implement quick-switch, where pressing hotbar key while hovering over an item will swap the currently hovered item with the item in the pressed hotbar key's position
	 */
	public static class ConvenientInventory
	{
		public static Texture2D QuickStackButtonIcon { private get; set; }

		private static IReadOnlyList<TypedChest> NearbyTypedChests { get; set; }

		private static ClickableTextureComponent QuickStackButton { get; set; }

		private static InventoryPage Page { get; set; }

		private static bool IsDrawToolTip { get; set; } = false;

		private const int quickStackButtonID = 918021;  // Unique indentifier

		private static readonly List<TransferredItemSprite> transferredItemSprites = new List<TransferredItemSprite>();

		public static int? PlayerInventoryExpandedSize { get; set; } = null;  // For supporting mods which expand player inventory size

		public static Texture2D FavoriteItemsCursorTexture { private get; set; }

		public static Texture2D FavoriteItemsHighlightTexture { private get; set; }

		public static Texture2D FavoriteItemsBorderTexture { private get; set; }

		public static bool IsFavoriteItemsHotkeyDown { get; set; }

		private static int favoriteItemsHotkeyDownCounter = 0;

		private static readonly string favoriteItemSlotsModDataKey = $"{ModEntry.Context.ModManifest.UniqueID}/favoriteItemSlots";

		private static bool[] favoriteItemSlots;

		public static bool[] FavoriteItemSlots
		{
			get { return favoriteItemSlots ?? LoadFavoriteItemSlots(); }
			set { favoriteItemSlots = value; }
		}

		public static bool FavoriteItemsIsItemSelected { get; set; }

		public static int FavoriteItemsLastSelectedSlot { get; set; } = -1;

		public static bool[] LoadFavoriteItemSlots()
		{
			Game1.player.modData.TryGetValue(favoriteItemSlotsModDataKey, out string dataStr);
			
			FavoriteItemSlots = dataStr?
				.Select(x => x == '1')
				.ToArray()
				?? new bool[Farmer.maxInventorySpace];

			// TODO: Is this a good a way of getting max inventory size (for inventory expansion compatibility)?
			//		 For now, support inventory expansion mods manually by detecting them and setting PlayerInventoryExpandedSize to the new value.
			//		 Will have to make a new modData entry "favoriteItemSlotsExpanded" to support extra slots and not interfere with original inventory.

			dataStr = dataStr ?? new string('0', FavoriteItemSlots.Length);
			ModEntry.Context.Monitor.Log($"Favorite item slots loaded for {Game1.player.Name}: '{dataStr}'.", StardewModdingAPI.LogLevel.Trace);
			return FavoriteItemSlots;
		}

		public static string SaveFavoriteItemSlots()
		{
			if (FavoriteItemSlots is null)
            {
				LoadFavoriteItemSlots();
			}

			var saveStr = new string(FavoriteItemSlots.Select(x => x ? '1' : '0').ToArray());

			Game1.player.modData[favoriteItemSlotsModDataKey] = saveStr;

			ModEntry.Context.Monitor.Log($"Favorite item slots saved to {Game1.player.Name}.modData: '{saveStr}'.", StardewModdingAPI.LogLevel.Trace);
			return saveStr;
		}

		public static void Constructor(InventoryPage inventoryPage, int x, int y, int width, int height)
		{
			Page = inventoryPage;

			if (ModEntry.Config.IsEnableQuickStack)
            {
				QuickStackButton = new ClickableTextureComponent("",
					new Rectangle(inventoryPage.xPositionOnScreen + width, inventoryPage.yPositionOnScreen + height / 3 - 64 + 8 + 80, 64, 64),
					string.Empty,
					"Quick Stack To Nearby Chests",
					QuickStackButtonIcon,
					Rectangle.Empty,
					4f,
					false)
				{
					myID = quickStackButtonID,
					downNeighborID = 105,  // trash can
					upNeighborID = 106,  // organize button
					leftNeighborID = 11  // top-right inventory slot
				};

				inventoryPage.organizeButton.downNeighborID = quickStackButtonID;
				inventoryPage.trashCan.upNeighborID = quickStackButtonID;
			}
		}

		public static bool PreReceiveLeftClickInMenu<T>(T menu, int x, int y) where T : IClickableMenu
		{
			if (ModEntry.Config.IsEnableFavoriteItems)
			{
				InventoryMenu inventory = (menu as InventoryPage)?.inventory    // Player menu - inventory tab
					?? (menu as CraftingPage)?.inventory                        // Player menu - crafting tab
					?? (menu as ShopMenu)?.inventory							// Shop menu
					?? (menu as MenuWithInventory)?.inventory;                  // Arbitrary menu


				if (IsFavoriteItemsHotkeyDown)
				{
					return !ToggleFavoriteItemSlotAtClickPosition(inventory, x, y);
				}
                else
                {
					TrackSelectedFavoriteItemSlotAtClickPosition(inventory, x, y);
                }
			}

			return true;
		}

		public static bool PreReceiveRightClickInMenu<T>(T menu, int x, int y) where T : IClickableMenu
		{
			if (ModEntry.Config.IsEnableFavoriteItems)
			{
				if (IsFavoriteItemsHotkeyDown)
				{
					return false;
				}

				InventoryMenu inventory = (menu as InventoryPage)?.inventory    // Player menu - inventory tab
					?? (menu as CraftingPage)?.inventory                        // Player menu - crafting tab
					?? (menu as ShopMenu)?.inventory                            // Shop menu
					?? (menu as MenuWithInventory)?.inventory;                  // Arbitrary menu

				TrackSelectedFavoriteItemSlotAtClickPosition(inventory, x, y, isRightClick: true);
			}

			return true;
		}

		// Toggles the favorited status of a selected item slot. Returns whether an item was toggled.
		private static bool ToggleFavoriteItemSlotAtClickPosition(InventoryMenu inventoryMenu, int clickX, int clickY, bool? favoriteOverride = null)
		{
			if (inventoryMenu is null)
			{
				return false;
			}

			int clickPos = inventoryMenu.getInventoryPositionOfClick(clickX, clickY);

			// Only allow favoriting if selected slot contains an item. Always allow unfavoriting.
			if (clickPos != -1 && inventoryMenu.actualInventory.Count > clickPos && (FavoriteItemSlots[clickPos] || inventoryMenu.actualInventory[clickPos] != null))
            {
                ModEntry.Context.Monitor
					.Log($"{(FavoriteItemSlots[clickPos] ? "Un-" : string.Empty)}Favorited item slot {clickPos}: {inventoryMenu.actualInventory[clickPos]?.Name}",
                	StardewModdingAPI.LogLevel.Trace);

				Game1.playSound("smallSelect");

				FavoriteItemSlots[clickPos] = (favoriteOverride is null)
					? !FavoriteItemSlots[clickPos]
					: favoriteOverride.Value;

				return true;
            }

			return false;
        }

		// Tracks and "moves" favorite item slots when selecting/de-selecting items in an inventory menu.
		private static bool TrackSelectedFavoriteItemSlotAtClickPosition(InventoryMenu inventoryMenu, int clickX, int clickY, bool isRightClick = false)
		{
			if (!ModEntry.Config.IsEnableFavoriteItems || IsFavoriteItemsHotkeyDown || inventoryMenu is null)
			{
				return false;
			}

			int clickPos = inventoryMenu.getInventoryPositionOfClick(clickX, clickY);

			if (!FavoriteItemsIsItemSelected || isRightClick)
			{
				// Check that we've selected a favorited item slot
				if (clickPos != -1 && inventoryMenu.actualInventory.Count > clickPos && inventoryMenu.actualInventory[clickPos] != null && FavoriteItemSlots[clickPos]) 
				{
					if (!isRightClick && Game1.player.CursorSlotItem is null)
					{
						// Left click, with no item currently selected
						if (!IsCurrentActiveMenuNoHeldItems())
						{
							FavoriteItemsLastSelectedSlot = clickPos;
							FavoriteItemsIsItemSelected = true;
							FavoriteItemSlots[clickPos] = false;
						}
						else
						{
							// Shop menus only allow held items after purchasing something, so we check for that case here.
							if (Game1.activeClickableMenu is ShopMenu shopMenu)
							{
								if (shopMenu.heldItem == null && inventoryMenu.highlightMethod(inventoryMenu.actualInventory[clickPos]))
								{
									FavoriteItemSlots[clickPos] = false;
								}
							}
							else
							{
								FavoriteItemSlots[clickPos] = false;
							}
						}
					}
					else if (isRightClick && inventoryMenu.actualInventory[clickPos].Stack == 1)
					{
						// Right click, taking the last item
						if (!IsCurrentActiveMenuNoHeldItems())
						{
							FavoriteItemsLastSelectedSlot = clickPos;
							FavoriteItemsIsItemSelected = true;
							FavoriteItemSlots[clickPos] = false;
						}
						else
						{
							// Shop menus only allow held items after purchasing something, so we check for that case here.
							if (Game1.activeClickableMenu is ShopMenu shopMenu)
							{
								if ((shopMenu.heldItem == null || shopMenu.heldItem.canStackWith(inventoryMenu.actualInventory[clickPos]))
									&& inventoryMenu.highlightMethod(inventoryMenu.actualInventory[clickPos]))
								{
									FavoriteItemsLastSelectedSlot = clickPos;
									FavoriteItemsIsItemSelected = true;
									FavoriteItemSlots[clickPos] = false;
								}
							}
							else
							{
								FavoriteItemSlots[clickPos] = false;
							}
						}
					}
				}
			}
			else
			{
				if (clickPos != -1 && inventoryMenu.actualInventory.Count > clickPos && !isRightClick)
				{
					FavoriteItemsLastSelectedSlot = -1;
					FavoriteItemsIsItemSelected = false;

					if (!FavoriteItemSlots[clickPos])
					{
						// We are placing the selected item into a non-favorited slot, so favorite this new one.
						FavoriteItemSlots[clickPos] = true;
					}
				}
			}

			return true;
		}

		// Checks for menus which don't allow selecting and holding items, i.e. chests, shipping bins, shops, etc.
		private static bool IsCurrentActiveMenuNoHeldItems()
        {
			bool result = Game1.activeClickableMenu is ItemGrabMenu
				|| Game1.activeClickableMenu is ShopMenu;

			return result;
		}

		public static void PostReceiveLeftClickInMenu<T>(T menu, int x, int y) where T : IClickableMenu
		{
			// Quick stack button clicked (in InventoryPage)
			if (ModEntry.Config.IsEnableQuickStack && menu is InventoryPage inventoryPage && QuickStackButton != null && QuickStackButton.containsPoint(x, y))
            {
				QuickStackLogic.StackToNearbyChests(ModEntry.Config.QuickStackRange, inventoryPage);
			}
		}

		// Player shifted toolbar row, so shift all favorited item slots by a row
		public static void ShiftToolbar(bool right)
		{
			RotateArray(FavoriteItemSlots, 12, right);
		}

		private static void RotateArray(bool[] arr, int count, bool right)
		{
			count %= arr.Length;
			if (count == 0) return;

			bool[] toMove = (bool[]) arr.Clone();

			if (right)
			{
				// SDV "right" => left shift
				for (int i = 0; i < arr.Length - count; i++)
				{
					arr[i] = arr[i + count];
				}
				for (int i = 0; i < count; i++)
				{
					arr[arr.Length - count + i] = toMove[i];
				}

				return;
			}

			// SDV "left" => right shift
			for (int i = 0; i < arr.Length - count; i++)
			{
				arr[arr.Length - 1 - i] = arr[arr.Length - 1 - i - count];
			}
			for (int i = 0; i < count; i++)
			{
				arr[i] = toMove[arr.Length - count + i];
			}

			return;
		}

		public static void PerformHoverActionInInventoryPage(int x, int y)
		{
			if (ModEntry.Config.IsEnableQuickStack)
			{
				QuickStackButton.tryHover(x, y);
				IsDrawToolTip = QuickStackButton.containsPoint(x, y);
			}
		}

		public static void PopulateClickableComponentsListInInventoryPage(InventoryPage inventoryPage)
		{
			if (ModEntry.Config.IsEnableQuickStack)
			{
				inventoryPage.allClickableComponents.Add(QuickStackButton);
			}
		}

		public static bool IsPlayerInventory(InventoryMenu inventoryMenu)
		{
			bool result = inventoryMenu.playerInventory
				|| (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.pages?[gameMenu.currentTab] is CraftingPage)  // CraftingPage.inventory has playerInventory = false
				|| (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu && itemGrabMenu.inventory == inventoryMenu)  // ItemGrabMenu.inventory is the player's InventoryMenu
				|| (Game1.activeClickableMenu is ShopMenu);

			return result;
		}

		// TODO: Should also figure out when toolbar items are drawn, and prefix that draw method as well (assuming it is not simply using InventoryMenu.draw()).
		//		  - Toolbar class

		// Called after drawing everything else in arbitrary inventory menu.
		// Draws favorite items cursor if keybind is being pressed.
		public static void PostMenuDraw<T>(T menu, SpriteBatch spriteBatch) where T : IClickableMenu
		{
			// Draw quick stack button tooltip (in InventoryPage)
			if (ModEntry.Config.IsEnableQuickStack && menu is InventoryPage && IsDrawToolTip)
			{
				DrawQuickStackButtonToolTip(spriteBatch);
			}

			if (ModEntry.Config.IsEnableFavoriteItems)
			{
				// Get inventory if menu has one
				InventoryMenu inventory = (menu as InventoryMenu)   // Inventory item slots container
					?? (menu as InventoryPage)?.inventory           // Player menu - inventory tab
					?? (menu as CraftingPage)?.inventory            // Player menu - crafting tab
					?? (menu as ShopMenu)?.inventory				// Shop menu
					?? (menu as MenuWithInventory)?.inventory;      // Arbitrary menu

				// Draw favorite cursor (unless this is an InventoryMenu)
				if (inventory != null && !(menu is InventoryMenu))
				{
					if (IsFavoriteItemsHotkeyDown)
					{
						DrawFavoriteItemsCursor(spriteBatch);
						favoriteItemsHotkeyDownCounter++;
					}
					else
					{
						favoriteItemsHotkeyDownCounter = 0;
					}
				}
			}
		}

        private static void DrawQuickStackButtonToolTip(SpriteBatch spriteBatch)
        {
            NearbyTypedChests = QuickStackLogic.GetTypedChestsAroundFarmer(Game1.player, ModEntry.Config.QuickStackRange, true).AsReadOnly();

            if (ModEntry.Config.IsQuickStackTooltipDrawNearbyChests)
            {
                int numPos = ModEntry.Config.IsQuickStackIntoBuildingsWithInventories
                    ? NearbyTypedChests.Count + GetExtraNumPosUsedByBuildingChests(NearbyTypedChests)
                    : NearbyTypedChests.Count;

                var text = QuickStackButton.hoverText + new string('\n', 2 * ((numPos + 7) / 8));  // Draw two newlines for each row of chests
                IClickableMenu.drawToolTip(spriteBatch, text, string.Empty, null, false, -1, 0, -1, -1, null, -1);

                DrawTypedChestsInToolTip(spriteBatch, NearbyTypedChests);
            }
            else
            {
                IClickableMenu.drawToolTip(spriteBatch, QuickStackButton.hoverText + $" ({NearbyTypedChests.Count})", string.Empty, null, false, -1, 0, -1, -1, null, -1);
            }
        }

		public static void DrawFavoriteItemSlotHighlights(SpriteBatch spriteBatch, InventoryMenu inventoryMenu)
        {
			if (inventoryMenu is null)
            {
				return;
            }

			List<Vector2> slotDrawPositions = inventoryMenu.GetSlotDrawPositions();

            for (int i = 0; i < slotDrawPositions.Count; i++)
            {
                if (!FavoriteItemSlots[i])
                {
                    continue;
                }

                spriteBatch.Draw(FavoriteItemsHighlightTexture,
                    slotDrawPositions[i],
                    new Rectangle(0, 0, FavoriteItemsHighlightTexture.Width, FavoriteItemsHighlightTexture.Height),
                    Color.White,
                    0f, Vector2.Zero, 4f, SpriteEffects.None, 1f
                );
            }
        }

		private static void DrawFavoriteItemsCursor(SpriteBatch spriteBatch)
		{
			float scale = (float)(3d + 0.15d * Math.Cos(favoriteItemsHotkeyDownCounter / 15d));

			spriteBatch.Draw(FavoriteItemsCursorTexture,
			   new Vector2(Game1.getOldMouseX() - 32, Game1.getOldMouseY()),
			   new Rectangle(0, 0, FavoriteItemsCursorTexture.Width, FavoriteItemsCursorTexture.Height),
			   Color.White,
			   0f, Vector2.Zero, scale, SpriteEffects.None, 1f
		   );
		}

		public static void DrawFavoriteItemsToolTipBorder(Item item, SpriteBatch spriteBatch, int x, int y)
		{
			int index = GetPlayerInventoryIndexOfItem(item);

			if (ModEntry.Config.IsEnableFavoriteItems && index != -1 && FavoriteItemSlots[index])
			{
				spriteBatch.Draw(FavoriteItemsBorderTexture,
					new Vector2(x, y),
					new Rectangle(0, 0, FavoriteItemsBorderTexture.Width, FavoriteItemsBorderTexture.Height),
					Color.White,
					0f, Vector2.Zero, 4f, SpriteEffects.None, 1f
				);
			}
		}

		public static int GetPlayerInventoryIndexOfItem(Item item)
		{
			if (item is null)
            {
				return -1;
            }

			for (int i = 0; i < Game1.player.Items.Count; i++)
			{
				if (Game1.player.Items[i] == item && Game1.player.Items[i] != null && item != null)
				{
					return i;
				}
			}

			return -1;
		}

		public static void PostClickableTextureComponentDraw(ClickableTextureComponent textureComponent, SpriteBatch spriteBatch)
		{
			// Check if we have just drawn the trash can for this inventory page, which happens before in-game tooltip is drawn.
			if (Page?.trashCan != textureComponent)
            {
				return;
            }

			if (ModEntry.Config.IsEnableQuickStack)
			{
				// Draw transferred item sprites
				foreach (TransferredItemSprite transferredItemSprite in transferredItemSprites)
				{
					transferredItemSprite.Draw(spriteBatch);
				}

				QuickStackButton?.draw(spriteBatch);
			}
		}

        private static int GetExtraNumPosUsedByBuildingChests(IReadOnlyList<TypedChest> chests)
        {
			int extraNumPos = 0;

			for (int i=0; i<chests.Count; i++)
            {
				if (chests[i] is null)
                {
					continue;
                }

				extraNumPos += TypedChest.IsBuildingChestType(chests[i].ChestType)
					? 1 + ((i % 8 == 7) ? 1 : 0)
					: 0;
            }

			return extraNumPos;
        }

		private static void DrawTypedChestsInToolTip(SpriteBatch spriteBatch, IReadOnlyList<TypedChest> typedChests)
		{
			Point toolTipPosition = GetToolTipDrawPosition(QuickStackButton.hoverText);

			for (int i = 0, pos = 0; i < typedChests.Count; i++, pos++)
			{
				pos += typedChests[i]?.DrawInToolTip(spriteBatch, toolTipPosition, pos) ?? 0;
			}
		}

		// Refactored from IClickableMenu drawHoverText()
		// Finds the origin position of where a tooltip will be drawn.
		private static Point GetToolTipDrawPosition(string text)
        {
			int width = (int)Game1.smallFont.MeasureString(text).X + 32;
			int height = Math.Max(20 * 3, (int)Game1.smallFont.MeasureString(text).Y + 32 + 8);

			int x = Game1.getOldMouseX() + 32;
			int y = Game1.getOldMouseY() + 32;
			
			if (x + width > Utility.getSafeArea().Right)
			{
				x = Utility.getSafeArea().Right - width;
				y += 16;
			}
			if (y + height > Utility.getSafeArea().Bottom)
			{
				x += 16;
				if (x + width > Utility.getSafeArea().Right)
				{
					x = Utility.getSafeArea().Right - width;
				}
				y = Utility.getSafeArea().Bottom - height;
			}

			return new Point(x, y);
		}

        // Updates transferredItemSprite animation
        public static void Update(GameTime time)
		{
			for (int i = 0; i < transferredItemSprites.Count; i++)
			{
				if (transferredItemSprites[i].Update(time))
				{
					transferredItemSprites.RemoveAt(i);
					i--;
				}
			}
		}

		public static void AddTransferredItemSprite(TransferredItemSprite itemSprite)
		{
			transferredItemSprites.Add(itemSprite);
		}
	}
}
