namespace ConvenientInventory.QuickStack
{
    /// <summary>
    /// Represents the quick stack toggle state of a chest, for use by the Quick Stack Toggle Chest feature.
    /// </summary>
    public enum QuickStackToggleChestState
    {
        /// <summary>Quick stack is disabled for this chest.</summary>
        Disabled = 0,

        /// <summary>Quick stack is enabled for this chest.</summary>
        /// <remarks>This is the default state.</remarks>
        Enabled = 1,

        /// <summary>Quick stack is enabled for this chest, and has an additional priority level of 1.</summary>
        Priority1 = 2,

        /// <summary>Quick stack is enabled for this chest, and has an additional priority level of 2.</summary>
        Priority2 = 3,

        /// <summary>Quick stack is enabled for this chest, and has an additional priority level of 3.</summary>
        Priority3 = 4,
    }
}
