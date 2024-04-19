using Microsoft.Xna.Framework.Graphics;

namespace ConvenientInventory.TypedChests
{
    internal static class CachedTextures
    {
        public static Texture2D Mill { get; } = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(@"Buildings\Mill");

        public static Texture2D JunimoHut { get; } = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(@"Buildings\Junimo Hut");

        public static Texture2D FarmHouse { get; } = ModEntry.Instance.Helper.GameContent.Load<Texture2D>(@"Maps\farmhouse_tiles");
    }
}
