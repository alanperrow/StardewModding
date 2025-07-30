using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;

namespace ConvenientInventory.QuickStack
{
    /// <summary>
    /// Handles all logic related to enabling, disabling, and prioritizing quick stack per-chest.
    /// </summary>
    public static class ChestQuickStackToggleLogic
    {        
        private const int ToggleChestQuickStackButtonID = 918022;  // Unique indentifier

        private static readonly PerScreen<ClickableTextureComponent> chestQuickStackToggleButton = new();

        private static ClickableTextureComponent ChestQuickStackToggleButton
        {
            get => chestQuickStackToggleButton.Value;
            set => chestQuickStackToggleButton.Value = value;
        }

        public static void OnOpenedItemGrabMenu(ItemGrabMenu itemGrabMenu)
        {
            if (!ModEntry.Config.QuickStack.IsToggleChestEnabled)
            {
                return;
            }

            // TODO: Check `itemGrabMenu.context` to determine whether this is a quick-stackable chest.
            //       We only want the Toggle Quick Stack button appearing on chests that can be quick stacked into.

            if (itemGrabMenu.fillStacksButton != null)
            {
                ChestQuickStackToggleButton = CreateButton(itemGrabMenu);
            }
        }

        public static void OnClosedItemGrabMenu()
        {
            ChestQuickStackToggleButton = null;
        }

        private static ClickableTextureComponent CreateButton(ItemGrabMenu itemGrabMenu)
        {
            const int buttonSize = 64;
            Rectangle buttonBounds = new(
                itemGrabMenu.fillStacksButton.bounds.Left + buttonSize + 16, // place to the right of Fill Stacks button
                itemGrabMenu.fillStacksButton.bounds.Top,
                buttonSize, buttonSize);

            ClickableTextureComponent button = new(
                name: string.Empty,
                bounds: buttonBounds,
                label: string.Empty,
                hoverText: GetHoverText(itemGrabMenu),
                texture: GetButtonTexture(itemGrabMenu),
                sourceRect: Rectangle.Empty,
                scale: 4f)
            {
                myID = ToggleChestQuickStackButtonID,
            };

            SetButtonNeighborIds(button, itemGrabMenu);
            return button;
        }

        private static string GetHoverText(ItemGrabMenu itemGrabMenu)
        {
            // TODO: Should this even be dynamic?
            //       It might be good enough with the button icon alone; they do a pretty good job showing the current toggle state.
            //       This way the hover text could just be "Toggle Quick Stack".


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

        private static void SetButtonNeighborIds(ClickableTextureComponent button, ItemGrabMenu itemGrabMenu)
        {
            // Base game logic for Fill Stacks button.
            button.upNeighborID = itemGrabMenu.colorPickerToggleButton != null
                ? ItemGrabMenu.region_colorPickToggle
                : (itemGrabMenu.specialButton != null ? ItemGrabMenu.region_specialButton : ClickableComponent.ID_ignore);
            button.downNeighborID = ItemGrabMenu.region_organizeButton;
            button.leftNeighborID = ItemGrabMenu.region_itemsToGrabMenuModifier + 11; // = 53921

            // Define our button as the right neighbor of the Fill Stacks button.
            button.rightNeighborID = itemGrabMenu.fillStacksButton.rightNeighborID;
            itemGrabMenu.fillStacksButton.rightNeighborID = button.myID;


            // TODO: Need to Postfix `ItemGrabMenu.SetupBorderNeighbors` to call this method.
            //       Reasoning is that all button neighbors get re-setup when color picker is toggled.
        }
    }
}
