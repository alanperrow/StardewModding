﻿using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using HarmonyLib;
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;

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

		[HarmonyPrefix]
		[HarmonyPatch(nameof(InventoryPage.receiveLeftClick))]
		public static bool ReceiveLeftClick_Prefix(InventoryPage __instance, int x, int y)
		{
			try
			{
				return ConvenientInventory.PreReceiveLeftClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Prefix)}:\n{e}", LogLevel.Error);
			}

			return true;
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(InventoryPage.receiveLeftClick))]
		public static void ReceiveLeftClick_Postfix(InventoryPage __instance, int x, int y)
		{
			try
			{
				ConvenientInventory.PostReceiveLeftClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Postfix)}:\n{e}", LogLevel.Error);
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(nameof(InventoryPage.receiveRightClick))]
		public static bool ReceiveRightClick_Prefix(InventoryPage __instance, int x, int y)
		{
			try
			{
				return ConvenientInventory.PreReceiveRightClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveRightClick_Prefix)}:\n{e}", LogLevel.Error);
			}

			return true;
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

		[HarmonyPrefix]
		[HarmonyPatch(nameof(CraftingPage.receiveLeftClick))]
		public static bool ReceiveLeftClick_Prefix(CraftingPage __instance, int x, int y)
		{
			try
			{
				return ConvenientInventory.PreReceiveLeftClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Prefix)}:\n{e}", LogLevel.Error);
			}

			return true;
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(CraftingPage.receiveLeftClick))]
		public static void ReceiveLeftClick_Postfix(CraftingPage __instance, int x, int y)
		{
			try
			{
				ConvenientInventory.PostReceiveLeftClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Postfix)}:\n{e}", LogLevel.Error);
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(nameof(CraftingPage.receiveRightClick))]
		public static bool ReceiveRightClick_Prefix(CraftingPage __instance, int x, int y)
		{
			try
			{
				return ConvenientInventory.PreReceiveRightClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveRightClick_Prefix)}:\n{e}", LogLevel.Error);
			}

			return true;
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

		[HarmonyTranspiler]
		[HarmonyPatch(nameof(IClickableMenu.drawHoverText))]
		[HarmonyPatch(new Type[]
        {
            typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int),
            typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int), typeof(int),
            typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>)
        })]
		public static IEnumerable<CodeInstruction> DrawHoverText_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo getYMinusBoldTitleTextHeight = AccessTools.Method(typeof(IClickableMenuPatches), nameof(GetYMinusBoldTitleTextHeight));
			MethodInfo drawFavoriteItemsToolTipBorder = AccessTools.Method(typeof(ConvenientInventory), nameof(ConvenientInventory.DrawFavoriteItemsToolTipBorder));

			List<CodeInstruction> instructionsList = instructions.ToList();

			bool flag = false;

			for (int i = 0; i<instructionsList.Count; i++)
			{
				// Find instruction after if(boldTitleText != null){} block
				// IL_07b3 (instructions[772])
				if (i > 0 && i < instructionsList.Count - 1
					&& instructionsList[i - 1].opcode == OpCodes.Stloc_S && (instructionsList[i - 1].operand as LocalBuilder)?.LocalIndex == 6
					&& instructionsList[i].opcode == OpCodes.Ldarg_S && instructionsList[i].operand is byte b && b == 9
					&& instructionsList[i + 1].opcode == OpCodes.Brfalse)
				{
					yield return new CodeInstruction(OpCodes.Ldarg, 9);                              // load Item "hoveredItem" (arg 9)
					yield return new CodeInstruction(OpCodes.Ldarg_0);                               // load SpriteBatch "b" (arg 1)
					yield return new CodeInstruction(OpCodes.Ldloc_S, 5);                            // load local int "x"
					yield return new CodeInstruction(OpCodes.Ldloc_S, 6);                            //     load local int "y"
					yield return new CodeInstruction(OpCodes.Ldarg, 6);                              //     load string "boldTitleText" (arg 6)
					yield return new CodeInstruction(OpCodes.Call, getYMinusBoldTitleTextHeight);    //     call GetYMinusBoldTitleTextHeight(y, boldTitleText)
					yield return new CodeInstruction(OpCodes.Call, drawFavoriteItemsToolTipBorder);  // call DrawFavoriteItemsToolTipBorder(hoveredItem, b, x, GetYMinus)

					flag = true;
				}

				yield return instructionsList[i];
			}

			if (!flag)
			{
				ModEntry.Context.Monitor.Log(
					$"{nameof(DrawHoverText_Transpiler)} could not find target instruction(s) in {nameof(IClickableMenu.drawHoverText)}, so no changes were made.", LogLevel.Error);
			}

			yield break;
		}

		public static int GetYMinusBoldTitleTextHeight(int y, string boldTitleText) => (boldTitleText is null) ? y : (y - (int)Game1.dialogueFont.MeasureString(boldTitleText).Y);
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

		[HarmonyPrefix]
		[HarmonyPatch(nameof(MenuWithInventory.receiveLeftClick))]
		public static bool ReceiveLeftClick_Prefix(MenuWithInventory __instance, int x, int y)
		{
			try
			{
				return ConvenientInventory.PreReceiveLeftClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Prefix)}:\n{e}", LogLevel.Error);
			}

			return true;
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(MenuWithInventory.receiveLeftClick))]
		public static void ReceiveLeftClick_Postfix(MenuWithInventory __instance, int x, int y)
		{
			try
			{
				ConvenientInventory.PostReceiveLeftClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Postfix)}:\n{e}", LogLevel.Error);
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(nameof(MenuWithInventory.receiveRightClick))]
		public static bool ReceiveRightClick_Prefix(MenuWithInventory __instance, int x, int y)
		{
			try
			{
				return ConvenientInventory.PreReceiveRightClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveRightClick_Prefix)}:\n{e}", LogLevel.Error);
			}

			return true;
		}
	}

	[HarmonyPatch(typeof(InventoryMenu))]
	public class InventoryMenuPatches
	{
		[HarmonyTranspiler]
		[HarmonyPatch(nameof(InventoryMenu.draw))]
		[HarmonyPatch(new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(int) })]
		public static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
		{
			MethodInfo isPlayerInventory = AccessTools.Method(typeof(ConvenientInventory), nameof(ConvenientInventory.IsPlayerInventory));
			MethodInfo drawFavoriteItemSlotHighlights = AccessTools.Method(typeof(ConvenientInventory), nameof(ConvenientInventory.DrawFavoriteItemSlotHighlights));

			MethodInfo isConfigEnableFavoriteItems = AccessTools.Method(typeof(InventoryMenuPatches), nameof(InventoryMenuPatches.IsConfigEnableFavoriteItems));

			List<CodeInstruction> instructionsList = instructions.ToList();

			bool flag = false;

			for (int i = 0; i < instructionsList.Count; i++)
			{
				if (i < instructionsList.Count - 2
					&& instructionsList[i].opcode == OpCodes.Ldc_I4_0
					&& instructionsList[i + 1].opcode == OpCodes.Stloc_S && (instructionsList[i + 1].operand as LocalBuilder)?.LocalIndex == 8
					&& instructionsList[i + 2].opcode == OpCodes.Br)
				{
					Label label = ilg.DefineLabel();

					yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = instructionsList[i].ExtractLabels() };	// load this InventoryMenu instance (arg 0)
					yield return new CodeInstruction(OpCodes.Call, isPlayerInventory);									// call IsPlayerInventory(this)
					yield return new CodeInstruction(OpCodes.Brfalse, label);                                           // break if call => false

					yield return new CodeInstruction(OpCodes.Call, isConfigEnableFavoriteItems);                        // call helper method
					yield return new CodeInstruction(OpCodes.Brfalse, label);                                           // break if call => false

					yield return new CodeInstruction(OpCodes.Ldarg_1);													// load SpriteBatch "b" (arg 1)
					yield return new CodeInstruction(OpCodes.Ldarg_0);													// load this InventoryMenu instance (arg 0)
					yield return new CodeInstruction(OpCodes.Call, drawFavoriteItemSlotHighlights);						// call DrawFavoriteItemSlotHighlights(b, this)

					instructionsList[i].WithLabels(label);

					flag = true;
				}

				yield return instructionsList[i];
			}

			if (!flag)
			{
				ModEntry.Context.Monitor.Log(
					$"{nameof(Draw_Transpiler)} could not find target instruction(s) in {nameof(InventoryMenu.draw)}, so no changes were made.", LogLevel.Error);
			}

			yield break;
		}

		public static bool IsConfigEnableFavoriteItems() => ModEntry.Config.IsEnableFavoriteItems;


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

	[HarmonyPatch(typeof(ShopMenu))]
	public class ShopMenuPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(ShopMenu.draw))]
		public static void Draw_Postfix(ShopMenu __instance, SpriteBatch b)
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

		[HarmonyPrefix]
		[HarmonyPatch(nameof(ShopMenu.receiveLeftClick))]
		public static bool ReceiveLeftClick_Prefix(ShopMenu __instance, int x, int y)
		{
			try
			{
				return ConvenientInventory.PreReceiveLeftClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Prefix)}:\n{e}", LogLevel.Error);
			}

			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch(nameof(ShopMenu.receiveRightClick))]
		public static bool ReceiveRightClick_Prefix(ShopMenu __instance, int x, int y)
		{
			try
			{
				return ConvenientInventory.PreReceiveRightClickInMenu(__instance, x, y);
			}
			catch (Exception e)
			{
				ModEntry.Context.Monitor.Log($"Failed in {nameof(ReceiveRightClick_Prefix)}:\n{e}", LogLevel.Error);
			}

			return true;
		}
	}

	[HarmonyPatch(typeof(Item))]
	public class ItemPatches
    {
		[HarmonyPrefix]
		[HarmonyPatch(nameof(Item.canBeTrashed))]
		public static bool CanBeTrashed_Prefix(ref bool __result)
        {
			if (ModEntry.Config.IsEnableFavoriteItems && ConvenientInventory.FavoriteItemsIsItemSelected)
			{
				if (!(Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu && itemGrabMenu.shippingBin))
				{
					__result = false;
					return false;
				}
			}

			return true;
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
				// Game logic
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
