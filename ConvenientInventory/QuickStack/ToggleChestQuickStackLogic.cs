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
            MenuButtons menuButtons = GetMenuButtons(itemGrabMenu);
            ToggleQuickStackButtonPosition buttonPosition = FindBestPositionForToggleQuickStackButton(menuButtons);

            ClickableTextureComponent toggleQuickStackButton = new(
                name: string.Empty,
                bounds: GetBoundsByButtonPosition(buttonPosition, itemGrabMenu),
                label: string.Empty,
                hoverText: GetButtonHoverText(itemGrabMenu),
                texture: GetButtonTexture(itemGrabMenu),
                sourceRect: Rectangle.Empty,
                scale: 4f)
            {
                myID = ToggleChestQuickStackButtonID,
            };

            SetNeighborIdsForToggleQuickStackButton(toggleQuickStackButton, itemGrabMenu, menuButtons, buttonPosition);
            return toggleQuickStackButton;
        }

        private static MenuButtons GetMenuButtons(ItemGrabMenu itemGrabMenu)
        {
            MenuButtons buttons = MenuButtons.None;
            buttons |= (itemGrabMenu.junimoNoteIcon != null) ? MenuButtons.JunimoNote : MenuButtons.None;
            buttons |= (itemGrabMenu.colorPickerToggleButton != null) ? MenuButtons.ColorPickerToggle : MenuButtons.None;
            buttons |= (itemGrabMenu.specialButton != null) ? MenuButtons.Special : MenuButtons.None;
            buttons |= (itemGrabMenu.fillStacksButton != null) ? MenuButtons.FillStacks : MenuButtons.None;
            buttons |= (itemGrabMenu.organizeButton != null) ? MenuButtons.Organize : MenuButtons.None;

            return buttons;
        }

        /// <summary>
        /// Finds the best valid Toggle Quick Stack button position (i.e. no button already there) for the provided <paramref name="itemGrabMenu"/>.
        /// </summary>
        private static ToggleQuickStackButtonPosition FindBestPositionForToggleQuickStackButton(MenuButtons menuButtons) => menuButtons switch
        {
            // Ideal position is where Junimo Note icon would otherwise be; if not valid, flow downwards, defaulting to the right of Fill Stacks.
            _ when !menuButtons.HasFlag(MenuButtons.JunimoNote) => ToggleQuickStackButtonPosition.JunimoNote,
            _ when !menuButtons.HasFlag(MenuButtons.ColorPickerToggle) && !menuButtons.HasFlag(MenuButtons.Special) => ToggleQuickStackButtonPosition.ColorPickerOrSpecial,
            _ when !menuButtons.HasFlag(MenuButtons.FillStacks) => ToggleQuickStackButtonPosition.FillStacks,
            _ => ToggleQuickStackButtonPosition.RightOfFillStacks,
        };

        private static Rectangle GetBoundsByButtonPosition(ToggleQuickStackButtonPosition buttonPosition, ItemGrabMenu itemGrabMenu)
        {
            // Ideal position for button is where Junimo Note icon would otherwise be.
            int buttonX = itemGrabMenu.xPositionOnScreen + itemGrabMenu.width;
            int buttonY = itemGrabMenu.yPositionOnScreen + itemGrabMenu.height / 3 - 64 - 216;
            const int buttonWidth = 64;
            const int buttonHeight = 64;

            switch (buttonPosition)
            {
                case ToggleQuickStackButtonPosition.JunimoNote:
                    break;
                case ToggleQuickStackButtonPosition.ColorPickerOrSpecial:
                    // Use y-position of the color picker toggle/special button.
                    buttonY = itemGrabMenu.yPositionOnScreen + itemGrabMenu.height / 3 - 64 - 160;
                    break;
                case ToggleQuickStackButtonPosition.FillStacks:
                    // Use y-position of the fill stacks button.
                    buttonY = itemGrabMenu.yPositionOnScreen + itemGrabMenu.height / 3 - 64 - 64 - 16;
                    break;
                case ToggleQuickStackButtonPosition.RightOfFillStacks:
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

        private static void SetNeighborIdsForToggleQuickStackButton(
            ClickableTextureComponent toggleQuickStackButton,
            ItemGrabMenu itemGrabMenu,
            MenuButtons menuButtons,
            ToggleQuickStackButtonPosition buttonPosition)
        {
            //downNeighborID = InventoryPage.region_trashCan,
            //upNeighborID = InventoryPage.region_organizeButton,
            //leftNeighborID = 11, // top-right inventory slot

            // TODO: Need to Postfix `ItemGrabMenu.SetupBorderNeighbors` to accommodate:
            //          1) if color picker is toggled:
            //              - if we are in Junimo Note's spot, this will require our button's top neighbor to be updated
            //          2) if no empty button space available and we had to place our button to the right of fill stacks, we 

            // ACTUALLY
            // Would it simpler if we just always placed the button to the right of the fill stacks button?
            // All supported chests for quick stack have a fill stacks button, so there wouldn't be any empty spaces.
            // Looks a little uglier than filling in the empty Junimo Note space, but it also is better UX to have the button alwys be in the same position.
            // I'm leaning toward this idea...

            switch (buttonPosition)
            {
                case ToggleQuickStackButtonPosition.JunimoNote:

            }
        }

        [Flags]
        private enum MenuButtons
        {
            None = 0,
            JunimoNote = 1 << 0,
            ColorPickerToggle = 1 << 1,
            Special = 1 << 2,
            FillStacks = 1 << 3,
            Organize = 1 << 4,
        }

        private enum ToggleQuickStackButtonPosition
        {
            RightOfFillStacks,
            FillStacks,
            ColorPickerOrSpecial,
            JunimoNote,
        }
    }
}
