﻿using System.Collections.Generic;
using SplitscreenImproved.Layout;
using SplitscreenImproved.ShowName;

namespace SplitscreenImproved
{
    public class ModConfig
    {
        public bool IsModEnabled { get; set; } = true;

        public LayoutFeatureConfig LayoutFeature { get; set; } = new();

        public ShowNameFeatureConfig ShowNameFeature { get; set; } = new();

        public MusicFixFeatureConfig MusicFixFeature { get; set; } = new();

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
            public bool IsFeatureEnabled { get; set; } = true;

            public int PrecedentPlayerNumber { get; set; } = 1;
            //public int[] PlayerPrecedence { get; set; } = new[] { 1, 2, 3, 4 };
        }
    }
}
