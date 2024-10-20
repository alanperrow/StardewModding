using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Inventories;
using StardewValley.Objects;
using ConvenientInventory.QuickStack;
using ConvenientInventory.AutoOrganize;

namespace ConvenientInventory.Patches
{
    public static class InventoryPageConstructorPatch
    {
        public static void Postfix(InventoryPage __instance, int x, int y, int width, int height)
        {
            try
            {
                ConvenientInventory.InventoryPageConstructor(__instance, x, y, width, height);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryPage))]
    public static class InventoryPagePatches
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(PerformHoverAction_Postfix)}:\n{e}", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Prefix)}:\n{e}", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Postfix)}:\n{e}", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveRightClick_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CraftingPage))]
    public static class CraftingPagePatches
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveRightClick_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(IClickableMenu))]
    public static class IClickableMenuPatches
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(PopulateClickableComponentsList_Postfix)}:\n{e}", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Update_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(IClickableMenu.drawHoverText))]
        [HarmonyPatch(new Type[]
        {
            typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int),
            typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(string), typeof(int), typeof(int), typeof(int),
            typeof(float), typeof(CraftingRecipe), typeof(IList<Item>),
            typeof(Texture2D), typeof(Rectangle?), typeof(Color?), typeof(Color?), typeof(float), typeof(int), typeof(int)
        })]
        public static IEnumerable<CodeInstruction> DrawHoverText_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo getYMinusBoldTitleTextHeight = AccessTools.Method(typeof(IClickableMenuPatches), nameof(GetYMinusBoldTitleTextHeight));
            MethodInfo drawFavoriteItemsToolTipBorder = AccessTools.Method(typeof(ConvenientInventory), nameof(ConvenientInventory.DrawFavoriteItemsToolTipBorder));

            List<CodeInstruction> instructionsList = instructions.ToList();

            bool flag = false;

            for (int i = 0; i < instructionsList.Count; i++)
            {
                // Find instruction after if(boldTitleText != null){ ... b.DrawString() x 3 ... } block
                // IL_084e (instructionsList[?])
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
                ModEntry.Instance.Monitor.Log(
                    $"{nameof(DrawHoverText_Transpiler)} could not find target instruction(s) in {nameof(IClickableMenu.drawHoverText)}, so no changes were made.", LogLevel.Error);
            }

            yield break;
        }

        public static int GetYMinusBoldTitleTextHeight(int y, string boldTitleText) => (boldTitleText is null) ? y : (y - (int)Game1.dialogueFont.MeasureString(boldTitleText).Y);
    }

    [HarmonyPatch(typeof(ClickableTextureComponent))]
    public static class ClickableTextureComponentPatches
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(MenuWithInventory))]
    public static class MenuWithInventoryPatches
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveRightClick_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(InventoryMenu))]
    public static class InventoryMenuPatches
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
                // Find instruction at start of (int k = 0; k < this.capacity; k++){} block
                // IL_02d7 (instructionsList[?])
                if (i < instructionsList.Count - 2
                    && instructionsList[i].opcode == OpCodes.Ldc_I4_0
                    && instructionsList[i + 1].opcode == OpCodes.Stloc_S && (instructionsList[i + 1].operand as LocalBuilder)?.LocalIndex == 10
                    && instructionsList[i + 2].opcode == OpCodes.Br)
                {
                    Label label = ilg.DefineLabel();

                    yield return new CodeInstruction(OpCodes.Ldarg_0) { labels = instructionsList[i].ExtractLabels() }; // load this InventoryMenu instance (arg 0)
                    yield return new CodeInstruction(OpCodes.Call, isPlayerInventory);                                  // call IsPlayerInventory(this)
                    yield return new CodeInstruction(OpCodes.Brfalse, label);                                           // break if call => false

                    yield return new CodeInstruction(OpCodes.Call, isConfigEnableFavoriteItems);                        // call helper method
                    yield return new CodeInstruction(OpCodes.Brfalse, label);                                           // break if call => false

                    yield return new CodeInstruction(OpCodes.Ldarg_1);                                                  // load SpriteBatch "b" (arg 1)
                    yield return new CodeInstruction(OpCodes.Ldarg_0);                                                  // load this InventoryMenu instance (arg 0)
                    yield return new CodeInstruction(OpCodes.Call, drawFavoriteItemSlotHighlights);                     // call DrawFavoriteItemSlotHighlights(b, this)

                    instructionsList[i].WithLabels(label);

                    flag = true;
                }

                yield return instructionsList[i];
            }

            if (!flag)
            {
                ModEntry.Instance.Monitor.Log(
                    $"{nameof(InventoryMenuPatches)}.{nameof(Draw_Transpiler)} could not find target instruction(s) in {nameof(InventoryMenu.draw)}, so no changes were made.", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(InventoryMenu.rightClick))]
        public static IEnumerable<CodeInstruction> RightClick_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            MethodInfo isTakeAllButOneHotkeyDownMethod = AccessTools.DeclaredMethod(typeof(InventoryMenuPatches), nameof(InventoryMenuPatches.IsTakeAllButOneHotkeyDown));
            MethodInfo takeAllButOneItemMethod = AccessTools.DeclaredMethod(typeof(InventoryMenuPatches), nameof(InventoryMenuPatches.TakeAllButOneItem));
            MethodInfo takeAllButOneItemWithAddToMethod = AccessTools.DeclaredMethod(typeof(InventoryMenuPatches), nameof(InventoryMenuPatches.TakeAllButOneItemWithAddTo));

            List<CodeInstruction> instructionsList = instructions.ToList();
            bool flag = false, flag2 = false;
            int patch2MinIndex = int.MaxValue;

            for (int i = 0; i < instructionsList.Count; i++)
            {
                // ==== First case: No item in cursor slot ====
                // Find instruction for `if (this.actualInventory[slotNumber].Stack > 1 && Game1.isOneOfTheseKeysDown( ... { LeftShift })`
                // IL_014d (instructionsList[?])
                if (i > 0 && i < instructionsList.Count - 2
                    && instructionsList[i - 1].opcode == OpCodes.Stloc_S && (instructionsList[i - 1].operand as LocalBuilder)?.LocalIndex == 4
                    && instructionsList[i].opcode == OpCodes.Ldarg_0
                    && instructionsList[i + 1].opcode == OpCodes.Ldfld
                    && instructionsList[i + 2].opcode == OpCodes.Ldloc_1)
                {
                    // Original source code checks for LeftShift in an if-statement. We want to prefix this with our own if-check for "Take All But One" feature.
                    // By tracking the original instruction label, we can convert the original if-statement into an else condition of our own if-statement.
                    Label labelIf = ilg.DefineLabel();

                    // Find instruction for `if (this.actualInventory[slotNumber] != null && this.actualInventory[slotNumber].Stack <= 0)`
                    // IL_0213 (instructionsList[?])
                    Label labelEndElse = ilg.DefineLabel();
                    int j = i + 4;
                    for (; j < instructionsList.Count - 1; j++)
                    {
                        if (instructionsList[j - 2].opcode == OpCodes.Sub
                            && instructionsList[j - 1].opcode == OpCodes.Callvirt
                            && instructionsList[j].opcode == OpCodes.Ldarg_0
                            && instructionsList[j + 1].opcode == OpCodes.Ldfld)
                        {
                            break;
                        }
                    }

                    // Inject "Take All But One" code.
                    yield return new CodeInstruction(OpCodes.Ldarg_0);                                          // load `this` InventoryMenu instance (arg0)
                    yield return new CodeInstruction(OpCodes.Ldloc_1);                                          // load `num` int (local var @ index 1)
                    yield return new CodeInstruction(OpCodes.Call, isTakeAllButOneHotkeyDownMethod)             // call helper method `IsTakeAllButOneHotkeyDown(this, num)`
                    {
                        labels = instructionsList[i].ExtractLabels()
                    };
                    yield return new CodeInstruction(OpCodes.Brfalse, labelIf);                                 // break to original if-statement if call => false

                    yield return new CodeInstruction(OpCodes.Ldarg_0);                                          // load `this` InventoryMenu instance (arg0)
                    yield return new CodeInstruction(OpCodes.Ldloc_1);                                          // load `num` int (local var @ index 1)
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);                                       // load `one` Item instance (local var @ short index 4)
                    yield return new CodeInstruction(OpCodes.Call, takeAllButOneItemMethod);                    // call helper method `TakeAllButOneItem(this, num, one)`
                    yield return new CodeInstruction(OpCodes.Br, labelEndElse);                                 // break to end of else block

                    instructionsList[i].WithLabels(labelIf);
                    instructionsList[j].WithLabels(labelEndElse);

                    flag = true;
                    patch2MinIndex = j + 2;
                }

                // ==== Second case: Has item in cursor slot ====
                // Find instruction in `else` block for `if (Game1.isOneOfTheseKeysDown( ... { LeftShift })`
                // IL_028c (instructionsList[?])
                if (i > patch2MinIndex && i < instructionsList.Count - 2
                    && instructionsList[i - 1].opcode == OpCodes.Bge
                    && instructionsList[i].opcode == OpCodes.Ldsfld // valuetype StardewValley.Game1::oldKBState
                    && instructionsList[i + 1].opcode == OpCodes.Ldc_I4_1
                    && instructionsList[i + 2].opcode == OpCodes.Newarr)
                {
                    // Original source code checks for LeftShift in an if-statement. We want to prefix this with our own if-check for "Take All But One" feature.
                    // By tracking the original instruction label, we can convert the original if-statement into an else condition of our own if-statement.
                    Label labelIf = ilg.DefineLabel();

                    // Find instruction for `if (playSound)`
                    // IL_0343 (instructionsList[?])
                    Label labelEndElse = ilg.DefineLabel();
                    int j = i + 4;
                    for (; j < instructionsList.Count - 1; j++)
                    {
                        if (instructionsList[j - 2].opcode == OpCodes.Sub
                            && instructionsList[j - 1].opcode == OpCodes.Callvirt // instance void StardewValley.Item::set_Stack(int32)
                            && instructionsList[j].opcode == OpCodes.Ldarg_S
                            && instructionsList[j + 1].opcode == OpCodes.Brfalse_S)
                        {
                            break;
                        }
                    }

                    // Inject "Take All But One" code.
                    yield return new CodeInstruction(OpCodes.Ldarg_0);                                          // load `this` InventoryMenu instance (arg0)
                    yield return new CodeInstruction(OpCodes.Ldloc_1);                                          // load `num` int (local var @ index 1)
                    yield return new CodeInstruction(OpCodes.Call, isTakeAllButOneHotkeyDownMethod)             // call helper method `IsTakeAllButOneHotkeyDown(this, num)`
                    {
                        labels = instructionsList[i].ExtractLabels()
                    };
                    yield return new CodeInstruction(OpCodes.Brfalse, labelIf);                                 // break to original if-statement if call => false

                    yield return new CodeInstruction(OpCodes.Ldarg_0);                                          // load `this` InventoryMenu instance (arg0)
                    yield return new CodeInstruction(OpCodes.Ldloc_1);                                          // load `num` int (local var @ index 1)
                    yield return new CodeInstruction(OpCodes.Ldarg_3);                                          // load `toAddTo` Item instance (arg3)
                    yield return new CodeInstruction(OpCodes.Call, takeAllButOneItemWithAddToMethod);           // call helper method `TakeAllButOneItemWithAddTo(this, num, toAddTo)`
                    yield return new CodeInstruction(OpCodes.Br, labelEndElse);                                 // break to end of else block

                    instructionsList[i].WithLabels(labelIf);
                    instructionsList[j].WithLabels(labelEndElse);

                    flag2 = true;
                }

                yield return instructionsList[i];
            }

            if (!flag && !flag2)
            {
                ModEntry.Instance.Monitor.Log(
                    $"{nameof(InventoryMenuPatches)}.{nameof(RightClick_Transpiler)} could not find target instruction(s) in {nameof(InventoryMenu.rightClick)}, so no changes were made.",
                    LogLevel.Error);
            }
            else if (!flag || !flag2)
            {
                string ordSuccess = !flag ? "First" : "Second";
                string ordFail = !flag ? "Second" : "First";
                ModEntry.Instance.Monitor.Log(
                    $"{nameof(InventoryMenuPatches)}.{nameof(RightClick_Transpiler)}: {ordSuccess} patch was applied successfully. "
                    + $"{ordFail} patch could not find target instruction(s) in {nameof(InventoryMenu.rightClick)}, so no changes were made.",
                    LogLevel.Error);
            }

            yield break;
        }

        public static bool IsTakeAllButOneHotkeyDown(InventoryMenu inventoryMenu, int slotNumber)
        {
            bool moreThanOneItem = inventoryMenu.actualInventory[slotNumber].Stack > 1; // Game logic

            return ModEntry.Config.IsEnableTakeAllButOne
                && moreThanOneItem
                && (ModEntry.Config.TakeAllButOneKeyboardHotkey.IsDown() || ModEntry.Config.TakeAllButOneControllerHotkey.IsDown());
        }

        public static void TakeAllButOneItem(InventoryMenu inventoryMenu, int slotNumber, Item newItem)
        {
            newItem.Stack = inventoryMenu.actualInventory[slotNumber].Stack - 1;
            inventoryMenu.actualInventory[slotNumber].Stack = 1;
        }

        public static void TakeAllButOneItemWithAddTo(InventoryMenu inventoryMenu, int slotNumber, Item toAddTo)
        {
            int amountToTake = Math.Min(inventoryMenu.actualInventory[slotNumber].Stack - 1, toAddTo.getRemainingStackSpace());
            inventoryMenu.actualInventory[slotNumber].Stack -= amountToTake;
            toAddTo.Stack += amountToTake;
        }
    }

    [HarmonyPatch(typeof(Toolbar))]
    public static class ToolbarPatches
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Toolbar.draw))]
        [HarmonyPatch(new Type[] { typeof(SpriteBatch) })]
        public static IEnumerable<CodeInstruction> Draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            MethodInfo isConfigEnableFavoriteItems = AccessTools.Method(typeof(ToolbarPatches), nameof(ToolbarPatches.IsConfigEnableFavoriteItems));
            MethodInfo DrawFavoriteItemSlotHighlightsInToolbar = AccessTools.Method(typeof(ConvenientInventory), nameof(ConvenientInventory.DrawFavoriteItemSlotHighlightsInToolbar));

            FieldInfo yPositionOnScreen = typeof(Toolbar).GetField("yPositionOnScreen", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            FieldInfo transparency = typeof(Toolbar).GetField("transparency", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            FieldInfo slotText = typeof(Toolbar).GetField("slotText", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            List<CodeInstruction> instructionsList = instructions.ToList();

            bool flag = false;

            for (int i = 0; i < instructionsList.Count; i++)
            {
                // Find instruction after for(int j = 0; j < 12; j++){} block
                // IL_027c (instructionsList[?])
                if (i > 0 && i < instructionsList.Count - 2
                    && instructionsList[i - 1].opcode == OpCodes.Blt
                    && instructionsList[i].opcode == OpCodes.Ldc_I4_0
                    && instructionsList[i + 1].opcode == OpCodes.Stloc_S && (instructionsList[i + 1].operand as LocalBuilder)?.LocalIndex == 8
                    && instructionsList[i + 2].opcode == OpCodes.Br)
                {
                    Label label = ilg.DefineLabel();

                    yield return new CodeInstruction(OpCodes.Call, isConfigEnableFavoriteItems)                 // call helper method
                    {
                        labels = instructionsList[i].ExtractLabels()
                    };
                    yield return new CodeInstruction(OpCodes.Brfalse, label);                                   // break if call => false

                    yield return new CodeInstruction(OpCodes.Ldarg_1);                                          // load SpriteBatch "b" (arg 1)
                    yield return new CodeInstruction(OpCodes.Ldarg_0);                                          // load this (arg0)
                    yield return new CodeInstruction(OpCodes.Ldfld, yPositionOnScreen);                         // load field "this.yPositionOnScreen"
                    yield return new CodeInstruction(OpCodes.Ldarg_0);                                          // load this (arg0)
                    yield return new CodeInstruction(OpCodes.Ldfld, transparency);                              // load field "this.transparency"
                    yield return new CodeInstruction(OpCodes.Ldarg_0);                                          // load this (arg0)
                    yield return new CodeInstruction(OpCodes.Ldfld, slotText);                                  // load field "this.slotText"
                    yield return new CodeInstruction(OpCodes.Call, DrawFavoriteItemSlotHighlightsInToolbar);    // call DrawFavoriteItemSlotHighlightsInToolbar(b, yPositionOnScreen)

                    instructionsList[i].WithLabels(label);

                    flag = true;
                }

                yield return instructionsList[i];
            }

            if (!flag)
            {
                ModEntry.Instance.Monitor.Log(
                    $"{nameof(ToolbarPatches)}.{nameof(Draw_Transpiler)} could not find target instruction(s) in {nameof(Toolbar.draw)}, so no changes were made.", LogLevel.Error);
            }

            yield break;
        }

        public static bool IsConfigEnableFavoriteItems() => ModEntry.Config.IsEnableFavoriteItems;

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Toolbar.receiveRightClick))]
        public static IEnumerable<CodeInstruction> ReceiveRightClick_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            MethodInfo isItemSlotFavoritedMethod = AccessTools.DeclaredMethod(typeof(ToolbarPatches), nameof(ToolbarPatches.IsItemSlotFavorited));

            List<CodeInstruction> instructionsList = instructions.ToList();
            bool flag = false;
            for (int i = 0; i < instructionsList.Count; i++)
            {
                // Find instruction for `Game1.playSound("throwDownITem");`
                // IL_000aa (instructionsList[?])
                if (i > 0 && i < instructionsList.Count
                    && instructionsList[i].opcode == OpCodes.Ldstr && (instructionsList[i].operand as string).Equals("throwDownITem"))
                {
                    // Find instruction for `leave.s` (end loop), searching backwards for efficiency
                    // IL_0106 (instructionsList[?])
                    Label labelLeave = ilg.DefineLabel();
                    int j = instructionsList.Count - 1;
                    for (; j > i; j--)
                    {
                        if (instructionsList[j].opcode == OpCodes.Leave_S)
                        {
                            break;
                        }
                    }

                    yield return new CodeInstruction(OpCodes.Ldloc_3);                                         // load `slotNumber` int (local var @ index 3)
                    yield return new CodeInstruction(OpCodes.Call, isItemSlotFavoritedMethod);                 // call helper method `IsItemSlotFavorited`
                    yield return new CodeInstruction(OpCodes.Brtrue, labelLeave);                              // break if call => true

                    instructionsList[j].WithLabels(labelLeave);

                    flag = true;
                }

                yield return instructionsList[i];
            }

            if (!flag)
            {
                ModEntry.Instance.Monitor.Log(
                    $"{nameof(ToolbarPatches)}.{nameof(ReceiveRightClick_Transpiler)} could not find target instruction(s) in {nameof(Toolbar.receiveRightClick)}, so no changes were made.",
                    LogLevel.Error);
            }

            yield break;
        }

        public static bool IsItemSlotFavorited(int slotNumber)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return false;
            }

            return ConvenientInventory.FavoriteItemSlots[slotNumber];
        }
    }

    [HarmonyPatch(typeof(ShopMenu))]
    public static class ShopMenuPatches
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{e}", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveLeftClick_Prefix)}:\n{e}", LogLevel.Error);
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveRightClick_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ItemGrabMenu))]
    public static class ItemGrabMenuPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ItemGrabMenu.organizeItemsInList))]
        public static bool OrganizeItemsInList_Prefix(ItemGrabMenu __instance, out Item[] __state, IList<Item> items)
        {
            __state = null;

            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return true;
            }

            try
            {
                // Only call when this is the player's inventory
                if (items == Game1.player.Items)
                {
                    __state = ConvenientInventory.ExtractFavoriteItemsFromList(items);
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(OrganizeItemsInList_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ItemGrabMenu.organizeItemsInList))]
        public static void OrganizeItemsInList_Postfix(ItemGrabMenu __instance, Item[] __state, IList<Item> items)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return;
            }

            try
            {
                // Only call when this is the player's inventory
                if (items == Game1.player.Items)
                {
                    ConvenientInventory.ReinsertExtractedFavoriteItemsIntoList(__state, items);
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(OrganizeItemsInList_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(ItemGrabMenu.FillOutStacks))]
        public static bool FillOutStacks_Prefix(ItemGrabMenu __instance, out Item[] __state)
        {
            __state = null;

            try
            {
                if (ModEntry.Config.IsEnableFavoriteItems)
                {
                    __state = ConvenientInventory.ExtractFavoriteItemsFromList(__instance.inventory.actualInventory);
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(FillOutStacks_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ItemGrabMenu.FillOutStacks))]
        public static void FillOutStacks_Postfix(ItemGrabMenu __instance, Item[] __state)
        {
            try
            {
                if (ModEntry.Config.IsEnableFavoriteItems)
                {
                    ConvenientInventory.ReinsertExtractedFavoriteItemsIntoList(__state, __instance.inventory.actualInventory, false);
                }

                if (ModEntry.Config.IsEnableAutoOrganizeChest)
                {
                    Chest chest = AutoOrganizeLogic.GetChestFromItemGrabMenuContext(__instance.context);
                    if (chest != null)
                    {
                        AutoOrganizeLogic.TryOrganizeChestOnFillOutStacks(__instance, chest);
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(FillOutStacks_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor, new Type[]
        {
            typeof(IList<Item>),                        // inventory
            typeof(bool),                               // reverseGrab
            typeof(bool),                               // showReceivingMenu
            typeof(InventoryMenu.highlightThisItem),    // highlightFunction
            typeof(ItemGrabMenu.behaviorOnItemSelect),  // behaviorOnItemSelectFunction
            typeof(string),                             // message
            typeof(ItemGrabMenu.behaviorOnItemSelect),  // behaviorOnItemGrab
            typeof(bool),                               // snapToBottom
            typeof(bool),                               // canBeExitedWithKey
            typeof(bool),                               // playRightClickSound
            typeof(bool),                               // allowRightClick
            typeof(bool),                               // showOrganizeButton
            typeof(int),                                // source
            typeof(Item),                               // sourceItem
            typeof(int),                                // whichSpecialButton
            typeof(object),                             // context
            typeof(ItemExitBehavior),                   // heldItemExitBehavior
            typeof(bool)                                // allowExitWithHeldItem
        })]
        public static void Constructor18_Postfix(ItemGrabMenu __instance, bool showOrganizeButton, int source, object context)
        {
            try
            {
                if (ModEntry.Config.IsEnableAutoOrganizeChest
                    && showOrganizeButton
                    && source == ItemGrabMenu.source_chest)
                {
                    Chest chest = AutoOrganizeLogic.GetChestFromItemGrabMenuContext(context);
                    if (chest != null)
                    {
                        AutoOrganizeLogic.TrySetupAutoOrganizeButton(__instance, chest);
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Constructor18_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ItemGrabMenu.receiveRightClick))]
        public static void ReceiveRightClick_Postfix(ItemGrabMenu __instance, int x, int y)
        {
            if (!ModEntry.Config.IsEnableAutoOrganizeChest)
            {
                return;
            }

            try
            {
                if (__instance.source == ItemGrabMenu.source_chest
                    && __instance.organizeButton != null
                    && __instance.organizeButton.containsPoint(x, y))
                {
                    Chest chest = AutoOrganizeLogic.GetChestFromItemGrabMenuContext(__instance.context);
                    if (chest != null)
                    {
                        AutoOrganizeLogic.ToggleChestAutoOrganize(__instance, chest);
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveRightClick_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        // ItemGrabMenu does not override `IClickableMenu.setUpForGamePadMode`, so we cannot postfix it directly.
        // Instead, we postfix the base class method and manually perform a pattern match for ItemGrabMenu.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(IClickableMenu), nameof(IClickableMenu.setUpForGamePadMode))]
        public static void SetUpForGamePadMode_Postfix(IClickableMenu __instance)
        {
            if (!ModEntry.Config.IsEnableAutoOrganizeChest)
            {
                return;
            }

            try
            {
                if (__instance is ItemGrabMenu itemGrabMenu
                    && itemGrabMenu.source == ItemGrabMenu.source_chest
                    && itemGrabMenu.organizeButton != null)
                {
                    Chest chest = AutoOrganizeLogic.GetChestFromItemGrabMenuContext(itemGrabMenu.context);
                    if (chest != null)
                    {
                        AutoOrganizeLogic.TryUpdateAutoOrganizeButtonHoverTextByGamePadMode(itemGrabMenu.organizeButton, chest);
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(SetUpForGamePadMode_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(Item))]
    public static class ItemPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Item.canBeTrashed))]
        public static bool CanBeTrashed_Prefix(Item __instance, ref bool __result)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems || !ConvenientInventory.FavoriteItemsIsItemSelected)
            {
                return true;
            }

            try
            {
                if (Game1.activeClickableMenu is ForgeMenu forgeMenu)
                {
                    int mouseX, mouseY;
                    if (__instance == ConvenientInventory.FavoriteItemsSelectedItem
                        && (forgeMenu.trashCan.containsPoint(mouseX = Game1.getMouseX(), mouseY = Game1.getMouseY())
                            || !forgeMenu.isWithinBounds(mouseX, mouseY)))
                    {
                        __result = false;
                        return false;
                    }
                }
                else if (!(Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu && itemGrabMenu.shippingBin))
                {
                    __result = false;
                    return false;
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(CanBeTrashed_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Farmer))]
    public static class FarmerPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Farmer.shiftToolbar))]
        public static void ShiftToolbar_Postfix(Farmer __instance, bool right)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return;
            }

            try
            {
                // Game logic
                if (__instance.Items == null
                    || __instance.Items.Count < 12
                    || __instance.UsingTool
                    || Game1.dialogueUp
                    || !__instance.CanMove
                    || !__instance.Items.HasAny()
                    || Game1.eventUp
                    || Game1.farmEvent != null)
                {
                    return;
                }

                ConvenientInventory.ShiftToolbar(right);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ShiftToolbar_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Farmer.reduceActiveItemByOne))]
        public static bool ReduceActiveItemByOne_Prefix(Farmer __instance)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return true;
            }

            try
            {
                ConvenientInventory.PreFarmerReduceActiveItemByOne(__instance);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReduceActiveItemByOne_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ForgeMenu))]
    public static class ForgeMenuPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ForgeMenu.receiveKeyPress))]
        public static bool ReceiveKeyPress_Prefix(Keys key)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems || !ConvenientInventory.FavoriteItemsIsItemSelected)
            {
                return true;
            }

            try
            {
                if (key == Keys.Delete)
                {
                    // Prevents deletion of selected favorited item in ForgeMenu since "canBeTrashed" condition is modified.
                    return false;
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReceiveKeyPress_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("_leftIngredientSpotClicked")]
        public static bool LeftIngredientSpotClicked_Prefix(ForgeMenu __instance)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return true;
            }

            try
            {
                if (__instance.heldItem != null)
                {
                    ConvenientInventory.ResetFavoriteItemSlotsTracking();
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(LeftIngredientSpotClicked_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("_rightIngredientSpotClicked")]
        public static bool RightIngredientSpotClicked_Prefix(ForgeMenu __instance)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return true;
            }

            try
            {
                if (__instance.heldItem != null)
                {
                    ConvenientInventory.ResetFavoriteItemSlotsTracking();
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(RightIngredientSpotClicked_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(ForgeMenu.CraftItem))]
        public static void CraftItem_Postfix(bool forReal = false)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems || !forReal)
            {
                return;
            }

            try
            {
                // In case this craft used all of the cinder shards in a stack, refresh favorite item slots.
                ConvenientInventory.UnfavoriteEmptyItemSlots();
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(CraftItem_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(Inventory))]
    public static class InventoryPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Inventory.ReduceId))]
        public static void ReduceId_Postfix()
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return;
            }

            try
            {
                // In case we reduced any item stack(s) to 0, refresh favorite item slots.
                ConvenientInventory.UnfavoriteEmptyItemSlots();
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(ReduceId_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(Chest))]
    public static class ChestPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Chest.draw))]
        [HarmonyPatch(new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) })]
        public static bool Draw_Prefix(Chest __instance)
        {
            // Optimization idea: Investigate IModHelper.Multiplayer.SendMessage()
            try
            {
                QuickStackChestAnimation.Animate(__instance);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(Draw_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Chest.grabItemFromInventory))]
        public static void GrabItemFromInventory_Postfix()
        {
            if (!ModEntry.Config.IsEnableAutoOrganizeChest)
            {
                return;
            }

            try
            {
                if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu
                    && itemGrabMenu.source == ItemGrabMenu.source_chest
                    && itemGrabMenu.organizeButton != null)
                {
                    Chest chest = AutoOrganizeLogic.GetChestFromItemGrabMenuContext(itemGrabMenu.context);
                    if (chest != null)
                    {
                        AutoOrganizeLogic.TryOrganizeChestInMenu(itemGrabMenu, chest);
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(GrabItemFromInventory_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Chest.addItem))]
        public static void AddItem_Postfix(Chest __instance)
        {
            if (!ModEntry.Config.IsEnableAutoOrganizeChest)
            {
                return;
            }

            try
            {
                AutoOrganizeLogic.TryOrganizeChest(__instance);
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(AddItem_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(StardewValley.Object))]
    public static class ObjectPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(StardewValley.Object.canBeShipped))]
        public static void CanBeShipped_PostFix(StardewValley.Object __instance, ref bool __result)
        {
            if (!__result)
            {
                // Result is already false, so no need to perform additional logic.
                return;
            }


            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return;
            }

            try
            {
                int index = ConvenientInventory.GetPlayerInventoryIndexOfItem(__instance);
                if (index != -1 && ConvenientInventory.FavoriteItemSlots[index])
                {
                    // Prevent shipping of favorited items.
                    __result = false;
                    return;
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(CanBeShipped_PostFix)}:\n{e}", LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocation))]
    public static class GameLocationPatches
    {
        private static Item lastToolbarSlotItem;

        [HarmonyPrefix]
        [HarmonyPatch("removeQueuedFurniture")]
        public static bool RemoveQueuedFurniture_Prefix()
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return true;
            }

            try
            {
                if (!ConvenientInventory.FavoriteItemSlots[11])
                {
                    // No need to track anything since the last toolbar slot is not favorited.
                    return true;
                }

                // Track the last slot of the inventory toolbar to handle base game behavior where, if all toolbar slots have an item,
                // the removed furniture will be placed into the last toolbar slot and add the previous item into the inventory.
                lastToolbarSlotItem = Game1.player.Items[11];
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(RemoveQueuedFurniture_Prefix)}:\n{e}", LogLevel.Error);
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("removeQueuedFurniture")]
        public static void RemoveQueuedFurniture_Postfix()
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return;
            }

            if (lastToolbarSlotItem == null)
            {
                return;
            }

            try
            {
                if (Game1.player.Items[11] != lastToolbarSlotItem)
                {
                    // Find where the previous item was "pushed" to, and move the favorited item slot accordingly.
                    ConvenientInventory.FavoriteItemSlots[11] = false;
                    int index = ConvenientInventory.GetPlayerInventoryIndexOfItem(lastToolbarSlotItem);
                    if (index != -1)
                    {
                        // We found the "pushed" item, so favorite it.
                        ConvenientInventory.FavoriteItemSlots[index] = true;
                    }
                    else
                    {
                        // The "pushed" item has merged with another stack; favorite the slot of the first inventory item that the "pushed" item can stack with.
                        for (int i = 0; i < Game1.player.Items.Count; i++)
                        {
                            if (lastToolbarSlotItem.canStackWith(Game1.player.Items[i]))
                            {
                                ConvenientInventory.FavoriteItemSlots[i] = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(RemoveQueuedFurniture_Postfix)}:\n{e}", LogLevel.Error);
            }
            finally
            {
                lastToolbarSlotItem = null;
            }
        }
    }

    [HarmonyPatch(typeof(JunimoNoteMenu))]
    public static class JunimoNoteMenuPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(JunimoNoteMenu.HandlePartialDonation))]
        public static void HandlePartialDonation_Postfix(Item item)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return;
            }

            try
            {
                if (item.Stack == 0)
                {
                    // We donated all the items in our held item stack, so cleanup leftover favorite item slots.
                    ConvenientInventory.UnfavoriteEmptyItemSlots();
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(HandlePartialDonation_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }

    [HarmonyPatch(typeof(Bundle))]
    public static class BundlePatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Bundle.tryToDepositThisItem))]
        public static void TryToDepositThisItem_Postfix(Item __result)
        {
            if (!ModEntry.Config.IsEnableFavoriteItems)
            {
                return;
            }

            try
            {
                if (__result == null)
                {
                    // The item was deposited into the bundle and its stack was depleted, so cleanup leftover favorite item slots.
                    ConvenientInventory.UnfavoriteEmptyItemSlots();
                }
            }
            catch (Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(TryToDepositThisItem_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
