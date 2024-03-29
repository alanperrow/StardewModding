using System;
using BetterSplitscreen.Compatibility;
using BetterSplitscreen.Layout;

namespace BetterSplitscreen
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
                getValue: () => Config.IsModEnabled,
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
                getValue: () => Config.LayoutFeature.IsFeatureEnabled,
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
                getValue: () => Config.LayoutFeature.PresetChoice.ToString(),
                setValue: value =>
                {
                    LayoutPreset valueEnum = Enum.Parse<LayoutPreset>(value);
                    if (Config.LayoutFeature.PresetChoice == valueEnum)
                    {
                        return;
                    }

                    Config.LayoutFeature.PresetChoice = valueEnum;

                    // TODO: Temporary solution just to get mod config working.
                    //       We should be storing each custom layout definition somewhere, rather than simply overwriting it every time.
                    //       IDEA: Include a json file to store all layout definitions. (See Modconfig comment about saving presets)
                    Config.LayoutFeature.CurrentLayout = new SplitscreenLayout(Config.LayoutFeature.PresetChoice);

                    // Refresh window to apply config change.
                    StardewValley.Game1.game1.Window_ClientSizeChanged(null, null);
                },
                name: () => "Layout Preset",
                tooltip: () => "The currently selected layout preset.\n" +
                    "'Default' = Vanilla splitscreen layout\n" +
                    "'SwapSides' = Left and right screens are swapped\n" +
                    "'Custom' = Use a custom layout (see below)",
                allowedValues: layoutPresetNames,
                fieldId: fieldId_LayoutPreset);

            // TODO: Make a nice pretty graphic with red/blue/green/yellow boxes representing each individual splitscreen position.
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
                tooltip: () => "The number of players to be displayed in the layout preview.",
                allowedValues: playerCountOptions,
                fieldId: fieldId_PreviewPlayerCount);

            // Register OnFieldChanged to support layout preview refreshing live (without requiring saving to config first).
            api.OnFieldChanged(
                mod: ModManifest,
                onChange: (string fieldId, object value) =>
                {
                    switch (fieldId)
                    {
                        case fieldId_IsModEnabled:
                        case fieldId_IsLayoutFeatureEnabled:
                            // TODO: Refresh preview based on IsEnabled value
                            break;
                        case fieldId_LayoutPreset:
                            // TODO: Refresh preview based on LayoutPreset value
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
