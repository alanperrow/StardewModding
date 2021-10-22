using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using HarmonyLib;
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Reflection;

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
		public static void Draw_Postfix(InventoryPage __instance, SpriteBatch b)
		{
			try
            {
                ConvenientInventory.PostMenuDraw(__instance, b);
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
                ConvenientInventory.PerformHoverActionInInventoryPage(x, y);
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
				ConvenientInventory.ReceiveLeftClickInMenu(__instance, x, y);
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
		public static void Draw_Postfix(CraftingPage __instance, SpriteBatch b)
		{
			try
			{
				ConvenientInventory.PostMenuDraw(__instance, b);
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
				ConvenientInventory.ReceiveLeftClickInMenu(__instance, x, y);
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
					ConvenientInventory.PopulateClickableComponentsListInInventoryPage(inventoryPage);
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
				ConvenientInventory.PostMenuDraw(__instance, b);
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
				ConvenientInventory.ReceiveLeftClickInMenu(__instance, x, y);
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
		static FieldInfo fieldInfo = AccessTools.Field(typeof(InventoryMenu), "");

		[HarmonyTranspiler]
		[HarmonyPatch(nameof(InventoryMenu.draw))]
		[HarmonyPatch(new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(int) })]
		public static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			try
			{
				// Add my conditional method call between these two opcodes in InventoryMenu.draw(s, i, i, i):
				// IL_02c3: blt IL_00e5		// END for j (SDV l) loop
				// >>> MY METHOD CALL HERE
				// IL_02c8: ldc.i4.0		// BEGIN for k loop

				/*
				=== Desired C# code ===
				if (ConvenientInventory.IsPlayerInventory(__instance))
				{
					ConvenientInventory.DrawFavoriteItemSlotHighlights(spriteBatch, __instance);
				}

				=== Psuedocode ===
				ldarg.0																						// load this object instance (arg 0)
				call ConvenientInventory::IsPlayerInventory()												// call IsPlayerInventory(this)
				brfalse IL_02c8																				// break if call => false

				ldarg.1																						// load SpriteBatch "b" (arg 1)
				ldarg.0																						// load this object instance (arg 0)
				call ConvenientInventory::DrawFavoriteItemSlotHighlights()									// call DrawFavoriteItemSlotHighlights(b, this)

				*/
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(Draw_Transpiler)}:\n{e}", LogLevel.Error);
			}

			return instructions;
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(InventoryMenu.draw))]
		[HarmonyPatch(new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(int) })]
		public static void Draw_Postfix(InventoryMenu __instance, SpriteBatch b)
		{
			try
			{
				if (ConvenientInventory.IsPlayerInventory(__instance))
                {
					ConvenientInventory.PostMenuDraw(__instance, b);
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
