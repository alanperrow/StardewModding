using System;
using System.Collections.Generic;
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
        }

        public int NumAnimatedItems { get; private set; }

        private TemporaryAnimatedSpriteList ItemSprites { get; } = new();

        private Farmer Farmer { get; }

        private Dictionary<Chest, int> NumAnimatedItemsByChest { get; } = new();

        /// <summary>
        /// Broadcasts item sprites to begin animation, synced with multiplayer.
        /// </summary>
        /// <param name="who">Farmer initiating the animation.</param>
        /// <returns>The total number of item stacks in the animation.</returns>
        public int Complete()
        {
            Game1.Multiplayer.broadcastSprites(Farmer.currentLocation, ItemSprites);
            return NumAnimatedItems;
        }

        /// <summary>
        /// Adds sprites to this <see cref="QuickStackAnimation"/> for <paramref name="item"/> to be visually quick stacked into <paramref name="chest"/>.
        /// </summary>
        /// <param name="chest">The chest to be quick stacked into.</param>
        /// <param name="item">The item being quick stacked.</param>
        public void AddToAnimation(Chest chest, Item item)
        {
            if (!NumAnimatedItemsByChest.TryGetValue(chest, out int numChestAnimatedItems))
            {
                numChestAnimatedItems = 0;
                NumAnimatedItemsByChest[chest] = 0;
            }

            // TODO: Junimo Hut/Fridge/Mill item stack is being tossed toward top-left of current location, near (0,0). Investigate and fix.
            Vector2 chestPosition = (chest.TileLocation + new Vector2(0, -1.5f)) * Game1.tileSize;
            Vector2 farmerOffset = Farmer.FacingDirection switch
            {
                0 => new Vector2(0f, -1.5f) * Game1.tileSize,   // Up
                1 => new Vector2(0.5f, -1f) * Game1.tileSize,   // Right
                3 => new Vector2(-0.5f, -1f) * Game1.tileSize,  // Left
                _ => new Vector2(0f, -1f) * Game1.tileSize,   // Down
            };

            // TODO: Instead of randomizing farmer offset, we should do a spiral pattern to offset hover position.
            //       The more items we stack per chest, the further we should increase the radial offset, spirally, from the original hover position.
            //       When hovering, use a motion vector that slowly moves to the original hover position, so when it is time to fade, we are back at the og hover position.
            Vector2 randFarmerOffset = new(new Random().NextSingle() * 16f - 8f, new Random().NextSingle() * 16f - 8f);

            Vector2 farmerPosition = Farmer.Position + farmerOffset + randFarmerOffset;

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
            float addlayerDepth = 1E-06f * NumAnimatedItems; // Avoids z-fighting by drawing each sprite on a separate layer depth.

            int delayPerItem = 0;

            // TODO: Experiment with hoverTimePerItem using sigmoid curve; each item being stacked gets less and less hover time.
            //       Precalculate two arrays (avoids expensive math for every sprite), one with with all values, and the other with the sum of all prev values at any index:
            //          private static int[] hoverTimesPerItem = new int[20] { 150, 149, 147, 142, 136, 127, ... , 53, 51, 50 };
            //          private static int[] sumHoverTimesPerItem = new int[20] { 150, 299, 446, ... , {sum of all elements} };
            //
            //       Sigmoid formula that seems to be similar enough to the desired behavior:
            //          y = 50 + (100 / (1 + e^(x/2 - 6))
            //
            //
            //       Max hover time @ first item stack.
            //       v
            // 150ms |-------..._____,
            //       |                `-.,
            //       |                    `-,
            //       |                       `--,
            //  50ms |                           ``-----..._________
            //       |__________________________________________^______
            //       0          5         10         15         20
            //                                                  Min hover time @ ~20 item stacks.
            //                                                  At this point, delay is very short between each item being stacked into chest.
            //     
            float CONFIG_animationHoverSpeed = 1.0f; // TODO: Implement in ModConfig: range = [0.5 to 3, 0.1 interval]; 0.5x speed (half speed) up to 3x speed (triple speed).
            int hoverTimePerItem = (int)(150 / CONFIG_animationHoverSpeed);
            int fadeTime = (int)(500 / CONFIG_animationHoverSpeed);

            ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
            var itemTossSprite = new TemporaryAnimatedSprite(itemData.GetTextureName(), itemData.GetSourceRect(), farmerPosition, false, alphaFade: 0f, Color.White)
            {
                delayBeforeAnimationStart = numChestAnimatedItems * delayPerItem,
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
                delayBeforeAnimationStart = itemTossSprite.delayBeforeAnimationStart + (int)itemTossSprite.interval,
                scale = 4f,
                layerDepth = baseLayerDepth - addlayerDepth,
                interval = numChestAnimatedItems * hoverTimePerItem,
            };
            var itemFadeSprite = new TemporaryAnimatedSprite(itemData.GetTextureName(), itemData.GetSourceRect(), chestPosition, false, alphaFade: 0f, Color.White)
            {
                delayBeforeAnimationStart = itemHoverSprite.delayBeforeAnimationStart + (int)itemHoverSprite.interval,
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

            NumAnimatedItems++;
            NumAnimatedItemsByChest[chest]++;

            int totalAnimationTimeMs = itemFadeSprite.delayBeforeAnimationStart + (int)itemFadeSprite.interval;
            QuickStackChestAnimation.SetModData(chest, totalAnimationTimeMs);
        }
    }
}
