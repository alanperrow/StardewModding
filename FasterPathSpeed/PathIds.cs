using System.Collections.Generic;

namespace FasterPathSpeed
{
    public static class PathIds
    {
        public const string Boardwalk = "405";
        public const string Wood = "328";
        public const string PlankFlooring = "840";
        public const string Ghost = "331";
        public const string Straw = "401";
        public const string Gravel = "407";
        public const string Cobblestone = "411";
        public const string SteppingStone = "415";
        public const string Stone = "329";
        public const string TownFlooring = "841";
        public const string Brick = "293";
        public const string ColoredCobblestone = "409";
        public const string IceTile = "333";

        public static readonly List<string> WhichIds = new()
        {
            Wood,
            Stone,
            Ghost,
            IceTile,
            Straw,
            Gravel,
            Boardwalk,
            ColoredCobblestone,
            Cobblestone,
            SteppingStone,
            Brick,
            PlankFlooring,
            TownFlooring
        };
    }
}
