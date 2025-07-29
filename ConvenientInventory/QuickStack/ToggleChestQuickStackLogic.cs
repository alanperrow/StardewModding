using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using System;

namespace ConvenientInventory.QuickStack
{
    /// <summary>
    /// Handles all logic related to enabling, disabling, and prioritizing quick stack per-chest.
    /// </summary>
    public static class ToggleChestQuickStackLogic
    {
        private const int ToggleChestQuickStackButtonID = 918022;  // Unique indentifier

        private static readonly PerScreen<ClickableTextureComponent> toggleChestQuickStackButton = new();

        private static ClickableTextureComponent ToggleChestQuickStackButton
        {
            get => toggleChestQuickStackButton.Value;
            set => toggleChestQuickStackButton.Value = value;
        }

        public static void OnOpenedItemGrabMenu(ItemGrabMenu itemGrabMenu)
        {
            if (ModEntry.Config.QuickStack.IsToggleChestEnabled)
            {
                ToggleChestQuickStackButton = CreateToggleChestQuickStackButton(itemGrabMenu);
            }
        }

        public static void OnClosedItemGrabMenu()
        {
            ToggleChestQuickStackButton = null;
        }

        private static ClickableTextureComponent CreateToggleChestQuickStackButton(ItemGrabMenu itemGrabMenu)
        {
            ButtonPosition buttonPosition = FindBestButtonPosition(itemGrabMenu);
            Rectangle buttonBounds = GetBoundsByButtonPosition(buttonPosition, itemGrabMenu);

            ClickableTextureComponent button = new(
                name: string.Empty,
                bounds: buttonBounds,
                label: string.Empty,
                hoverText: GetButtonHoverText(itemGrabMenu),
                texture: GetButtonTexture(itemGrabMenu),
                sourceRect: Rectangle.Empty,
                scale: 4f)
            {
                myID = ToggleChestQuickStackButtonID,
            };

            // TODO: update neighbor IDs based on button position

            //downNeighborID = InventoryPage.region_trashCan,
            //upNeighborID = InventoryPage.region_organizeButton,
            //leftNeighborID = 11, // top-right inventory slot

            return button;
        }

        private enum ButtonPosition
        {
            RightOfFillStacks,
            FillStacks,
            ColorPickerOrSpecial,
            JunimoNote,
        }

        /// <summary>
        /// Finds the best valid Toggle Quick Stack button position (i.e. no button already there) for the provided <paramref name="itemGrabMenu"/>.
        /// </summary>
        private static ButtonPosition FindBestButtonPosition(ItemGrabMenu itemGrabMenu) => itemGrabMenu switch
        {
            // Ideal position is where Junimo Note icon would otherwise be; if not valid, flow downwards, defaulting to the right of Fill Stacks.
            _ when itemGrabMenu.junimoNoteIcon is null => ButtonPosition.JunimoNote,
            _ when itemGrabMenu.colorPickerToggleButton is null && itemGrabMenu.specialButton is null => ButtonPosition.ColorPickerOrSpecial,
            _ when itemGrabMenu.fillStacksButton is null => ButtonPosition.FillStacks,
            _ => ButtonPosition.RightOfFillStacks,
        };

        private static Rectangle GetBoundsByButtonPosition(ButtonPosition buttonPosition, ItemGrabMenu itemGrabMenu)
        {
            // Ideal position for button is where Junimo Note icon would otherwise be.
            int buttonX = itemGrabMenu.xPositionOnScreen + itemGrabMenu.width;
            int buttonY = itemGrabMenu.yPositionOnScreen + itemGrabMenu.height / 3 - 64 - 216;
            const int buttonWidth = 64;
            const int buttonHeight = 64;

            switch (buttonPosition)
            {
                case ButtonPosition.JunimoNote:
                    break;
                case ButtonPosition.ColorPickerOrSpecial:
                    // Use y-position of the color picker toggle/special button.
                    buttonY = itemGrabMenu.yPositionOnScreen + itemGrabMenu.height / 3 - 64 - 160;
                    break;
                case ButtonPosition.FillStacks:
                    // Use y-position of the fill stacks button.
                    buttonY = itemGrabMenu.yPositionOnScreen + itemGrabMenu.height / 3 - 64 - 64 - 16;
                    break;
                case ButtonPosition.RightOfFillStacks:
                    // Use y-position of the fill stacks button, and shift to the right.
                    buttonY = itemGrabMenu.yPositionOnScreen + itemGrabMenu.height / 3 - 64 - 64 - 16;
                    buttonX += buttonWidth + 16;
                    break;
            }

            return new Rectangle(buttonY, buttonX, buttonWidth, buttonHeight);
        }

        private static string GetButtonHoverText(ItemGrabMenu itemGrabMenu)
        {
            // TODO: Dynamically format hover text based on modData
            //if (enabled) { "Enabled" } else { "Disabled" }
            string formatted = I18n.ToggleChestQuickStackButton_HoverText_Enabled();

            // TODO: Dynamically format hover text based on modData
            //switch (priority) { "\n" + "+1 Priority" }

            return formatted;
        }

        private static Texture2D GetButtonTexture(ItemGrabMenu itemGrabMenu)
        {
            // TODO: Dynamically get texture based on modData
            return CachedTextures.ChestQuickStackEnabledButtonIcon;
        }
    }
}
