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

        public static void OnOpenedItemGrabMenu(ItemGrabMenu itemGrabMenu)
        {
            if (!ModEntry.Config.QuickStack.IsToggleChestEnabled
                || !ShouldQuickStackIntoMenuContext(itemGrabMenu)
                || itemGrabMenu.fillStacksButton == null)
            {
                return;
            }

            QuickStackToggleChestButton = CreateButton(itemGrabMenu);
            ActiveItemGrabMenu = itemGrabMenu;
        }

        public static void OnClosedItemGrabMenu()
        {
            QuickStackToggleChestButton = null;
            ActiveItemGrabMenu = null;
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

        public static void OnDrawComponent(ClickableTextureComponent component, SpriteBatch b)
        {
            if (QuickStackToggleChestButton == null || ActiveItemGrabMenu == null || component != ActiveItemGrabMenu.fillStacksButton)
            {
                return;
            }

            // We have just drawn this menu's Fill Stacks button, so now draw our button next.
            QuickStackToggleChestButton.draw(b);
        }

        public static void OnReceiveLeftClickInItemGrabMenu(int x, int y)
        {
            if (QuickStackToggleChestButton == null || ActiveItemGrabMenu == null || !QuickStackToggleChestButton.containsPoint(x, y))
            {
                return;
            }

            QuickStackToggleChestButton.texture = /*GetButtonTexture(ActiveItemGrabMenu);*/
                // DEBUG: Simply toggle between textures for now, until modData logic is implemented.
                (QuickStackToggleChestButton.texture == CachedTextures.ChestQuickStackEnabledButtonIcon)
                    ? CachedTextures.ChestQuickStackDisabledButtonIcon
                    : CachedTextures.ChestQuickStackEnabledButtonIcon;

            Game1.playSound("drumkit6");
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

        private static bool ShouldQuickStackIntoMenuContext(ItemGrabMenu itemGrabMenu) => itemGrabMenu.context switch
        {
            Chest contextChest => QuickStackLogic.ShouldQuickStackInto(contextChest, out _, false),
            JunimoHut => ModEntry.Config.QuickStack.IntoJunimoHuts,
            _ => false,
        };

        private static ClickableTextureComponent CreateButton(ItemGrabMenu itemGrabMenu)
        {
            const int buttonSize = 64;
            Rectangle buttonBounds = new(
                itemGrabMenu.fillStacksButton.bounds.Left + buttonSize + 16, // Place to the right of Fill Stacks button
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
                myID = QuickStackToggleChestButtonID,
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
            string formatted = I18n.QuickStackToggleChestButton_HoverText_Enabled();

            // TODO: Dynamically format hover text based on modData
            //switch (addPriority) { "\n" + $"+{addPriority} Priority" }

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
        }
    }
}
