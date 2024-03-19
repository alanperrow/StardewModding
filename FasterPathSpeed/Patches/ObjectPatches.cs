using StardewModdingAPI;
using StardewValley;

namespace FasterPathSpeed.Patches
{
    public class ObjectPatches
    {
        public static void PlacementAction_Postfix(Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer who)
        {
            try
            {
                FasterPathSpeed.ObjectPlacementAction(__instance, ref __result, location, x, y, who);
            }
            catch (System.Exception e)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(PlacementAction_Postfix)}:\n{e}", LogLevel.Error);
            }
        }
    }
}
