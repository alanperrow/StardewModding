using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using HarmonyLib;
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Diagnostics;
using StardewValley;

namespace ConvenientInventory.Patches
{
	public static class Utilities
    {
		/// <summary>
		/// Iterates through the current StackTrace to try to find a specified type as a method caller.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="maxDepth"></param>
		/// <returns>Whether or not the specified type was found as a method caller within the maximum iteration depth.</returns>
		public static bool IsThisMethodCalledFromType(Type type, int maxDepth = 2)
        {
			var trace = new StackTrace(2);  // Skip first two frames (this method, and the caller of this method)
			var frames = trace?.GetFrames();
			int depth = 0;

			foreach (var frame in frames)
			{
				if (depth > maxDepth * 2)  // maxDepth * 2 because each method call takes up two frames
                {
					return false;
                }

				var method = frame?.GetMethod();

				var isCraftingPage = method?.GetParameters()?[0].ParameterType == type;
				if (isCraftingPage)
				{
					return true;
				}

				depth++;
			}

			return false;
		}
    }

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

	[HarmonyPatch(typeof(CraftingPage))]
	public class CraftingPagePatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(CraftingPage.draw))]
		public static void Draw_Postfix(SpriteBatch b)
		{
			try
			{
				ConvenientInventory.PostCraftingPageDraw(b);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(CraftingPage.receiveLeftClick))]
		public static void ReceiveLeftClick_Postfix(CraftingPage __instance, int x, int y)
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
				// HACK: CraftingPage.inventory has playerInventory = false, so we manually check if this draw call originated from CraftingPage.
				if (__instance.playerInventory || Utilities.IsThisMethodCalledFromType(typeof(CraftingPage)))
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

	[HarmonyPatch(typeof(Farmer))]
	public class FarmerPatches
    {
		[HarmonyPostfix]
		[HarmonyPatch(nameof(Farmer.shiftToolbar))]
		public static void ShiftToolbar_Postfix(Farmer __instance, bool right)
        {
            try
            {
				if (__instance.Items == null || __instance.Items.Count < 12 || __instance.UsingTool || Game1.dialogueUp || (!Game1.pickingTool && !Game1.player.CanMove) || __instance.areAllItemsNull() || Game1.eventUp || Game1.farmEvent != null)
				{
					return;
				}

				ConvenientInventory.ShiftToolbar(right);
			}
			catch (Exception e)
            {
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ShiftToolbar_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
