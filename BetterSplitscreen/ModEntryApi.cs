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
                draw: LayoutPreviewHelper.DrawPreview,
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

            string[] customLayoutPlayerNumberOptions = new string[] { "1", "2", "3", "4" };
            const string fieldId_CustomLayoutPlayerNumber = "CustomLayoutPlayerNumber";
            api.AddTextOption(
                mod: ModManifest,
                getValue: () => LayoutPreviewHelper.CustomLayoutPlayerNumber.ToString(),
                setValue: value =>
                {
                    int valueInt = int.Parse(value);
                    if (valueInt > LayoutPreviewHelper.PlayerCount)
                    {
                        // Cannot set player number to a value greater than the total number of players.
                        valueInt = LayoutPreviewHelper.PlayerCount;
                    }

                    LayoutPreviewHelper.CustomLayoutPlayerNumber = valueInt;
                },
                name: () => "Customize Layout for Player",
                tooltip: () => $"The player number for which to define a custom layout when playing {LayoutPreviewHelper.PlayerCount}-player splitscreen.\n" +
                    "This custom layout will be applied when using the \"Custom\" layout preset.\n" +
                    "To define a custom layout for splitscreen with a different number of players, change the selected value for \"Preview Player Count\" above.",
                allowedValues: customLayoutPlayerNumberOptions,
                fieldId: fieldId_CustomLayoutPlayerNumber);

            const string fieldId_CustomLayoutLeft = "CustomLayoutLeft";
            api.AddNumberOption(
                mod: ModManifest,
                getValue: () => GetCustomScreenSplitByPlayer(LayoutPreviewHelper.PlayerCount, LayoutPreviewHelper.CustomLayoutPlayerNumber).X,
                setValue: value => SetCustomScreenSplitByPlayer(LayoutPreviewHelper.PlayerCount, LayoutPreviewHelper.CustomLayoutPlayerNumber, ScreenSplitComponent.Left, value),
                min: 0f,
                max: 1f,
                interval: 0.01f,
                name: () => "Left",
                tooltip: () => "The left position of the screen for the player number specified by \"Customize Layout for Player\".",
                formatValue: (value) => $"P{LayoutPreviewHelper.CustomLayoutPlayerNumber}: {value}",
                fieldId: fieldId_CustomLayoutLeft);

            const string fieldId_CustomLayoutTop = "CustomLayoutTop";
            api.AddNumberOption(
                mod: ModManifest,
                getValue: () => GetCustomScreenSplitByPlayer(LayoutPreviewHelper.PlayerCount, LayoutPreviewHelper.CustomLayoutPlayerNumber).Y,
                setValue: value => SetCustomScreenSplitByPlayer(LayoutPreviewHelper.PlayerCount, LayoutPreviewHelper.CustomLayoutPlayerNumber, ScreenSplitComponent.Top, value),
                min: 0f,
                max: 1f,
                interval: 0.01f,
                name: () => "Top",
                tooltip: () => "The top position of the screen for the player number specified by \"Customize Layout for Player\".",
                formatValue: (value) => $"P{LayoutPreviewHelper.CustomLayoutPlayerNumber}: {value}",
                fieldId: fieldId_CustomLayoutTop);

            const string fieldId_CustomLayoutWidth = "CustomLayoutWidth";
            api.AddNumberOption(
                mod: ModManifest,
                getValue: () => GetCustomScreenSplitByPlayer(LayoutPreviewHelper.PlayerCount, LayoutPreviewHelper.CustomLayoutPlayerNumber).Z,
                setValue: value => SetCustomScreenSplitByPlayer(LayoutPreviewHelper.PlayerCount, LayoutPreviewHelper.CustomLayoutPlayerNumber, ScreenSplitComponent.Width, value),
                min: 0f,
                max: 1f,
                interval: 0.01f,
                name: () => "Width",
                tooltip: () => "The width of the screen for the player number specified by \"Customize Layout for Player\".",
                formatValue: (value) => $"P{LayoutPreviewHelper.CustomLayoutPlayerNumber}: {value}",
                fieldId: fieldId_CustomLayoutWidth);

            const string fieldId_CustomLayoutHeight = "CustomLayoutHeight";
            api.AddNumberOption(
                mod: ModManifest,
                getValue: () => GetCustomScreenSplitByPlayer(LayoutPreviewHelper.PlayerCount, LayoutPreviewHelper.CustomLayoutPlayerNumber).W,
                setValue: value => SetCustomScreenSplitByPlayer(LayoutPreviewHelper.PlayerCount, LayoutPreviewHelper.CustomLayoutPlayerNumber, ScreenSplitComponent.Height, value),
                min: 0f,
                max: 1f,
                interval: 0.01f,
                name: () => "Height",
                tooltip: () => "The height of the screen for the player number specified by \"Customize Layout for Player\".",
                formatValue: (value) => $"P{LayoutPreviewHelper.CustomLayoutPlayerNumber}: {value}",
                fieldId: fieldId_CustomLayoutHeight);

            Vector4 GetCustomScreenSplitByPlayer(int playerCount, int playerNumber)
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

            void SetCustomScreenSplitByPlayer(int playerCount, int playerNumber, ScreenSplitComponent screenSplitComponent, float value)
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

            // Register OnFieldChanged to support layout preview refreshing live (without requiring saving to config first).
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
                        case fieldId_CustomLayoutPlayerNumber:
                            int valueInt2 = int.Parse((string)value);
                            LayoutPreviewHelper.CustomLayoutPlayerNumber = valueInt2;
                            break;
                        case fieldId_CustomLayoutLeft:
                            SetLayoutPreviewScreenSplitByPlayer(
                                LayoutPreviewHelper.PlayerCount,
                                LayoutPreviewHelper.CustomLayoutPlayerNumber,
                                ScreenSplitComponent.Left,
                                (float)value);
                            break;
                        case fieldId_CustomLayoutTop:
                            SetLayoutPreviewScreenSplitByPlayer(
                                LayoutPreviewHelper.PlayerCount,
                                LayoutPreviewHelper.CustomLayoutPlayerNumber,
                                ScreenSplitComponent.Top,
                                (float)value);
                            break;
                        case fieldId_CustomLayoutWidth:
                            SetLayoutPreviewScreenSplitByPlayer(
                                LayoutPreviewHelper.PlayerCount,
                                LayoutPreviewHelper.CustomLayoutPlayerNumber,
                                ScreenSplitComponent.Width,
                                (float)value);
                            break;
                        case fieldId_CustomLayoutHeight:
                            SetLayoutPreviewScreenSplitByPlayer(
                                LayoutPreviewHelper.PlayerCount,
                                LayoutPreviewHelper.CustomLayoutPlayerNumber,
                                ScreenSplitComponent.Height,
                                (float)value);
                            break;
                    }
                });

            void SetLayoutPreviewScreenSplitByPlayer(int playerCount, int playerNumber, ScreenSplitComponent screenSplitComponent, float value)
            {
                SplitscreenLayoutData splitscreenLayoutData = playerCount switch
                {
                    4 => LayoutPreviewHelper.Layout.FourPlayerLayout,
                    3 => LayoutPreviewHelper.Layout.ThreePlayerLayout,
                    _ => LayoutPreviewHelper.Layout.TwoPlayerLayout,
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
}
