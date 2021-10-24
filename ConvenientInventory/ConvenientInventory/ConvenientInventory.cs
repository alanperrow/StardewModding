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
	 *		- Prefix "Add To Existing Stacks" button logic to ignore favorited items as well.
	 *		- (?) Prevents being sold to shops
	 *		- (?) Prevents being dropped from inventory
	 *		- (?) Prevents being trashed in inventory
	 *		- Draw an icon in item tooltip post-draw, to better convey an item is favorited (especially if the item is large and covers most of its item slot)
	 *			- Prefix item hover method? (if there is one, lol. Otherwise will need a new generic PostMenuPerformHoverAction method)
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

		public static bool IsFavoriteItemsHotkeyDown { get; set; } = false;

		private static int favoriteItemsHotkeyDownCounter = 0;

		private static readonly string favoriteItemSlotsModDataKey = $"{ModEntry.Context.ModManifest.UniqueID}/favoriteItemSlots";

		private static bool[] favoriteItemSlots;
		public static bool[] FavoriteItemSlots
		{
			get { return favoriteItemSlots ?? LoadFavoriteItemSlots(); }
			set { favoriteItemSlots = value; }
		}

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
		
		public static void ReceiveLeftClickInMenu<T>(T menu, int x, int y) where T : IClickableMenu
		{
			// Quick stack button clicked (in InventoryPage)
			if (menu is InventoryPage inventoryPage && ModEntry.Config.IsEnableQuickStack && QuickStackButton != null && QuickStackButton.containsPoint(x, y))
            {
				QuickStackLogic.StackToNearbyChests(ModEntry.Config.QuickStackRange, inventoryPage);
			}

			// TODO: Move to prefix method (InventoryMenu.LeftClick)
			// Try to favorite an item slot
			if (ModEntry.Config.IsEnableFavoriteItems && IsFavoriteItemsHotkeyDown)
			{
				InventoryMenu inventory = (menu as InventoryPage)?.inventory    // Player menu - inventory tab
					?? (menu as CraftingPage)?.inventory						// Player menu - crafting tab
					?? (menu as MenuWithInventory)?.inventory;					// Arbitrary menu

				if (inventory != null)
				{
					ToggleFavoriteItemsSlotAtClickPosition(inventory, x, y);
				}
            }
        }

		// Toggles the favorited status of a selected item slot.
        private static void ToggleFavoriteItemsSlotAtClickPosition(InventoryMenu inventoryMenu, int clickX, int clickY)
        {
            int clickPos = inventoryMenu.getInventoryPositionOfClick(clickX, clickY);

			// TODO: Only allow favoriting if selected slot contains an item.
			//		 Currently this doesn't work because the item gets picked up before we get to check. (See TODO above: "Move to prefix method")
			if (clickPos != -1 && inventoryMenu.actualInventory.Count > clickPos/* && inventoryMenu.actualInventory[clickPos] != null*/)
            {
                ModEntry.Context.Monitor
					.Log($"{(FavoriteItemSlots[clickPos] ? "Un-" : string.Empty)}Favorited item slot {clickPos}: {inventoryMenu.actualInventory[clickPos]?.Name}",
                	StardewModdingAPI.LogLevel.Trace);

				Game1.playSound("smallSelect");

                FavoriteItemSlots[clickPos] = !FavoriteItemSlots[clickPos];
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
			var result = inventoryMenu.playerInventory
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
