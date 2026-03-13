using System;
using StardewModdingAPI;

namespace ConvenientInventory.Compatibility
{
    /// <inheritdoc/>
    /// <remarks>Includes new features as of GMCM version 1.16.0.</remarks>
    public interface IGenericModConfigMenuApi16 : IGenericModConfigMenuApi
    {
        /// <summary>Add a subheader at the current position in the form.</summary>
        /// <remarks>Larger than paragraph, smaller than title.</remarks>
        /// <param name="mod">The mod's manifest.</param>
        /// <param name="text">The title text shown in the form.</param>
        void AddSubHeader(IManifest mod, Func<string> text);

        /// <summary>Open the config UI for a specific mod, as a child menu if there is an existing menu.</summary>
        /// <param name="mod">The mod's manifest.</param>
        void OpenModMenuAsChildMenu(IManifest mod);
    }
}