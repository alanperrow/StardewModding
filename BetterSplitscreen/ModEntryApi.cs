using System;
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
                tooltip: () => "The number of players to be displayed in the layout preview.\n(This value is not saved to the config)",
                allowedValues: playerCountOptions,
                fieldId: fieldId_PreviewPlayerCount);

            // TODO: Float number sliders for custom preset screen splits


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
                    }
                });
        }
    }
}
