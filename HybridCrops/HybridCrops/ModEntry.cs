using StardewModdingAPI;
using HarmonyLib;

namespace HybridCrops
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
        }
    }
}
