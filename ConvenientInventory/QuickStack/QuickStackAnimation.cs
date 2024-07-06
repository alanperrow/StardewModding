using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace ConvenientInventory.QuickStack
{
    /// <summary>
    /// Handles the creation and broadcasting of item sprites to be used in a quick stack animation where
    /// items are visually deposited into their respective chests.
    /// </summary>
    public class QuickStackAnimation
    {
        /// <summary>
        /// Creates a new instance of <see cref="QuickStackAnimation"/> with reference to the provided farmer.
        /// </summary>
        /// <param name="who">The farmer triggering this animation.</param>
        public QuickStackAnimation(Farmer who)
        {
            Farmer = who;
            ItemSprites = new();
        }

        public int NumItemsAnimated { get; private set; }

        private TemporaryAnimatedSpriteList ItemSprites { get; }

        private Farmer Farmer { get; }

        /// <summary>
        /// Broadcasts item sprites to begin animation, synced with multiplayer.
        /// </summary>
        /// <param name="who">Farmer initiating the animation.</param>
        /// <returns>The total number of item stacks in the animation.</returns>
        public int Complete()
        {
            Game1.Multiplayer.broadcastSprites(Farmer.currentLocation, ItemSprites);
            return NumItemsAnimated;
        }

        /// <summary>
        /// Adds sprites to this <see cref="QuickStackAnimation"/> for <paramref name="item"/> to be visually quick stacked into <paramref name="chest"/>.
        /// </summary>
        /// <param name="chest">The chest to be quick stacked into.</param>
        /// <param name="item">The item being quick stacked.</param>
        public void AddToAnimation(Chest chest, Item item)
        {
            // TODO: Create a Dictionary<Chest, int> so each chest can have its own count, so that item delay is calculated per-chest.

            // DEBUG: Currently adding random number of item stacks for testing purposes.
            int numChestItemStacks = new Random().Next(8) + 1;
            for (int i = 0; i < numChestItemStacks; i++)
            {
                // TODO: Junimo Hut/Fridge/Mill item stack is being tossed toward top-left of current location, near (0,0). Investigate and fix.
                Vector2 chestPosition = (chest.TileLocation + new Vector2(0, -1.5f)) * Game1.tileSize;
                Vector2 farmerOffset = Farmer.FacingDirection switch
                {
                    0 => new Vector2(0f, -1.5f) * Game1.tileSize,   // Up
                    1 => new Vector2(0.5f, -1f) * Game1.tileSize,   // Right
                    3 => new Vector2(-0.5f, -1f) * Game1.tileSize,  // Left
                    _ => new Vector2(0f, -0.5f) * Game1.tileSize,   // Down
                };
                Vector2 farmerPosition = Farmer.Position + farmerOffset;

                float distance = Vector2.Distance(farmerPosition, chestPosition);
                Vector2 motionVec = (chestPosition - farmerPosition) * 0.98f; // 0.98 multiplier gives a better result in-game; without it, items slightly overshoot the chest.

                float CONFIG_animationSpeed = 1.0f; // TODO: Implement in ModConfig: range = [0.5 to 3, 0.1 interval]; 0.5x speed (half speed) up to 3x speed (triple speed).
                float tossTime = (float)(10 * Math.Pow(distance, 0.5)) + 400 - 0.5f * Math.Min(0, motionVec.Y) / CONFIG_animationSpeed;

                float extraHeight = 192 - Math.Min(0, motionVec.Y);
                float gravity = 2 * extraHeight / tossTime;
                motionVec.Y -= extraHeight;
                float extraX = motionVec.X;
                float xAccel = -2 * motionVec.X / tossTime;
                motionVec.X += extraX;

                float baseLayerDepth = (float)((chest.TileLocation.Y + 1) * 64) / 10000f + chest.TileLocation.X / 50000f; // Refactored from Object.draw()
                float addlayerDepth = 1E-06f * NumItemsAnimated; // Avoids z-fighting by drawing each sprite on a separate layer depth.

                int delayPerItem = 0;

                float CONFIG_animationHoverSpeed = 1.0f; // TODO: Implement in ModConfig: range = [0.5 to 3, 0.1 interval]; 0.5x speed (half speed) up to 3x speed (triple speed).
                int hoverTimePerItem = (int)(150 / CONFIG_animationHoverSpeed);
                int fadeTime = (int)(500 / CONFIG_animationHoverSpeed);

                ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem("(O)" + new Random().Next(100)); // DEBUG: Get random item sprite.
                //ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
                var itemTossSprite = new TemporaryAnimatedSprite(itemData.GetTextureName(), itemData.GetSourceRect(), farmerPosition, false, alphaFade: 0f, Color.White)
                {
                    delayBeforeAnimationStart = i * delayPerItem,
                    scale = 4f,
                    layerDepth = 1f - addlayerDepth,
                    totalNumberOfLoops = 0,
                    interval = tossTime,
                    motion = motionVec / tossTime,
                    acceleration = new Vector2(xAccel, gravity) / tossTime,
                    timeBasedMotion = true,
                };
                var itemHoverSprite = new TemporaryAnimatedSprite(itemData.GetTextureName(), itemData.GetSourceRect(), chestPosition, false, alphaFade: 0f, Color.White)
                {
                    delayBeforeAnimationStart = i * delayPerItem + (int)tossTime,
                    scale = 4f,
                    layerDepth = baseLayerDepth - addlayerDepth,
                    interval = i * hoverTimePerItem,
                };
                var itemFadeSprite = new TemporaryAnimatedSprite(itemData.GetTextureName(), itemData.GetSourceRect(), chestPosition, false, alphaFade: 0f, Color.White)
                {
                    delayBeforeAnimationStart = i * delayPerItem + (int)tossTime + i * hoverTimePerItem,
                    scale = 4f,
                    layerDepth = baseLayerDepth + addlayerDepth,
                    alphaFade = 0.04f / CONFIG_animationHoverSpeed,
                    interval = fadeTime,
                    motion = new Vector2(0.6f, 4.5f) / CONFIG_animationHoverSpeed,
                    acceleration = new Vector2(0f, -0.08f) / CONFIG_animationHoverSpeed,
                    scaleChange = -0.07f / CONFIG_animationHoverSpeed,
                };

                ItemSprites.Add(itemTossSprite);
                ItemSprites.Add(itemHoverSprite);
                ItemSprites.Add(itemFadeSprite);

                NumItemsAnimated++;

                int totalAnimationTimeMs = (i * delayPerItem + (int)tossTime + i * hoverTimePerItem + fadeTime);
                QuickStackChestAnimation.SetModData(chest, totalAnimationTimeMs);
            }
        }
    }
}
