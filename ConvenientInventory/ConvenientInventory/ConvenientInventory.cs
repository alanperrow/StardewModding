﻿using ConvenientInventory.TypedChests;
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
	 *	- Implement favorited items, which will be ignored by "Quick Stack To Nearby Chests" button.
	 *		- Prefix "Add To Existing Stacks" button logic to ignore favorited items as well.
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
			ModEntry.Context.Monitor.Log($"Favorite item slots loaded for {Game1.player.Name}: '{dataStr}'.", StardewModdingAPI.LogLevel.Debug);
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

			ModEntry.Context.Monitor.Log($"Favorite item slots saved to {Game1.player.Name}.modData: '{saveStr}'.", StardewModdingAPI.LogLevel.Debug);
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

		// Backpack inventory page.
		public static void ReceiveLeftClick(InventoryPage inventoryPage, int x, int y)
		{
			if (ModEntry.Config.IsEnableQuickStack && QuickStackButton != null && QuickStackButton.containsPoint(x, y))
			{
				QuickStackLogic.StackToNearbyChests(ModEntry.Config.QuickStackRange, inventoryPage);

				return;
			}

			// TODO: Move to prefix method.
			//		 Favoriting an item should only toggle the favorited status, not select the item.
			if (ModEntry.Config.IsEnableFavoriteItems)
			{
				if (IsFavoriteItemsHotkeyDown)
                {
                    ToggleFavoriteItemsSlotAtClickPosition(inventoryPage.inventory, x, y);
                }
            }
		}

		// Crafting inventory page.
		public static void ReceiveLeftClick(CraftingPage craftingPage, int x, int y)
		{
			// TODO: Move to prefix method.
			//		 Favoriting an item should only toggle the favorited status, not select the item.
			if (ModEntry.Config.IsEnableFavoriteItems)
			{
				if (IsFavoriteItemsHotkeyDown)
				{
					ToggleFavoriteItemsSlotAtClickPosition(craftingPage.inventory, x, y);
				}
			}
		}

		// Arbitrary inventory menu with a section for the player's inventory.
		public static void ReceiveLeftClick(MenuWithInventory menuWithInventory, int x, int y)
		{
			// TODO: Move to prefix method.
			//		 Favoriting an item should only toggle the favorited status, not select the item.
			if (ModEntry.Config.IsEnableFavoriteItems)
			{
				if (IsFavoriteItemsHotkeyDown)
				{
					ToggleFavoriteItemsSlotAtClickPosition(menuWithInventory.inventory, x, y);
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

		public static void PerformHoverAction(int x, int y)
		{
			if (ModEntry.Config.IsEnableQuickStack)
			{
				QuickStackButton.tryHover(x, y);
				IsDrawToolTip = QuickStackButton.containsPoint(x, y);
			}
		}

		public static void PopulateClickableComponentsList(InventoryPage inventoryPage)
		{
			if (ModEntry.Config.IsEnableQuickStack)
			{
				inventoryPage.allClickableComponents.Add(QuickStackButton);
			}
		}

		// TODO: Try using "helper.Events.Display.RenderedActiveMenu" and "StardewValley.Game1.activeClickableMenu" for post-draw logic.
		// TODO: Should also figure out when toolbar items are drawn, and prefix that draw method as well (assuming it is not simply using InventoryMenu.draw()).
		//		  - Toolbar class

		// TODO: Draw highlight *underneath* items
		// Called after drawing everything else in arbitrary inventory menu.
		// Draws favorite item slot highlights, and favorite items cursor if keybind is being pressed.
		public static void PostMenuWithInventoryDraw(MenuWithInventory menuWithInventory, SpriteBatch spriteBatch)
        {
			if (ModEntry.Config.IsEnableFavoriteItems)
			{
				DrawFavoriteItemSlotHighlights(spriteBatch, menuWithInventory.inventory);

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

		// CraftingPage.inventory has playerInventory = false, so we manually check if this inventory is from CraftingPage.
		public static bool IsPlayerInventory(InventoryMenu inventoryMenu)
		{
			return inventoryMenu.playerInventory || (Game1.activeClickableMenu is GameMenu gameMenu && gameMenu.pages?[gameMenu.currentTab] is CraftingPage);
		}

		// Called after drawing everything else in player inventory section of arbitrary inventory menu.
		public static void PostInventoryDraw(InventoryMenu inventoryMenu, SpriteBatch spriteBatch)
		{
			if (ModEntry.Config.IsEnableFavoriteItems)
            {
				DrawFavoriteItemSlotHighlights(spriteBatch, inventoryMenu);
            }
        }

        public static void DrawFavoriteItemSlotHighlights(SpriteBatch spriteBatch, InventoryMenu inventoryMenu)
        {
			List<Vector2> slotDrawPositions = inventoryMenu?.GetSlotDrawPositions();

			if (slotDrawPositions is null)
            {
				return;
            }

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

		// Called after drawing everything else in InventoryPage. Draws tooltip. Draws favorite cursor.
		public static void PostInventoryPageDraw(SpriteBatch spriteBatch)
		{
			if (ModEntry.Config.IsEnableQuickStack && IsDrawToolTip)
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

			if (ModEntry.Config.IsEnableFavoriteItems)
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

		// Called after drawing everything else in CraftingPage. Draws favorite cursor.
		public static void PostCraftingPageDraw(SpriteBatch spriteBatch)
		{
			if (ModEntry.Config.IsEnableFavoriteItems)
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
