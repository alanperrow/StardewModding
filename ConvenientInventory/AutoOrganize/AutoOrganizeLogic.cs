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
        public static Texture2D AutoOrganizeIcon { get; set; }

        private static string AutoOrganizeModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/AutoOrganize";

        /// <summary>
        /// Determines if this chest's mod data contains auto organize data, and if so, applies the auto organize icon texture
        /// to the chest menu's organize button.
        /// </summary>
        public static void TryApplyAutoOrganizeButton(Chest chest, ItemGrabMenu chestMenu)
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
            }
            else
            {
                chest.modData[AutoOrganizeModDataKey] = "1";
                UpdateToAutoOrganizeButton(organizeButton);
            }
        }

        private static void UpdateToAutoOrganizeButton(ClickableTextureComponent organizeButton)
        {
            organizeButton.texture = AutoOrganizeIcon;
            organizeButton.sourceRect = new Rectangle(0, 0, AutoOrganizeIcon.Width, AutoOrganizeIcon.Height);
            organizeButton.label = ModEntry.Instance.Helper.Translation.Get("AutoOrganizeButton.label"); //"Auto Organize ON"
            organizeButton.hoverText = ModEntry.Instance.Helper.Translation.Get("AutoOrganizeButton.hoverText"); //"Right click to disable"
            // TODO: "Right click" should dynamically change to whichever hotkey is correct for the current input (keyboard vs controller).
            //       Alternatively, investigate long-click to toggle auto organize. This would have no confusion.
        }

        private static void ResetToDefaultOrganizeButton(ClickableTextureComponent organizeButton)
        {
            // Default organize button values taken from base game ItemGrabMenu constructor.
            organizeButton.texture = Game1.mouseCursors;
            organizeButton.sourceRect = new Rectangle(162, 440, 16, 16);
            organizeButton.label = "";
            organizeButton.hoverText = Game1.content.LoadString("Strings\\UI:ItemGrab_Organize");
        }
    }
}
