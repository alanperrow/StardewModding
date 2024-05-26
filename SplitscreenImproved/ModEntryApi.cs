using System;
using Microsoft.Xna.Framework;
using SplitscreenImproved.Compatibility;
using SplitscreenImproved.Layout;
using SplitscreenImproved.ShowName;

namespace SplitscreenImproved
{
    public partial class ModEntry
    {
        /// <summary>
        /// Initializes IGenericModConfigMenuApi.
        /// </summary>
        /// <param name="api">API instance.</param>
        public void Initialize(IGenericModConfigMenuApi api)
        {
            api.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config));

            const string fieldId_IsModEnabled = "IsModEnabled";
            api.AddBoolOption(
                mod: ModManifest,
                getValue: () =>
                {
                    // Initial assignment for default preview value.
                    if (!Config.IsModEnabled)
                    {
                        LayoutPreviewHelper.IsModEnabled = false;
                    }

                    return Config.IsModEnabled;
                },
                setValue: value =>
                {
                    if (Config.IsModEnabled == value)
                    {
                        return;
                    }

                    Config.IsModEnabled = value;

                    // Refresh window to apply config change.
                    StardewValley.Game1.game1.Window_ClientSizeChanged(null, null);
                },
                name: () => "Is Mod Enabled",
                tooltip: () => "Enables/disables the Better Splitscreen mod. This option has precedence over all others.",
                fieldId: fieldId_IsModEnabled);

            api.AddSectionTitle(
                mod: ModManifest,
                text: () => "Layout");

            const string fieldId_IsLayoutFeatureEnabled = "IsLayoutFeatureEnabled";
            api.AddBoolOption(
                mod: ModManifest,
                getValue: () =>
                {
                    // Initial assignment for default preview value.
                    if (!Config.LayoutFeature.IsFeatureEnabled)
                    {
                        LayoutPreviewHelper.IsLayoutFeatureEnabled = false;
                    }

                    return Config.LayoutFeature.IsFeatureEnabled;
                },
                setValue: value =>
                {
                    if (Config.LayoutFeature.IsFeatureEnabled == value)
                    {
                        return;
                    }

                    Config.LayoutFeature.IsFeatureEnabled = value;

                    // Refresh window to apply config change.
                    StardewValley.Game1.game1.Window_ClientSizeChanged(null, null);
                },
                name: () => "Is Layout Feature Enabled",
                tooltip: () => "Enables/disables the custom splitscreen layout feature.",
                fieldId: fieldId_IsLayoutFeatureEnabled);

            const string fieldId_LayoutPreset = "LayoutPreset";
            string[] layoutPresetNames = Enum.GetNames(typeof(LayoutPreset));
            api.AddTextOption(
                mod: ModManifest,
                getValue: () =>
                {
                    LayoutPreset preset = Config.LayoutFeature.PresetChoice;

                    // Initial assignment for default preview value.
                    LayoutPreviewHelper.Layout ??= Config.LayoutFeature.LayoutPresets[preset];

                    return preset.ToString();
                },
                setValue: value =>
                {
                    LayoutPreset valueEnum = Enum.Parse<LayoutPreset>(value);
                    if (Config.LayoutFeature.PresetChoice == valueEnum)
                    {
                        return;
                    }

                    Config.LayoutFeature.PresetChoice = valueEnum;

                    // Refresh window to apply config change.
                    StardewValley.Game1.game1.Window_ClientSizeChanged(null, null);
                },
                name: () => "Layout Preset",
                tooltip: () => "The currently selected layout preset.\n" +
                    " - 'Default': Vanilla splitscreen layout\n" +
                    " - 'SwapSides': Left and right screens are swapped\n" +
                    " - 'Custom': Use a custom layout (see below)",
                allowedValues: layoutPresetNames,
                fieldId: fieldId_LayoutPreset);

            api.AddComplexOption(
                mod: ModManifest,
                name: () => "Preview:",
                draw: (sb, p) => LayoutPreviewHelper.DrawPreview(sb, p),
                tooltip: () => "Preview how the currently selected layout preset will be displayed.",
                height: () => 200);

