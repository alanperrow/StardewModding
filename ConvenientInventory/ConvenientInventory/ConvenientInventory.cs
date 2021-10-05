using ConvenientInventory.TypedChests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using static StardewValley.Menus.ItemGrabMenu;

namespace ConvenientInventory
{
	/*
	 * TODO: Implement favorited items, which will be ignored by "Quick Stack To Nearby Chests" button.
	 *       Prefix "Add To Existing Stacks" button logic to ignore favorited items as well.
	 */
	internal class ConvenientInventory
	{
		internal static Texture2D QuickStackButtonIcon { private get; set; }

		internal static IReadOnlyList<TypedChest> NearbyTypedChests { get; set; }

		private static ClickableTextureComponent QuickStackButton { get; set; }

		private static InventoryPage Page { get; set; }

		private static bool IsDrawToolTip { get; set; } = false;

		private const int quickStackButtonID = 918021;  // Unique indentifier
		private static readonly List<TransferredItemSprite> transferredItemSprites = new List<TransferredItemSprite>();

		public static void Constructor(InventoryPage inventoryPage, int x, int y, int width, int height)
		{
			Page = inventoryPage;

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

		public static void ReceiveLeftClick(int x, int y)
		{
			if (QuickStackButton != null && QuickStackButton.containsPoint(x, y))
			{
				QuickStackLogic.StackToNearbyChests(ModEntry.Config.QuickStackRange, Page);
			}
		}

		public static void PerformHoverAction(int x, int y)
		{
			QuickStackButton.tryHover(x, y);
			IsDrawToolTip = QuickStackButton.containsPoint(x, y);
		}

		public static void PopulateClickableComponentsList(InventoryPage inventoryPage)
		{
			inventoryPage.allClickableComponents.Add(QuickStackButton);
		}

		// Called before drawing tooltip. Use for drawing the button.
		public static void TrashCanDrawn(ClickableTextureComponent textureComponent, SpriteBatch spriteBatch)
		{
			if (Page?.trashCan == textureComponent)
			{
				// Draw transferred item sprites
				foreach (TransferredItemSprite transferredItemSprite in transferredItemSprites)
				{
					transferredItemSprite.Draw(spriteBatch);
				}

				QuickStackButton?.draw(spriteBatch);
			}
		}


		// Called after drawing everything else in InventoryPage. Use for drawing tooltip.
		public static void PostDraw(SpriteBatch spriteBatch)
		{
			if (!IsDrawToolTip)
            {
				return;
            }

			NearbyTypedChests = QuickStackLogic.GetTypedChestsAroundFarmer(Game1.player, ModEntry.Config.QuickStackRange, true).AsReadOnly();

			if (ModEntry.Config.IsQuickStackTooltipDrawNearbyChests)
			{
				var text = QuickStackButton.hoverText + new string('\n', 2 * ((NearbyTypedChests.Count + 7) / 8));  // Draw two newlines for each row of chests
				IClickableMenu.drawToolTip(spriteBatch, text, string.Empty, null, false, -1, 0, /*166*/-1, -1, null, -1);
				DrawTypedChestsInToolTip(spriteBatch, NearbyTypedChests);
			}
            else
            {
				IClickableMenu.drawToolTip(spriteBatch, QuickStackButton.hoverText + $" ({NearbyTypedChests.Count})", string.Empty, null, false, -1, 0, -1, -1, null, -1);
			}
		}

		private static void DrawTypedChestsInToolTip(SpriteBatch spriteBatch, IReadOnlyList<TypedChest> typedChests)
		{
			/*
			 * TODO: Draw preview of all chests/inventories in range. (Should also show chest colors, fridge, hut, etc...)
			 *		 - May need to override Chest.draw() or make custom draw function. Currently cannot scale *and* display correctly colored sprite.
			 *		 - layerDepth??? Can't seem to draw on top of tooltip :(
			 *		     - Look into how "extraItemToShow" works in drawToolTip method, and see if that same logic can be used to draw on top of tooltip directly.
			 *		 
			 *		 
			 * 9/19: Chests and stone chests are drawn perfectly - colored and show trim.
			 * TODO:
			 * - If chest is not wooden chest/stone chest, do NOT draw trim. (Do junimo chests draw trim?)
			 * - Kitchen fridge and building chests display as normal chests. Fixing would require a modification to QuickStackLogic.GetChestsAroundFarmer
			 * - OR new method QuickStackLogic.GetTypedChestsAroundFarmer which returns a List<TypedChest>, each object containing the Chest and its respective Type enum value
			 *     - I.e., Default, Stone, Fridge, MiniFridge, Mill, JunimoHut, Special (x-ref chest.SpecialChestTypes when this is the case)
			 *	   - Display chests under tooltip in a new "section", with horizontal divider between tooltip and new section.
			 *	       - X chests per row, nearest chests shown first starting @ top-left of section.
			 *		   - When row is full, start new row below.
			 *		   - Each chest should be moderately spaced apart (drawing buildings might cause trouble... take more space? scale smaller? custom icons?)
			 *		       - Maybe for-loop with separate "horizontal space" counter that += 1 with normal chests, but += 2 with buildings so they take up more space
			 *		 
			 *	9/30: Created TypedChest class, so we can now draw each chest type differently.
			 *		  For each row of chests, add 2 newlines ('\n') to QuickStackButton.hoverText. This gives just enough space for chests to be drawn in that row.
			 *		  Started working on drawing chests in correct position(s) in tooltip.
			 *		  
			 *	10/1: Finished GetToolTipDrawPosition method.
			 *	
			 *	10/5: Implemented GetChestType method in TypedChest.
			 */

			// TODO: Find offset of this position for first chest to be drawn. After that, simple iteration, will be easy
			Point toolTipPosition = GetToolTipDrawPosition(QuickStackButton.hoverText);

			for (int i = 0, pos = 0; i < typedChests.Count; i++, pos++)
			{
				var chest = typedChests[i].Chest;
				var chestType = typedChests[i].ChestType;

				if (chest is null)
                {
					continue;
                }

				int offsetX = toolTipPosition.X + 20 + 46 * (i % 8);
				int offsetY = toolTipPosition.Y + 40 + 52 * (i / 8);

				switch (chestType)
                {
					case ChestType.Normal:
					default:

						// TODO: Split this logic into ChestType.Stone case. Currently handles both Normal and Stone.

						spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
							new Vector2(
								offsetX,
								offsetY
							),
							Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, (chest.ParentSheetIndex == 130 && !chest.playerChoiceColor.Value.Equals(Color.Black)) ? 168 : chest.ParentSheetIndex, 16, 32),
							chest.playerChoiceColor.Value.Equals(Color.Black) ? chest.Tint : chest.playerChoiceColor.Value,
							0f, Vector2.Zero, 2f, SpriteEffects.None, 1f
						);

						spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
							new Vector2(
								offsetX,
								offsetY + 42
							),
							new Rectangle(0, ((chest.ParentSheetIndex == 130) ? 168 : chest.ParentSheetIndex) / 8 * 32 + 53, 16, 11),
							Color.White,
							0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 1E-05f
						);

						spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
							new Vector2(
								offsetX,
								offsetY
							),
							Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, (chest.ParentSheetIndex == 130) ? chest.startingLidFrame.Value + 46 : chest.startingLidFrame.Value + 8, 16, 32),
							Color.White,
							0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 2E-05f
						);

						break;
					case ChestType.Stone:

						break;
					case ChestType.Fridge:

						break;
					case ChestType.MiniFridge:

						break;
					case ChestType.Mill:

						break;
					case ChestType.JunimoHut:

						break;
					case ChestType.Special:

						break;
				}
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

        // Used to update transferredItemSprite animation
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
