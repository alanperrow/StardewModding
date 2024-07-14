using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace ConvenientInventory.AutoOrganize
{
    /// <summary>
    /// Handles all logic related to the auto organize chest feature.
    /// </summary>
    public static class AutoOrganizeLogic
    {
        private static string AutoOrganizeModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/AutoOrganize";

        /// <summary>
        /// Determines if this chest's mod data contains auto organize data, and if so, applies the auto organize icon texture
        /// to the chest menu's organize button.
        /// </summary>
        public static void TryApplyAutoOrganizeButton(ItemGrabMenu chestMenu, Chest chest)
        {
            if (chest.modData.ContainsKey(AutoOrganizeModDataKey))
            {
                UpdateToAutoOrganizeButton(chestMenu.organizeButton);
            }
        }

        public static void ToggleChestAutoOrganize(ClickableTextureComponent organizeButton, Chest chest)
        {
            if (chest.modData.ContainsKey(AutoOrganizeModDataKey))
            {
                chest.modData.Remove(AutoOrganizeModDataKey);
                ResetToDefaultOrganizeButton(organizeButton);
                Game1.playSound("dialogueCharacter");
            }
            else
            {
                chest.modData[AutoOrganizeModDataKey] = "1";
                UpdateToAutoOrganizeButton(organizeButton);
                Game1.playSound("smallSelect");

                // TODO: Organize upon toggling on auto organize.
            }

            Game1.playSound("openBox");
        }

        private static void UpdateToAutoOrganizeButton(ClickableTextureComponent organizeButton)
        {
            organizeButton.texture = CachedTextures.AutoOrganizeButtonIcon;
            organizeButton.sourceRect = CachedTextures.AutoOrganizeButtonIcon.Bounds;
            organizeButton.hoverText = "Auto Organize ON\n(Right click to disable)"; // ModEntry.Instance.Helper.Translation.Get("AutoOrganizeButton.hoverText");
            // TODO: "Right click" should dynamically change to whichever hotkey is correct for the current input (keyboard vs controller).
            //       Alternatively, investigate long-click to toggle auto organize. This would have no confusion.
        }

        private static void ResetToDefaultOrganizeButton(ClickableTextureComponent organizeButton)
        {
            // Default organize button values taken from base game ItemGrabMenu constructor.
            organizeButton.texture = Game1.mouseCursors;
            organizeButton.sourceRect = new Rectangle(162, 440, 16, 16);
            organizeButton.hoverText = Game1.content.LoadString("Strings\\UI:ItemGrab_Organize");
        }
    }
}
