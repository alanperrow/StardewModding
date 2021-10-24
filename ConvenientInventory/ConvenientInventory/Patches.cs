using Microsoft.Xna.Framework.Graphics;
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
