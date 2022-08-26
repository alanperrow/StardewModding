using Microsoft.Xna.Framework.Graphics;

namespace ConvenientInventory.Compatibility.TypedChests
{
    public class ContentPackChestType
    {
        public string UniqueModId { get; set; }

        public string Name { get; set; }

        public bool IsSupported { get; private set; }

        // TODO: Find out how to get Content Pack image (content pipeline? directly from path?)
        private Texture2D assetTexture;

        /// <summary>
        /// Holds information about a type of chest implemented with a mod content pack.
        /// </summary>
        /// <param name="uniqueModId">The mod's unique ID.</param>
        /// <param name="name">Name of the mod's chest type (dictionary value).</param>
        public ContentPackChestType(string uniqueModId, string name)
        {
            UniqueModId = uniqueModId;
            Name = name;

            // TODO: (1) Check if automatic support for Content Pack chests is possible.
            //       (2) If not, initialize all supported mod chest data in ModEntry, and reference that here (rather than for each chest).
            IsSupported = false;

            /*
            IsSupported = ModEntry.Instance.Helper.ModRegistry.IsLoaded(UniqueModId);

            if (!IsSupported)
            {
                ModEntry.Instance.Monitor
                    .Log($"Mod: {uniqueModId} was not loaded, so chest of type: {name}.", StardewModdingAPI.LogLevel.Trace);
            }
            */
        }
    }
}