            string[] playerCountOptions = new string[] { "1", "2", "3", "4" };
            const string fieldId_PreviewPlayerCount = "PreviewPlayerCount";
            api.AddTextOption(
                mod: ModManifest,
                getValue: () => LayoutPreviewHelper.PlayerCount.ToString(),
                setValue: value => LayoutPreviewHelper.PlayerCount = int.Parse(value),
                name: () => "Preview Player Count",
                tooltip: () => "The number of players to be displayed in the layout preview above.\n" +
                    "The selected value can be used to define custom layouts per number of players/screens.",
                allowedValues: playerCountOptions,
                fieldId: fieldId_PreviewPlayerCount);

            api.AddPageLink(
                mod: ModManifest,
                pageId: "Custom Layout: 2-Player",
                text: () => "Custom Layout: 2-Player",
                tooltip: () => "Define the \"Custom\" layout preset for 2-player splitscreen.");

            api.AddPageLink(
                mod: ModManifest,
                pageId: "Custom Layout: 3-Player",
                text: () => "Custom Layout: 3-Player",
                tooltip: () => "Define the \"Custom\" layout preset for 3-player splitscreen.");

            api.AddPageLink(
                mod: ModManifest,
                pageId: "Custom Layout: 4-Player",
                text: () => "Custom Layout: 4-Player",
                tooltip: () => "Define the \"Custom\" layout preset for 4-player splitscreen.");

            api.AddSectionTitle(
                mod: ModManifest,
                text: () => "Music Fix");

