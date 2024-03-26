using Microsoft.Xna.Framework;
using StardewValley;

namespace BetterSplitscreen.Layout
{
    internal class SplitscreenLayoutData
    {
        public SplitscreenLayoutData(byte numScreens, LayoutPreset layoutPreset = LayoutPreset.Default)
        {
            if (numScreens < 1)
            {
                numScreens = 1;
            }

            NumScreens = numScreens;
            ScreenSplits = layoutPreset switch
            {
                LayoutPreset.SwapSides => GetSwapSidesScreenSplits(),
                _ => GetDefaultScreenSplits(),
            };
        }

        public byte NumScreens { get; }

        public Vector4[] ScreenSplits { get; }

        private Vector4[] GetDefaultScreenSplits()
        {
            var defaultScreenSplits = new Vector4[NumScreens];

            switch (NumScreens)
            {
                case 1:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 1f, 1f);
                    break;
                case 2:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 0.5f, 1f);
                    defaultScreenSplits[1] = new Vector4(0.5f, 0f, 0.5f, 1f);
                    break;
                case 3:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 1f, 0.5f);
                    defaultScreenSplits[1] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    defaultScreenSplits[2] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    break;
                default:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 0.5f, 0.5f);
                    defaultScreenSplits[1] = new Vector4(0.5f, 0f, 0.5f, 0.5f);
                    defaultScreenSplits[2] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    defaultScreenSplits[3] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    break;
            }

            return defaultScreenSplits;
        }

        private Vector4[] GetSwapSidesScreenSplits()
        {
            var defaultScreenSplits = new Vector4[NumScreens];

            switch (NumScreens)
            {
                case 1:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 1f, 1f);
                    break;
                case 2:
                    defaultScreenSplits[0] = new Vector4(0.5f, 0f, 0.5f, 1f);
                    defaultScreenSplits[1] = new Vector4(0f, 0f, 0.5f, 1f);
                    break;
                case 3:
                    defaultScreenSplits[0] = new Vector4(0f, 0f, 1f, 0.5f);
                    defaultScreenSplits[1] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    defaultScreenSplits[2] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    break;
                default:
                    defaultScreenSplits[0] = new Vector4(0.5f, 0f, 0.5f, 0.5f);
                    defaultScreenSplits[1] = new Vector4(0f, 0f, 0.5f, 0.5f);
                    defaultScreenSplits[2] = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    defaultScreenSplits[3] = new Vector4(0f, 0.5f, 0.5f, 0.5f);
                    break;
            }

            return defaultScreenSplits;
        }
    }
}
