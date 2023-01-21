using System.Collections.Generic;

namespace FasterPathSpeed
{
    public static class PathIds
    {
        public const int Boardwalk = 405;
        public const int Wood = 328;
        public const int PlankFlooring = 840;
        public const int Ghost = 331;
        public const int Straw = 401;
        public const int Gravel = 407;
        public const int Cobblestone = 411;
        public const int SteppingStone = 415;
        public const int Stone = 329;
        public const int TownFlooring = 841;
        public const int Brick = 293;
        public const int ColoredCobblestone = 409;
        public const int IceTile = 333;

        public static List<int> WhichIds = new List<int>()
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
