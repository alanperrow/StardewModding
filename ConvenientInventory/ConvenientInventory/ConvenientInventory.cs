using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
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

		internal static IReadOnlyList<Chest> NearbyChests { get; set; }

		private static ClickableTextureComponent QuickStackButton { get; set; }

		private static InventoryPage Page { get; set; }

		private static bool IsDrawToolTip { get; set; } = false;

		private static bool PrevIsDrawToolTip { get; set; } = false;

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
			PrevIsDrawToolTip = IsDrawToolTip;
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
			// Optimization: Only update nearby chests once upon initial hover over quick stack button
			// On second thought, this doesn't handle edge cases very well (co-op chest usage, closing & reopening menu, etc)
			if (IsDrawToolTip/* && !PrevIsDrawToolTip*/)
			{
				NearbyChests = QuickStackLogic.GetChestsAroundFarmer(Game1.player, ModEntry.Config.QuickStackRange, true).AsReadOnly();

				// DEBUG
				//ModEntry.Context.Monitor.Log($"NearbyChests populated with {NearbyChests.Count} chests.", StardewModdingAPI.LogLevel.Debug);
			}

			if (IsDrawToolTip)
			{
				IClickableMenu.drawToolTip(spriteBatch, QuickStackButton.hoverText + $" ({NearbyChests.Count})", string.Empty, null, false, -1, 0, /*166*/-1, -1, null, -1);

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
				 */
				if (ModEntry.Config.IsQuickStackTooltipDrawNearbyChests)
				{
					for (int i = 0; i < NearbyChests.Count; i++)
					{
						var chest = NearbyChests[i];

						spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
							new Vector2(
								Page.xPositionOnScreen + Page.width + (IClickableMenu.borderWidth / 2) - 64 + 192,
								Page.yPositionOnScreen + 16 + 256 + ((i % 2 == 0 ? (i + 1) / 2 : -(i + 1) / 2) * 64)
							),
							Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, (chest.ParentSheetIndex == 130 && !chest.playerChoiceColor.Value.Equals(Color.Black)) ? 168 : chest.ParentSheetIndex, 16, 32),
							chest.playerChoiceColor.Value.Equals(Color.Black) ? chest.Tint : chest.playerChoiceColor.Value,
							0f, Vector2.Zero, 2f, SpriteEffects.None, 1E-05f
						);

						spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
							new Vector2(
								Page.xPositionOnScreen + Page.width + (IClickableMenu.borderWidth / 2) - 64 + 192,
								Page.yPositionOnScreen + 16 + 256 + 42 + ((i % 2 == 0 ? (i + 1) / 2 : -(i + 1) / 2) * 64)
							),
							new Rectangle(0, ((chest.ParentSheetIndex == 130) ? 168 : chest.ParentSheetIndex) / 8 * 32 + 53, 16, 11),
							Color.White,
							0f, Vector2.Zero, 2f, SpriteEffects.None, 3E-05f
						);

						spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
							new Vector2(
								Page.xPositionOnScreen + Page.width + (IClickableMenu.borderWidth / 2) - 64 + 192,
								Page.yPositionOnScreen + 16 + 256 + ((i % 2 == 0 ? (i + 1) / 2 : -(i + 1) / 2) * 64)
							),
							Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, (chest.ParentSheetIndex == 130) ? chest.startingLidFrame.Value + 46 : chest.startingLidFrame.Value + 8, 16, 32),
							Color.White,
							0f, Vector2.Zero, 2f, SpriteEffects.None, 4E-05f
						);
					}
				}
			}
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
