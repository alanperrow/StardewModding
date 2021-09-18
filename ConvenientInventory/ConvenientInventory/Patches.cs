using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using HarmonyLib;
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace ConvenientInventory.Patches
{
	public class InventoryPageConstructorPatch
	{
		public static void Postfix(InventoryPage __instance, int x, int y, int width, int height)
		{
            try
            {
                ConvenientInventory.Constructor(__instance, x, y, width, height);
            }
            catch (Exception e)
            {
				ModEntry.Context.Monitor.Log($"Failed in {nameof(Postfix)}:\n{e}", LogLevel.Error);
			}
		}
	}

	[HarmonyPatch(typeof(InventoryPage))]
	public class InventoryPagePatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(InventoryPage.receiveLeftClick))]
		public static void ReceiveLeftClick_Postfix(int x, int y)
		{
			try
			{
				ConvenientInventory.ReceiveLeftClick(x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Postfix)}:\n{e}", LogLevel.Error);
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(InventoryPage.draw))]
		public static void Draw_Postfix(SpriteBatch b)
		{
			try
            {
                ConvenientInventory.PostDraw(b);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(InventoryPage.performHoverAction))]
		public static void PerformHoverAction_Postfix(int x, int y)
		{
			try
            {
                ConvenientInventory.PerformHoverAction(x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(PerformHoverAction_Postfix)}:\n{e}", LogLevel.Error);
			}
		}
	}

	[HarmonyPatch(typeof(IClickableMenu))]
	public class IClickableMenuPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(IClickableMenu.populateClickableComponentList))]
		public static void PopulateClickableComponentsList_Postfix(IClickableMenu __instance)
		{
			if (__instance is InventoryPage inventoryPage)
			{
				try
				{
					ConvenientInventory.PopulateClickableComponentsList(inventoryPage);
				}
				catch (Exception e)
				{
					ModEntry.Context.Monitor.Log($"Failed in {nameof(PopulateClickableComponentsList_Postfix)}:\n{e}", LogLevel.Error);
				}
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(IClickableMenu.update))]
		public static void Update_Postfix(IClickableMenu __instance, GameTime time)
		{
			if (__instance is InventoryPage inventoryPage)
			{
				try
				{
					ConvenientInventory.Update(time);
				}
				catch (Exception e)
				{
					ModEntry.Context.Monitor.Log($"Failed in {nameof(Update_Postfix)}:\n{e}", LogLevel.Error);
				}
			}
		}
	}

	[HarmonyPatch(typeof(ClickableTextureComponent))]
	public class ClickableTextureComponentPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(ClickableTextureComponent.draw))]
		[HarmonyPatch(new Type[] { typeof(SpriteBatch) })]
		public static void Draw_Postfix(ClickableTextureComponent __instance, SpriteBatch b)
		{
			try
			{
				ConvenientInventory.TrashCanDrawn(__instance, b);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
			}
		}
	}
}
