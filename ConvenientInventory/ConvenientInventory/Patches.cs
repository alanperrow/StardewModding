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
		[HarmonyPatch(nameof(InventoryPage.draw))]
		public static void Draw_Postfix(SpriteBatch b)
		{
			try
            {
                ConvenientInventory.PostInventoryPageDraw(b);
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


		[HarmonyPostfix]
		[HarmonyPatch(nameof(InventoryPage.receiveLeftClick))]
		public static void ReceiveLeftClick_Postfix(InventoryPage __instance, int x, int y)
		{
			try
			{
				ConvenientInventory.ReceiveLeftClick(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Postfix)}:\n{e}", LogLevel.Error);
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
			try
			{
				if (__instance is InventoryPage inventoryPage)
				{
					ConvenientInventory.PopulateClickableComponentsList(inventoryPage);
				}
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(PopulateClickableComponentsList_Postfix)}:\n{e}", LogLevel.Error);
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(IClickableMenu.update))]
		public static void Update_Postfix(IClickableMenu __instance, GameTime time)
		{
			try
			{
				if (__instance is InventoryPage inventoryPage)
				{
					ConvenientInventory.Update(time);
				}
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(Update_Postfix)}:\n{e}", LogLevel.Error);
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
				ConvenientInventory.PostClickableTextureComponentDraw(__instance, b);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
			}
		}
	}

	[HarmonyPatch(typeof(MenuWithInventory))]
	public class MenuWithInventoryPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(MenuWithInventory.draw))]
		[HarmonyPatch(new Type[] { typeof(SpriteBatch), typeof(bool), typeof(bool), typeof(int), typeof(int), typeof(int) })]
		public static void Draw_Postfix(MenuWithInventory __instance, SpriteBatch b)
		{
			try
			{
				ConvenientInventory.PostInventoryDraw(__instance, b);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
			}
		}


		[HarmonyPostfix]
		[HarmonyPatch(nameof(MenuWithInventory.receiveLeftClick))]
		public static void ReceiveLeftClick_Postfix(MenuWithInventory __instance, int x, int y)
		{
			try
			{
				ConvenientInventory.ReceiveLeftClick(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Postfix)}:\n{e}", LogLevel.Error);
			}
		}
	}

	[HarmonyPatch(typeof(InventoryMenu))]
	public class InventoryMenuPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(InventoryMenu.draw))]
		[HarmonyPatch(new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(int) })]
		public static void Draw_Postfix(InventoryMenu __instance, SpriteBatch b)
		{
			try
			{
				if (__instance.playerInventory)
                {
					ConvenientInventory.PostInventoryDraw(__instance, b);
				}
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
			}
		}
	}
}
