using Microsoft.Xna.Framework;

namespace SplitscreenImproved.Layout
{
    public class SplitscreenLayout
    {
        public SplitscreenLayout(LayoutPreset layoutPreset = LayoutPreset.Default)
        {
            Preset = layoutPreset;

            TwoPlayerLayout = new SplitscreenLayoutData(2, layoutPreset);
            ThreePlayerLayout = new SplitscreenLayoutData(3, layoutPreset);
            FourPlayerLayout = new SplitscreenLayoutData(4, layoutPreset);
        }

        public SplitscreenLayoutData TwoPlayerLayout { get; }

        public SplitscreenLayoutData ThreePlayerLayout { get; }

        public SplitscreenLayoutData FourPlayerLayout { get; }

        private LayoutPreset Preset { get; }

        // Singleplayer layout should not be configurable.
        private SplitscreenLayoutData SinglePlayerLayout { get; } = new(1);

        public Vector4[] GetScreenSplits(int numScreens)
        {
            if (numScreens < 1)
            {
                numScreens = 1;
            }

            switch (numScreens)
            {
                case 1:
                    return SinglePlayerLayout.ScreenSplits;
                case 2:
                    return TwoPlayerLayout.ScreenSplits;
                case 3:
                    return ThreePlayerLayout.ScreenSplits;
                default:
                    return FourPlayerLayout.ScreenSplits;
            }
        }
    }
}
