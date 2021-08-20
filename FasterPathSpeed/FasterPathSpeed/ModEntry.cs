using StardewModdingAPI;
using HarmonyLib;

namespace FasterPathSpeed
{
    public class ModEntry : Mod
    {
        public ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            // Init config
            Config = Helper.ReadConfig<ModConfig>();

            // Handle Harmony patches
            FlooringPatches.Initialize(Monitor, Config);
            FarmerPatches.Initialize(Monitor, Config);

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Flooring), nameof(StardewValley.TerrainFeatures.Flooring.doCollisionAction)),
                postfix: new HarmonyMethod(typeof(FlooringPatches), nameof(FlooringPatches.DoCollisionAction_Postfix))
            );

            // Farmer GetMovementSpeed calculation is not touched unless we want paths to affect horse speed as well
            if (Config.IsPathAffectHorseSpeed)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.getMovementSpeed)),
                    postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.GetMovementSpeed_Postfix))
                );
            }
        }
    }
}
