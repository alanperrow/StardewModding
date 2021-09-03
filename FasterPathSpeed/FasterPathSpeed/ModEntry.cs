using StardewModdingAPI;
using HarmonyLib;
using StardewModdingAPI.Events;
using GenericModConfigMenu;

namespace FasterPathSpeed
{
    public class ModEntry : Mod
    {
        public ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            FarmerPatches.Initialize(Monitor, Config);

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.getMovementSpeed)),
                postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.GetMovementSpeed_Postfix))
            );
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Get Generic Mod Config Menu API (if it's installed)
            IGenericModConfigMenuApi api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(
                    mod: ModManifest,
                    revertToDefault: () => Config = new ModConfig(),
                    saveToFile: () => Helper.WriteConfig(Config)
                );

                api.SetDefaultIngameOptinValue(ModManifest, true);

                api.RegisterClampedOption(
                    mod: ModManifest,
                    optionName: "Default Path Speed Buff",
                    optionDesc: "Extra movement speed obtained from walking on a path",
                    optionGet: () => Config.DefaultPathSpeedBuff,
                    optionSet: value => Config.DefaultPathSpeedBuff = value,
                    min: 0,
                    max: 5,
                    interval: 0.1f
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: "Only On Farm?",
                    optionDesc: "Whether the path speed buff is only obtained while on your farm",
                    optionGet: () => Config.IsPathSpeedBuffOnlyOnTheFarm,
                    optionSet: value => Config.IsPathSpeedBuffOnlyOnTheFarm = value
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: "Affect Horse Speed?",
                    optionDesc: "Whether the path speed buff is also obtained while riding a horse",
                    optionGet: () => Config.IsPathAffectHorseSpeed,
                    optionSet: value => Config.IsPathAffectHorseSpeed = value
                );
                api.RegisterClampedOption(
                    mod: ModManifest,
                    optionName: "Horse Speed Multiplier",
                    optionDesc: "Multiplier for path speed buff while riding a horse",
                    optionGet: () => Config.HorsePathSpeedBuffModifier,
                    optionSet: value => Config.HorsePathSpeedBuffModifier = value,
                    min: 0,
                    max: 2,
                    interval: 0.05f
                );
                api.RegisterSimpleOption(
                    mod: ModManifest,
                    optionName: "Use Custom Path Values?",
                    optionDesc: "Whether the default path speed buff is used, or each path has its own buff amount",
                    optionGet: () => Config.IsUseCustomPathSpeedBuffValues,
                    optionSet: value => Config.IsUseCustomPathSpeedBuffValues = value
                );
                foreach (var prop in Config.CustomPathSpeedBuffValues.GetType().GetProperties())
                {
                    api.RegisterClampedOption(
                        mod: ModManifest,
                        optionName: $" - Custom: {prop.Name}",
                        optionDesc: $"Extra movement speed obtained from walking on a {prop.Name} path",
                        optionGet: () => (float)prop.GetValue(Config.CustomPathSpeedBuffValues),
                        optionSet: value => prop.SetValue(Config.CustomPathSpeedBuffValues, value),
                        min: 0,
                        max: 5,
                        interval: 0.1f
                    );
                }
            }
        }
    }
}
