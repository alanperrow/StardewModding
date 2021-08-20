using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ToggleableWateringCan
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            if (Context.IsWorldReady)
            {
                helper.Events.Input.ButtonPressed += TestMethod;
            }
        }

        private void TestMethod(object sender, ButtonPressedEventArgs e)
        {
            

        }
    }
}
