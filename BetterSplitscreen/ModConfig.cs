using BetterSplitscreen.Layout;

namespace BetterSplitscreen
{
    public class ModConfig
    {
        public bool IsModEnabled { get; set; } = true;

        public LayoutFeatureConfig LayoutFeature { get; set; } = new();

        public class LayoutFeatureConfig
        {
            public bool IsFeatureEnabled { get; set; } = true;

            public LayoutPreset PresetChoice { get; set; } = LayoutPreset.Default;

            public SplitscreenLayout CurrentLayout { get; set; } = new SplitscreenLayout();

            // IDEA: Remove CurrentLayout and Add new property: List<SplitscreenLayout> LayoutPresets
            //       Should have Default, SwapSides, and Custom preset values already included.
            //       This way users can define custom presets in JSON rather than needing to use GMCM.
        }

        //public MusicFeatureConfig MusicFeature { get; set; } = new();
    }
}
