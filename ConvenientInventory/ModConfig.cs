using ConvenientInventory.QuickStack;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ConvenientInventory
{
    /// <summary>Represents the mod configuration. Acts as a wrapper around <see cref="SerializableModConfig"/>.</summary>
    public class ModConfig
    {
        private readonly IModHelper _helper;
        private SerializableModConfig serialConfig;

        /// <summary>Creates a new instance of <see cref="ModConfig"/>, wrapping an instance of <see cref="SerializableModConfig"/>.</summary>
        public ModConfig(SerializableModConfig serializableModConfig, IModHelper helper)
        {
            _helper = helper;
            serialConfig = serializableModConfig;

            QuickStack = new QuickStackConfig(this);
            FavoriteItems = new FavoriteItemsConfig(this);
            TakeAllButOne = new TakeAllButOneConfig(this);
            AutoOrganizeChest = new AutoOrganizeChestConfig(this);
            Miscellaneous = new MiscellaneousConfig(this);
        }

        /// <inheritdoc cref="QuickStackConfig"/>
        public QuickStackConfig QuickStack { get; }

        /// <inheritdoc cref="FavoriteItemsConfig"/>
        public FavoriteItemsConfig FavoriteItems { get; }

        /// <inheritdoc cref="TakeAllButOneConfig"/>
        public TakeAllButOneConfig TakeAllButOne { get; }

        /// <inheritdoc cref="AutoOrganizeChestConfig"/>
        public AutoOrganizeChestConfig AutoOrganizeChest { get; }

        /// <inheritdoc cref="MiscellaneousConfig"/>
        public MiscellaneousConfig Miscellaneous { get; }

        /// <summary>Creates a new instance of <see cref="ModConfig"/> loaded from the mod's configuration file.</summary>
        public static ModConfig Load(IModHelper helper)
        {
            var serializableModConfig = helper.ReadConfig<SerializableModConfig>();

            ModConfig config = new(serializableModConfig, helper);
            config.QuickStack.Range = ConfigHelper.ValidateAndConstrainQuickStackRange(config.QuickStack.Range);

            return config;
        }

        /// <summary>Replaces the internal serializable config with a new default instance.</summary>
        public void Reset() => serialConfig = new SerializableModConfig();

        /// <summary>Saves the internal serializable config to the mod's configuration file.</summary>
        public void Save() => _helper.WriteConfig(serialConfig);

        /// <summary>Config settings for the Quick Stack feature.</summary>
        public class QuickStackConfig
        {
            private readonly ModConfig _config;

            public QuickStackConfig(ModConfig modConfig)
            {
                _config = modConfig;
            }

            public bool IsEnabled
            {
                get => _config.serialConfig.IsEnableQuickStack;
                set => _config.serialConfig.IsEnableQuickStack = value;
            }

            public bool IsHotkeyEnabled
            {
                get => _config.serialConfig.IsEnableQuickStackHotkey;
                set => _config.serialConfig.IsEnableQuickStackHotkey = value;
            }

            public KeybindList KeyboardHotkey
            {
                get => _config.serialConfig.QuickStackKeyboardHotkey;
                set => _config.serialConfig.QuickStackKeyboardHotkey = value;
            }

            public KeybindList ControllerHotkey
            {
                get => _config.serialConfig.QuickStackControllerHotkey;
                set => _config.serialConfig.QuickStackControllerHotkey = value;
            }

            public string Range
            {
                get => _config.serialConfig.QuickStackRange;
                set => _config.serialConfig.QuickStackRange = value;
            }

            public bool OverflowItems
            {
                get => _config.serialConfig.IsQuickStackOverflowItems;
                set => _config.serialConfig.IsQuickStackOverflowItems = value;
            }

            public bool IgnoreItemQuality
            {
                get => _config.serialConfig.IsQuickStackIgnoreItemQuality;
                set => _config.serialConfig.IsQuickStackIgnoreItemQuality = value;
            }

            public bool IgnoreItemVariation
            {
                get => _config.serialConfig.IsQuickStackIgnoreItemVariation;
                set => _config.serialConfig.IsQuickStackIgnoreItemVariation = value;
            }

            public NonStackableTypes NonStackableTypesToOverflow
            {
                get => _config.serialConfig.QuickStackNonStackableTypesToOverflow;
                set => _config.serialConfig.QuickStackNonStackableTypesToOverflow = value;
            }

            public bool AllowHotkeyInChestMenu
            {
                get => _config.serialConfig.IsQuickStackAllowHotkeyInChestMenu;
                set => _config.serialConfig.IsQuickStackAllowHotkeyInChestMenu = value;
            }

            public bool WithFillStacksButton
            {
                get => _config.serialConfig.IsQuickStackWithFillStacksButton;
                set => _config.serialConfig.IsQuickStackWithFillStacksButton = value;
            }

            public bool VisuallyOverrideFillStacksButton
            {
                get => _config.serialConfig.IsQuickStackVisuallyOverrideFillStacksButton;
                set => _config.serialConfig.IsQuickStackVisuallyOverrideFillStacksButton = value;
            }

            public bool IsToggleChestEnabled
            {
                get => _config.serialConfig.IsEnableQuickStackToggleChest;
                set => _config.serialConfig.IsEnableQuickStackToggleChest = value;
            }

            public bool IsPrioritizeChestEnabled
            {
                get => _config.serialConfig.IsEnableQuickStackPrioritizeChest;
                set => _config.serialConfig.IsEnableQuickStackPrioritizeChest = value;
            }

            public bool IsToggleChestButtonHidden
            {
                get => _config.serialConfig.IsQuickStackToggleChestButtonHidden;
                set => _config.serialConfig.IsQuickStackToggleChestButtonHidden = value;
            }

            public bool IntoMills
            {
                get => _config.serialConfig.IsQuickStackIntoMills;
                set => _config.serialConfig.IsQuickStackIntoMills = value;
            }

            public bool IntoJunimoHuts
            {
                get => _config.serialConfig.IsQuickStackIntoJunimoHuts;
                set => _config.serialConfig.IsQuickStackIntoJunimoHuts = value;
            }

            public bool IntoBuildingsWithInventories => IntoMills || IntoJunimoHuts;

            public bool IntoDressers
            {
                get => _config.serialConfig.IsQuickStackIntoDressers;
                set => _config.serialConfig.IsQuickStackIntoDressers = value;
            }

            public bool IntoHoppers
            {
                get => _config.serialConfig.IsQuickStackIntoHoppers;
                set => _config.serialConfig.IsQuickStackIntoHoppers = value;
            }

            public bool IntoMiniShippingBins
            {
                get => _config.serialConfig.IsQuickStackIntoMiniShippingBins;
                set => _config.serialConfig.IsQuickStackIntoMiniShippingBins = value;
            }

            public bool IsAnimationEnabled
            {
                get => _config.serialConfig.IsEnableQuickStackAnimation;
                set => _config.serialConfig.IsEnableQuickStackAnimation = value;
            }

            public bool IsChestAnimationEnabled
            {
                get => _config.serialConfig.IsEnableQuickStackChestAnimation;
                set => _config.serialConfig.IsEnableQuickStackChestAnimation = value;
            }

            public float AnimationItemSpeedFactor
            {
                get => _config.serialConfig.QuickStackAnimationItemSpeed;
                set => _config.serialConfig.QuickStackAnimationItemSpeed = value;
            }

            public float AnimationStackSpeedFactor
            {
                get => _config.serialConfig.QuickStackAnimationStackSpeed;
                set => _config.serialConfig.QuickStackAnimationStackSpeed = value;
            }

            public bool DrawChestsInButtonTooltip
            {
                get => _config.serialConfig.IsQuickStackTooltipDrawNearbyChests;
                set => _config.serialConfig.IsQuickStackTooltipDrawNearbyChests = value;
            }

            public bool SuppressSoundWhenNoNearbyChests
            {
                get => _config.serialConfig.IsSuppressSoundWhenNoNearbyChests;
                set => _config.serialConfig.IsSuppressSoundWhenNoNearbyChests = value;
            }
        }

        /// <summary>Config settings for the Favorite Items feature.</summary>
        public class FavoriteItemsConfig
        {
            private readonly ModConfig _config;

            public FavoriteItemsConfig(ModConfig modConfig)
            {
                _config = modConfig;
            }

            public bool IsEnabled
            {
                get => _config.serialConfig.IsEnableFavoriteItems;
                set => _config.serialConfig.IsEnableFavoriteItems = value;
            }

            public KeybindList KeyboardHotkey
            {
                get => _config.serialConfig.FavoriteItemsKeyboardHotkey;
                set => _config.serialConfig.FavoriteItemsKeyboardHotkey = value;
            }

            public KeybindList ControllerHotkey
            {
                get => _config.serialConfig.FavoriteItemsControllerHotkey;
                set => _config.serialConfig.FavoriteItemsControllerHotkey = value;
            }

            public int HighlightTextureChoice
            {
                get => _config.serialConfig.FavoriteItemsHighlightTextureChoice;
                set => _config.serialConfig.FavoriteItemsHighlightTextureChoice = value;
            }

            public bool UseCustomHighlightColor
            {
                get => _config.serialConfig.IsFavoriteItemsUseCustomHighlightColor;
                set => _config.serialConfig.IsFavoriteItemsUseCustomHighlightColor = value;
            }

            public Color CustomHighlightColor
            {
                get => _config.serialConfig.FavoriteItemsCustomHighlightColor;
                set => _config.serialConfig.FavoriteItemsCustomHighlightColor = value;
            }
        }

        /// <summary>Config settings for the Take All But One feature.</summary>
        public class TakeAllButOneConfig
        {
            private readonly ModConfig _config;

            public TakeAllButOneConfig(ModConfig modConfig)
            {
                _config = modConfig;
            }

            public bool IsEnabled
            {
                get => _config.serialConfig.IsEnableTakeAllButOne;
                set => _config.serialConfig.IsEnableTakeAllButOne = value;
            }

            public KeybindList KeyboardHotkey
            {
                get => _config.serialConfig.TakeAllButOneKeyboardHotkey;
                set => _config.serialConfig.TakeAllButOneKeyboardHotkey = value;
            }

            public KeybindList ControllerHotkey
            {
                get => _config.serialConfig.TakeAllButOneControllerHotkey;
                set => _config.serialConfig.TakeAllButOneControllerHotkey = value;
            }
        }

        /// <summary>Config settings for the Auto Organize Chest feature.</summary>
        public class AutoOrganizeChestConfig
        {
            private readonly ModConfig _config;

            public AutoOrganizeChestConfig(ModConfig modConfig)
            {
                _config = modConfig;
            }

            public bool IsEnabled
            {
                get => _config.serialConfig.IsEnableAutoOrganizeChest;
                set => _config.serialConfig.IsEnableAutoOrganizeChest = value;
            }

            public bool ShowInstructionsInTooltip
            {
                get => _config.serialConfig.IsShowAutoOrganizeButtonInstructions;
                set => _config.serialConfig.IsShowAutoOrganizeButtonInstructions = value;
            }
        }

        /// <summary>Config settings for miscellaneous features.</summary>
        public class MiscellaneousConfig
        {
            private readonly ModConfig _config;

            public MiscellaneousConfig(ModConfig modConfig)
            {
                _config = modConfig;
            }

            public bool IsInventoryPageSideWarpEnabled
            {
                get => _config.serialConfig.IsEnableInventoryPageSideWarp;
                set => _config.serialConfig.IsEnableInventoryPageSideWarp = value;
            }
        }
    }
}
