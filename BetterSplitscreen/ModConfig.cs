using System.Collections.Generic;
using SplitscreenImproved.Layout;
using SplitscreenImproved.ShowName;

namespace SplitscreenImproved
{
    public class ModConfig
    {
        public bool IsModEnabled { get; set; } = true;

        public LayoutFeatureConfig LayoutFeature { get; set; } = new();

        public MusicFixFeatureConfig MusicFixFeature { get; set; } = new();

        public HudTweaksFeatureConfig HudTweaksFeature { get; set; } = new();

        public ShowNameFeatureConfig ShowNameFeature { get; set; } = new();

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

        public class ShowNameFeatureConfig
        {
            public bool IsFeatureEnabled { get; set; } = true;

            public bool IsSplitscreenOnly { get; set; } = true;

            public ShowNamePosition Position { get; set; } = ShowNamePosition.Top;
        }

        public class MusicFixFeatureConfig
        {
            // DEBUG
            public bool IsDebugMode { get; set; } = false;
            // DEBUG

            public bool IsFeatureEnabled { get; set; } = true;
        }

        public class HudTweaksFeatureConfig
        {
            // DEBUG
            public bool IsDebugMode { get; set; } = true;
            // DEBUG

            public bool IsFeatureEnabled { get; set; } = true;

            public bool IsSplitscreenOnly { get; set; } = false;

            public bool IsToolbarHudOffsetEnabled { get; set; } = true;
        }
    }
}
