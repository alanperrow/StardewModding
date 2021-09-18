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
		private const int QuickStackButtonID = 918021;  // Unique indentifier
		private static readonly List<TransferredItemSprite> TransferredItemSprites = new List<TransferredItemSprite>();

		internal static Texture2D QuickStackButtonIcon { private get; set; }

		internal static IReadOnlyList<Chest> NearbyChests { get; set; }

		private static ClickableTextureComponent QuickStackButton { get; set; }

		private static InventoryPage Page { get; set; }

		private static bool IsDrawToolTip { get; set; } = false;

		private static bool PrevIsDrawToolTip { get; set; } = false;

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
				myID = QuickStackButtonID,
				downNeighborID = 105,  // trash can
				upNeighborID = 106,  // organize button
				leftNeighborID = 11  // top-right inventory slot
			};

			inventoryPage.organizeButton.downNeighborID = QuickStackButtonID;
			inventoryPage.trashCan.upNeighborID = QuickStackButtonID;
		}

		public static void ReceiveLeftClick(int x, int y)
		{
			if (QuickStackButton != null && QuickStackButton.containsPoint(x, y))
			{
				QuickStackLogic.StackToNearbyChests(ModEntry.Config.Range, Page);
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
				foreach (TransferredItemSprite transferredItemSprite in TransferredItemSprites)
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
			if (IsDrawToolTip && !PrevIsDrawToolTip)
			{
				NearbyChests = QuickStackLogic.GetChestsAroundFarmer(Game1.player, ModEntry.Config.Range, true).AsReadOnly();

				// DEBUG
				ModEntry.Context.Monitor.Log($"NearbyChests populated with {NearbyChests.Count} chests.", StardewModdingAPI.LogLevel.Debug);
			}

			if (IsDrawToolTip)
			{
				IClickableMenu.drawToolTip(spriteBatch, QuickStackButton.hoverText, string.Empty, null, false, -1, 0, /*166*/-1, -1, null, -1);

				/*
				 * TODO: Draw preview of all chests/inventories in range. (Should also show chest colors, fridge, hut, etc...)
				 *		 - May need to override Chest.draw() or make custom draw function. Currently cannot scale *and* display correctly colored sprite.
				 *		 - layerDepth??? Can't seem to draw on top of tooltip :(
				 *		     - Look into how "extraItemToShow" works in drawToolTip method, and see if that same logic can be used to draw on top of tooltip directly.
				 */
				for (int i=0; i < NearbyChests.Count; i++)
                {
					NearbyChests[i]?.drawInMenu(spriteBatch, new Vector2(900 + i*36, 400), 1f, 1f, 0.9f, StackDrawType.Hide, NearbyChests[i].playerChoiceColor.Value, true);

					//draw(spriteBatch, Page.xPositionOnScreen + Page.width + (IClickableMenu.borderWidth / 2) - 64, Page.yPositionOnScreen + 16 + 128 + (i * 64), 1f, true);
				}
			}
		}

		// Used to update transferredItemSprite animation
		public static void Update(GameTime time)
		{
			for (int i = 0; i < TransferredItemSprites.Count; i++)
			{
				if (TransferredItemSprites[i].Update(time))
				{
					TransferredItemSprites.RemoveAt(i);
					i--;
				}
			}
		}

		public static void AddTransferredItemSprite(TransferredItemSprite itemSprite)
		{
			TransferredItemSprites.Add(itemSprite);
		}
	}
}