            api.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.MusicFixFeature.IsFeatureEnabled,
                setValue: value => Config.MusicFixFeature.IsFeatureEnabled = value,
                name: () => "Is Music Fix Enabled",
                tooltip: () => "Enables/disables fix for bug where music stops and remains silent in splitscreen. " +
                    "This does not affect music in singleplayer or online multiplayer.");

            api.AddSectionTitle(
                mod: ModManifest,
                text: () => "HUD Tweaks");

            api.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.HudTweaksFeature.IsFeatureEnabled,
                setValue: value => Config.HudTweaksFeature.IsFeatureEnabled = value,
                name: () => "Is HUD Tweaks Enabled",
                tooltip: () => "Enables/disables all HUD tweaks.");

            api.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.HudTweaksFeature.IsSplitscreenOnly,
                setValue: value => Config.HudTweaksFeature.IsSplitscreenOnly = value,
                name: () => "Is Splitscreen Only",
                tooltip: () => "Enables/disables HUD tweaks only being applied in splitscreen.");

            api.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.HudTweaksFeature.IsToolbarHudOffsetEnabled,
                setValue: value => Config.HudTweaksFeature.IsToolbarHudOffsetEnabled = value,
                name: () => "Is Toolbar HUD Offset Enabled",
                tooltip: () => "Enables/disables various HUD UI elements being offset from the toolbar, " +
                    "allowing the toolbar to remain fully visible and not be obscured by them.");

            api.AddSectionTitle(
                mod: ModManifest,
                text: () => "Show Player Name In Summary");

            api.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.ShowNameFeature.IsFeatureEnabled,
                setValue: value => Config.ShowNameFeature.IsFeatureEnabled = value,
                name: () => "Is Show Name Feature Enabled",
                tooltip: () => "Enables/disables player name banner being shown in end-of-day shipping summary and level up menus.");

            api.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.ShowNameFeature.IsSplitscreenOnly,
                setValue: value => Config.ShowNameFeature.IsSplitscreenOnly = value,
                name: () => "Is Splitscreen Only",
                tooltip: () => "Enables/disables player name banner only being shown in splitscreen.");

            string[] showNamePositions = Enum.GetNames(typeof(ShowNamePosition));
            api.AddTextOption(
                mod: ModManifest,
                getValue: () => Config.ShowNameFeature.Position.ToString(),
                setValue: value => Config.ShowNameFeature.Position = Enum.Parse<ShowNamePosition>(value),
                name: () => "Position",
                tooltip: () => "Where to display player name banner on-screen.",
                allowedValues: showNamePositions);

            // Register OnFieldChanged to support main layout preview refreshing live without requiring saving to config first.
            api.OnFieldChanged(
                mod: ModManifest,
                onChange: (string fieldId, object value) =>
                {
                    switch (fieldId)
                    {
                        case fieldId_IsModEnabled:
                            bool valueBool = (bool)value;
                            LayoutPreviewHelper.IsModEnabled = valueBool;
                            break;
                        case fieldId_IsLayoutFeatureEnabled:
                            bool valueBool2 = (bool)value;
                            LayoutPreviewHelper.IsLayoutFeatureEnabled = valueBool2;
                            break;
                        case fieldId_LayoutPreset:
                            LayoutPreset valueLayoutPreset = Enum.Parse<LayoutPreset>((string)value);
                            LayoutPreviewHelper.Layout = Config.LayoutFeature.LayoutPresets[valueLayoutPreset];
                            break;
                        case fieldId_PreviewPlayerCount:
                            int valueInt = int.Parse((string)value);
                            LayoutPreviewHelper.PlayerCount = valueInt;
                            break;
                    }
                });

            // Create custom layout pages.
            api.AddPage(
                mod: ModManifest,
                pageId: "Custom Layout: 2-Player");

            api.AddComplexOption(
                mod: ModManifest,
                name: () => "Preview:",
                draw: (sb, p) => LayoutPreviewHelper.DrawPreview(sb, p, 2, Config.LayoutFeature.LayoutPresets[LayoutPreset.Custom]),
                tooltip: () => "Preview how the \"Custom\" layout preset will be displayed for 2-player splitscreen.",
                height: () => 200);

            CreateNumberOptionsForScreenSplitsByPlayer(api, playerCount: 2, playerNumber: 1);
            CreateNumberOptionsForScreenSplitsByPlayer(api, playerCount: 2, playerNumber: 2);

            api.AddPage(
                mod: ModManifest,
                pageId: "Custom Layout: 3-Player");

            api.AddComplexOption(
                mod: ModManifest,
                name: () => "Preview:",
                draw: (sb, p) => LayoutPreviewHelper.DrawPreview(sb, p, 3, Config.LayoutFeature.LayoutPresets[LayoutPreset.Custom]),
                tooltip: () => "Preview how the \"Custom\" layout preset will be displayed for 3-player splitscreen.",
                height: () => 200);

            CreateNumberOptionsForScreenSplitsByPlayer(api, playerCount: 3, playerNumber: 1);
            CreateNumberOptionsForScreenSplitsByPlayer(api, playerCount: 3, playerNumber: 2);
            CreateNumberOptionsForScreenSplitsByPlayer(api, playerCount: 3, playerNumber: 3);

            api.AddPage(
                mod: ModManifest,
                pageId: "Custom Layout: 4-Player");

            api.AddComplexOption(
                mod: ModManifest,
                name: () => "Preview:",
                draw: (sb, p) => LayoutPreviewHelper.DrawPreview(sb, p, 4, Config.LayoutFeature.LayoutPresets[LayoutPreset.Custom]),
                tooltip: () => "Preview how the \"Custom\" layout preset will be displayed for 4-player splitscreen.",
                height: () => 200);

            CreateNumberOptionsForScreenSplitsByPlayer(api, playerCount: 4, playerNumber: 1);
            CreateNumberOptionsForScreenSplitsByPlayer(api, playerCount: 4, playerNumber: 2);
            CreateNumberOptionsForScreenSplitsByPlayer(api, playerCount: 4, playerNumber: 3);
            CreateNumberOptionsForScreenSplitsByPlayer(api, playerCount: 4, playerNumber: 4);
        }

        private void CreateNumberOptionsForScreenSplitsByPlayer(IGenericModConfigMenuApi api, int playerCount, int playerNumber)
        {
            api.AddNumberOption(
                mod: ModManifest,
                getValue: () => GetCustomScreenSplitByPlayer(playerCount, playerNumber).X,
                setValue: value => SetCustomScreenSplitByPlayer(playerCount, playerNumber, ScreenSplitComponent.Left, value),
                min: 0f,
                max: 1f,
                interval: 0.01f,
                name: () => $"P{playerNumber}: Left",
                tooltip: () => $"The left position of the screen for player {playerNumber}.");

            api.AddNumberOption(
                mod: ModManifest,
                getValue: () => GetCustomScreenSplitByPlayer(playerCount, playerNumber).Y,
                setValue: value => SetCustomScreenSplitByPlayer(playerCount, playerNumber, ScreenSplitComponent.Top, value),
                min: 0f,
                max: 1f,
                interval: 0.01f,
                name: () => $"P{playerNumber}: Top",
                tooltip: () => $"The top position of the screen for player {playerNumber}.");

            api.AddNumberOption(
                mod: ModManifest,
                getValue: () => GetCustomScreenSplitByPlayer(playerCount, playerNumber).Z,
                setValue: value => SetCustomScreenSplitByPlayer(playerCount, playerNumber, ScreenSplitComponent.Width, value),
                min: 0f,
                max: 1f,
                interval: 0.01f,
                name: () => $"P{playerNumber}: Width",
                tooltip: () => $"The width of the screen for player {playerNumber}.");

            api.AddNumberOption(
                mod: ModManifest,
                getValue: () => GetCustomScreenSplitByPlayer(playerCount, playerNumber).W,
                setValue: value => SetCustomScreenSplitByPlayer(playerCount, playerNumber, ScreenSplitComponent.Height, value),
                min: 0f,
                max: 1f,
                interval: 0.01f,
                name: () => $"P{playerNumber}: Height",
                tooltip: () => $"The height of the screen for player {playerNumber}.");
        }

        private static Vector4 GetCustomScreenSplitByPlayer(int playerCount, int playerNumber)
        {
            SplitscreenLayoutData splitscreenLayoutData = playerCount switch
            {
                4 => Config.LayoutFeature.LayoutPresets[LayoutPreset.Custom].FourPlayerLayout,
                3 => Config.LayoutFeature.LayoutPresets[LayoutPreset.Custom].ThreePlayerLayout,
                _ => Config.LayoutFeature.LayoutPresets[LayoutPreset.Custom].TwoPlayerLayout,
            };

            if (playerNumber > playerCount)
            {
                playerNumber = playerCount;
            }

            return splitscreenLayoutData.ScreenSplits[playerNumber - 1];
        }

        private static void SetCustomScreenSplitByPlayer(int playerCount, int playerNumber, ScreenSplitComponent screenSplitComponent, float value)
        {
            SplitscreenLayoutData splitscreenLayoutData = playerCount switch
            {
                4 => Config.LayoutFeature.LayoutPresets[LayoutPreset.Custom].FourPlayerLayout,
                3 => Config.LayoutFeature.LayoutPresets[LayoutPreset.Custom].ThreePlayerLayout,
                _ => Config.LayoutFeature.LayoutPresets[LayoutPreset.Custom].TwoPlayerLayout,
            };

            if (playerNumber > playerCount)
            {
                playerNumber = playerCount;
            }

            Vector4 split = splitscreenLayoutData.ScreenSplits[playerNumber - 1];
            switch (screenSplitComponent)
            {
                case ScreenSplitComponent.Left:
                    splitscreenLayoutData.ScreenSplits[playerNumber - 1] = new Vector4(value, split.Y, split.Z, split.W);
                    break;
                case ScreenSplitComponent.Top:
                    splitscreenLayoutData.ScreenSplits[playerNumber - 1] = new Vector4(split.X, value, split.Z, split.W);
                    break;
                case ScreenSplitComponent.Width:
                    splitscreenLayoutData.ScreenSplits[playerNumber - 1] = new Vector4(split.X, split.Y, value, split.W);
                    break;
                default:
                    splitscreenLayoutData.ScreenSplits[playerNumber - 1] = new Vector4(split.X, split.Y, split.Z, value);
                    break;
            };
        }
    }
}
