using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientInventory.QuickStack
{
    /// <summary>
    /// Handles all logic related to enabling, disabling, and prioritizing quick stack per-chest.
    /// </summary>
    public static class QuickStackToggleChestLogic
    {
        private const int QuickStackToggleChestButtonID = 918022;  // Unique indentifier

        private static string QuickStackToggleChestModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/QuickStackToggleChest";

        private static readonly PerScreen<ClickableTextureComponent> quickStackToggleChestButton = new();
        private static ClickableTextureComponent QuickStackToggleChestButton
        {
            get => quickStackToggleChestButton.Value;
            set => quickStackToggleChestButton.Value = value;
        }

        private static readonly PerScreen<ItemGrabMenu> activeItemGrabMenu = new();
        private static ItemGrabMenu ActiveItemGrabMenu
        {
            get => activeItemGrabMenu.Value;
            set => activeItemGrabMenu.Value = value;
        }

        /// <summary>
        /// Iterates through all chests in the provided game location and removes any quick stack toggle chest mod data.
        /// </summary>
        public static bool CleanupQuickStackToggleChestModDataByLocation(GameLocation gameLocation)
        {
            try
            {
                foreach (Chest chest in gameLocation.Objects.Values.OfType<Chest>())
                {
                    bool removed = chest.modData.Remove(QuickStackToggleChestModDataKey);
                    if (removed)
                    {
                        ModEntry.Instance.Monitor.Log(
                            $"Removed quick stack toggle chest mod data from chest ('{chest.Name}') at location {gameLocation.Name} {chest.TileLocation}.",
                            LogLevel.Trace);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the current <see cref="QuickStackToggleChestState"/> value from the provided chest's mod data.
        /// </summary>
        /// <param name="menuContextChest"></param>
        /// <returns>The obtained <see cref="QuickStackToggleChestState"/> value, or the default enum value if not found.</returns>
        public static QuickStackToggleChestState GetQuickStackToggleChestStateFromModData(IHaveModData chest)
        {
            if (chest == null || !chest.modData.ContainsKey(QuickStackToggleChestModDataKey))
            {
                // Return default state.
                return QuickStackToggleChestState.Enabled;
            }

            string modDataValue = chest.modData[QuickStackToggleChestModDataKey];
            if (!Enum.TryParse(modDataValue, out QuickStackToggleChestState parsedState))
            {
                // Return default state.
                return QuickStackToggleChestState.Enabled;
            }

            return parsedState;
        }

        public static void OnConstructedItemGrabMenu(ItemGrabMenu itemGrabMenu)
        {
            if (!ModEntry.Config.QuickStack.IsEnabled
                || !ModEntry.Config.QuickStack.IsToggleChestEnabled
                || ModEntry.Config.QuickStack.IsToggleChestButtonHidden
                || !ShouldQuickStackIntoMenuContext(itemGrabMenu)
                || itemGrabMenu.fillStacksButton == null)
            {
                return;
            }

            QuickStackToggleChestButton = CreateButton(itemGrabMenu);
            ActiveItemGrabMenu = itemGrabMenu;
        }

        public static void OnExitedItemGrabMenu(ItemGrabMenu itemGrabMenu)
        {
            if (ActiveItemGrabMenu != itemGrabMenu)
            {
                return;
            }

            QuickStackToggleChestButton = null;
            ActiveItemGrabMenu = null;
        }

        public static void OnWindowResized()
        {
            if (QuickStackToggleChestButton == null || ActiveItemGrabMenu == null)
            {
                return;
            }

            // Reposition our button now that the window size has changed.
            QuickStackToggleChestButton.bounds = GetButtonBounds(ActiveItemGrabMenu);
        }

        public static void OnSetupBorderNeighborsInItemGrabMenu()
        {
            if (QuickStackToggleChestButton == null || ActiveItemGrabMenu == null)
            {
                return;
            }

            // All ItemGrabMenu button neighbors get re-setup when color picker is toggled, so we need to re-setup our button neighbors too.
            SetButtonNeighborIds(QuickStackToggleChestButton, ActiveItemGrabMenu);
        }

        public static void OnPopulateClickableComponentsListInItemGrabMenu()
        {
            if (QuickStackToggleChestButton == null || ActiveItemGrabMenu == null)
            {
                return;
            }

            // Add our button to the new clickable components list (and set up its neighbors).
            SetButtonNeighborIds(QuickStackToggleChestButton, ActiveItemGrabMenu);
        }

        public static void OnDrawComponent(ClickableTextureComponent component, SpriteBatch b)
        {
            if (QuickStackToggleChestButton == null || ActiveItemGrabMenu == null || component != ActiveItemGrabMenu.fillStacksButton)
            {
                return;
            }

            // We have just drawn this menu's Fill Stacks button, so now draw our button.
            QuickStackToggleChestButton.draw(b);
        }

        public static void OnPerformHoverActionInItemGrabMenu(int x, int y)
        {
            if (QuickStackToggleChestButton == null)
            {
                return;
            }

            QuickStackToggleChestButton.tryHover(x, y, 0.25f);
            if (QuickStackToggleChestButton.containsPoint(x, y))
            {
                ActiveItemGrabMenu.hoverText = QuickStackToggleChestButton.hoverText;
            }
        }

        public static void OnReceiveLeftClickInItemGrabMenu(int x, int y)
        {
            if (QuickStackToggleChestButton == null || ActiveItemGrabMenu == null || !QuickStackToggleChestButton.containsPoint(x, y))
            {
                return;
            }

            IHaveModData menuContextChest = GetMenuContextChest(ActiveItemGrabMenu);
            QuickStackToggleChestState updatedState = UpdateQuickStackToggleChestStateInModData(menuContextChest);

            QuickStackToggleChestButton.hoverText = GetHoverText(menuContextChest, updatedState);
            QuickStackToggleChestButton.texture = GetButtonTexture(menuContextChest, updatedState);

            Game1.playSound("drumkit6");
        }

        public static void OnReceiveRightClickInItemGrabMenu(int x, int y)
        {
            if (QuickStackToggleChestButton == null || ActiveItemGrabMenu == null || !QuickStackToggleChestButton.containsPoint(x, y))
            {
                return;
            }

            IHaveModData menuContextChest = GetMenuContextChest(ActiveItemGrabMenu);
            QuickStackToggleChestState updatedState = UpdateQuickStackToggleChestStateInModData(menuContextChest, false);

            QuickStackToggleChestButton.hoverText = GetHoverText(menuContextChest, updatedState);
            QuickStackToggleChestButton.texture = GetButtonTexture(menuContextChest, updatedState);

            Game1.playSound("drumkit6");
        }

        private static bool ShouldQuickStackIntoMenuContext(ItemGrabMenu itemGrabMenu) => itemGrabMenu.context switch
        {
            Chest contextChest => QuickStackLogic.ShouldQuickStackInto(contextChest, out _, false),
            JunimoHut => ModEntry.Config.QuickStack.IntoJunimoHuts,
            _ => false,
        };

        private static ClickableTextureComponent CreateButton(ItemGrabMenu itemGrabMenu)
        {
            Rectangle buttonBounds = GetButtonBounds(itemGrabMenu);
            (string buttonHoverText, Texture2D buttonTexture) = GetDynamicButtonData(itemGrabMenu);

            ClickableTextureComponent button = new(
                name: string.Empty,
                bounds: buttonBounds,
                label: string.Empty,
                hoverText: buttonHoverText,
                texture: buttonTexture,
                sourceRect: Rectangle.Empty,
                scale: 4f)
            {
                myID = QuickStackToggleChestButtonID,
            };

            SetButtonNeighborIds(button, itemGrabMenu);
            return button;
        }

        private static Rectangle GetButtonBounds(ItemGrabMenu itemGrabMenu)
        {
            // Place to the right of Fill Stacks button.
            const int buttonSize = 64;
            Rectangle buttonBounds = new(
                itemGrabMenu.fillStacksButton.bounds.Left + buttonSize + 16,
                itemGrabMenu.fillStacksButton.bounds.Top,
                buttonSize, buttonSize);

            return buttonBounds;
        }

        private static (string, Texture2D) GetDynamicButtonData(ItemGrabMenu itemGrabMenu)
        {
            IHaveModData menuContextChest = GetMenuContextChest(itemGrabMenu);
            QuickStackToggleChestState state = GetQuickStackToggleChestStateFromModData(menuContextChest);
            return (GetHoverText(menuContextChest, state), GetButtonTexture(menuContextChest, state));
        }

        private static string GetHoverText(IHaveModData menuContextChest, QuickStackToggleChestState state)
        {
            if (menuContextChest == null)
            {
                // Edge case: This menu's context does not support mod data. This shouldn't happen, but return default text just to be safe.
                return I18n.QuickStackToggleChestButton_HoverText_Enabled();
            }

            return state switch
            {
                QuickStackToggleChestState.Disabled => I18n.QuickStackToggleChestButton_HoverText_Disabled(),
                QuickStackToggleChestState.Enabled => I18n.QuickStackToggleChestButton_HoverText_Enabled(),
                QuickStackToggleChestState.Priority1 => $"{I18n.QuickStackToggleChestButton_HoverText_Enabled()}\n{I18n.QuickStackToggleChestButton_HoverText_Priority1()}",
                QuickStackToggleChestState.Priority2 => $"{I18n.QuickStackToggleChestButton_HoverText_Enabled()}\n{I18n.QuickStackToggleChestButton_HoverText_Priority2()}",
                QuickStackToggleChestState.Priority3 => $"{I18n.QuickStackToggleChestButton_HoverText_Enabled()}\n{I18n.QuickStackToggleChestButton_HoverText_Priority3()}",
                _ => I18n.QuickStackToggleChestButton_HoverText_Enabled(),
            };
        }

        private static Texture2D GetButtonTexture(IHaveModData menuContextChest = null, QuickStackToggleChestState? state = null)
        {
            if (menuContextChest == null)
            {
                // Edge case: This menu's context chest does not support mod data. This shouldn't happen, but return default texture just to be safe.
                return CachedTextures.ChestQuickStackEnabledButtonIcon;
            }

            return state switch
            {
                QuickStackToggleChestState.Disabled => CachedTextures.ChestQuickStackDisabledButtonIcon,
                QuickStackToggleChestState.Enabled => CachedTextures.ChestQuickStackEnabledButtonIcon,
                QuickStackToggleChestState.Priority1 => CachedTextures.ChestQuickStackPriority1ButtonIcon,
                QuickStackToggleChestState.Priority2 => CachedTextures.ChestQuickStackPriority2ButtonIcon,
                QuickStackToggleChestState.Priority3 => CachedTextures.ChestQuickStackPriority3ButtonIcon,
                _ => CachedTextures.ChestQuickStackEnabledButtonIcon,
            };
        }

        private static void SetButtonNeighborIds(ClickableTextureComponent button, ItemGrabMenu itemGrabMenu)
        {
            // Base game logic for Fill Stacks button.
            button.upNeighborID = itemGrabMenu.colorPickerToggleButton != null
                ? ItemGrabMenu.region_colorPickToggle
                : (itemGrabMenu.specialButton != null ? ItemGrabMenu.region_specialButton : ClickableComponent.ID_ignore);
            button.downNeighborID = ItemGrabMenu.region_organizeButton;

            // Define our button as the right neighbor of the Fill Stacks button.
            button.leftNeighborID = ItemGrabMenu.region_fillStacksButton;
            itemGrabMenu.fillStacksButton.rightNeighborID = QuickStackToggleChestButtonID;

            // Add our button to the menu.
            if (!itemGrabMenu.allClickableComponents.Contains(button))
            {
                itemGrabMenu.allClickableComponents.Add(button);
            }
        }

        private static IHaveModData GetMenuContextChest(ItemGrabMenu itemGrabMenu) => itemGrabMenu?.context switch
        {
            Chest contextChest => contextChest,
            JunimoHut junimoHut => junimoHut.GetOutputChest(),
            _ => null,
        };

        /// <summary>
        /// Updates the current <see cref="QuickStackToggleChestState"/> value from the provided menu context's mod data to the next value in sequence.
        /// </summary>
        /// <param name="menuContextChest">The chest whose modData will be updated.</param>
        /// <param name="increment">Whether to increment (true) or decrement (false) the toggle chest state.</param>
        /// <returns>The new <see cref="QuickStackToggleChestState"/> value.</returns>
        private static QuickStackToggleChestState UpdateQuickStackToggleChestStateInModData(IHaveModData menuContextChest, bool increment = true)
        {
            if (menuContextChest == null)
            {
                // Edge case: This menu's context does not support mod data. This shouldn't happen, but return default state just to be safe.
                return QuickStackToggleChestState.Enabled;
            }

            if (!menuContextChest.modData.ContainsKey(QuickStackToggleChestModDataKey))
            {
                // Assign next state in sequence, starting from default.
                QuickStackToggleChestState newState = increment
                    ? GetNextStateInSequence(QuickStackToggleChestState.Enabled)
                    : GetPrevStateInSequence(QuickStackToggleChestState.Enabled);

                menuContextChest.modData[QuickStackToggleChestModDataKey] = newState.ToModDataString();
                return newState;
            }
            else
            {
                string modDataValue = menuContextChest.modData[QuickStackToggleChestModDataKey];
                if (!Enum.TryParse(modDataValue, out QuickStackToggleChestState parsedState))
                {
                    // Clear invalid state in modData and return default state.
                    menuContextChest.modData.Remove(QuickStackToggleChestModDataKey);
                    return QuickStackToggleChestState.Enabled;
                }

                // Assign next state in sequence after current state.
                QuickStackToggleChestState newState = increment
                    ? GetNextStateInSequence(parsedState)
                    : GetPrevStateInSequence(parsedState);

                menuContextChest.modData[QuickStackToggleChestModDataKey] = newState.ToModDataString();
                return newState;
            }
        }

        private static QuickStackToggleChestState GetNextStateInSequence(QuickStackToggleChestState current)
        {
            if (ModEntry.Config.QuickStack.IsPrioritizeChestEnabled)
            {
                return current switch
                {
                    QuickStackToggleChestState.Disabled => QuickStackToggleChestState.Enabled,
                    QuickStackToggleChestState.Enabled => QuickStackToggleChestState.Priority1,
                    QuickStackToggleChestState.Priority1 => QuickStackToggleChestState.Priority2,
                    QuickStackToggleChestState.Priority2 => QuickStackToggleChestState.Priority3,
                    QuickStackToggleChestState.Priority3 => QuickStackToggleChestState.Disabled,
                    _ => QuickStackToggleChestState.Enabled,
                };
            }

            return current switch
            {
                QuickStackToggleChestState.Disabled => QuickStackToggleChestState.Enabled,
                QuickStackToggleChestState.Enabled => QuickStackToggleChestState.Disabled,
                _ => QuickStackToggleChestState.Enabled,
            };
        }

        private static QuickStackToggleChestState GetPrevStateInSequence(QuickStackToggleChestState current)
        {
            if (ModEntry.Config.QuickStack.IsPrioritizeChestEnabled)
            {
                return current switch
                {
                    QuickStackToggleChestState.Disabled => QuickStackToggleChestState.Priority3,
                    QuickStackToggleChestState.Enabled => QuickStackToggleChestState.Disabled,
                    QuickStackToggleChestState.Priority1 => QuickStackToggleChestState.Enabled,
                    QuickStackToggleChestState.Priority2 => QuickStackToggleChestState.Priority1,
                    QuickStackToggleChestState.Priority3 => QuickStackToggleChestState.Priority2,
                    _ => QuickStackToggleChestState.Disabled,
                };
            }

            return current switch
            {
                QuickStackToggleChestState.Disabled => QuickStackToggleChestState.Enabled,
                QuickStackToggleChestState.Enabled => QuickStackToggleChestState.Disabled,
                _ => QuickStackToggleChestState.Disabled,
            };
        }

        /// <summary>
        /// Casts the provided <see cref="QuickStackToggleChestState"/> value as int before then converting to string, for use with mod data.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>The converted string value.</returns>
        private static string ToModDataString(this QuickStackToggleChestState state) => ((int)state).ToString();
    }
}
