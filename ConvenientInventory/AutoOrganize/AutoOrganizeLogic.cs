using Microsoft.Xna.Framework;
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
        public static void TrySetupAutoOrganizeButton(ItemGrabMenu chestMenu, Chest chest)
        {
            if (chest.modData.ContainsKey(AutoOrganizeModDataKey))
            {
                UpdateToAutoOrganizeButton(chestMenu.organizeButton);
            }
        }

        /// <summary>
        /// Toggles the auto organize state of the chest in its mod data, and updates the chest menu's organize button approppriately.
        /// </summary>
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

                // TODO: Organize chest upon toggling on auto organize.
            }

            Game1.playSound("openBox");
        }

        /// <summary>
        /// Determines if this chest's mod data contains auto organize data, and if so, updates the chest menu's auto organize button's hover text
        /// to reference the appropriate hotkey based on the current gamepad mode.
        /// </summary>
        public static void TryUpdateAutoOrganizeButtonHoverTextByGamePadMode(ClickableTextureComponent organizeButton, Chest chest)
        {
            if (chest.modData.ContainsKey(AutoOrganizeModDataKey))
            {
                UpdateHoverTextByGamePadMode(organizeButton);
            }
        }

        private static void UpdateToAutoOrganizeButton(ClickableTextureComponent organizeButton)
        {
            organizeButton.texture = CachedTextures.AutoOrganizeButtonIcon;
            organizeButton.sourceRect = CachedTextures.AutoOrganizeButtonIcon.Bounds;
            UpdateHoverTextByGamePadMode(organizeButton);
        }

        private static void ResetToDefaultOrganizeButton(ClickableTextureComponent organizeButton)
        {
            // Default organize button values taken from base game ItemGrabMenu constructor.
            organizeButton.texture = Game1.mouseCursors;
            organizeButton.sourceRect = new Rectangle(162, 440, 16, 16);
            organizeButton.hoverText = Game1.content.LoadString("Strings\\UI:ItemGrab_Organize");
        }

        private static void UpdateHoverTextByGamePadMode(ClickableTextureComponent organizeButton)
        {
            // TODO: (?) Config option: Show disable instructions in hover text.
            //       If this bool was false, hover text would simply be "Auto Organize ON".
            //          - Would require splitting the translation file into three strings: hoverText, hoverText-disable, hoverText-disable-gamepad

            // TODO: Slight bug: This is working when switching from kbm to gamepad, but is not getting triggered from gamepad to kbm.
            //       Not a big deal as the hover text resets when re-activating auto organize. Probably fine to leave as is.

            organizeButton.hoverText = Game1.options.gamepadControls
                ? "Auto Organize ON\n(X to disable)" // ModEntry.Instance.Helper.Translation.Get("AutoOrganizeButton.hoverText-gamepad");
                : "Auto Organize ON\n(Right click to disable)"; // ModEntry.Instance.Helper.Translation.Get("AutoOrganizeButton.hoverText");
        }
    }
}
