using System.Collections.Generic;
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

            public Dictionary<LayoutPreset, SplitscreenLayout> LayoutPresets { get; } = new()
            {
                { LayoutPreset.Default, new SplitscreenLayout(LayoutPreset.Default) },
                { LayoutPreset.SwapSides, new SplitscreenLayout(LayoutPreset.SwapSides) },
                { LayoutPreset.Custom, new SplitscreenLayout(LayoutPreset.Custom) },
            };
        }

        //public MusicFeatureConfig MusicFeature { get; set; } = new();
    }
}
