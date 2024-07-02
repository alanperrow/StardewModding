using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace ConvenientInventory.QuickStack
{
    public class QuickStackAnimation
    {
        private int numItemsAnimated;

        public void Begin(int numItemsAlreadyAnimated)
        {
            numItemsAnimated = numItemsAlreadyAnimated;
        }

        public int End()
        {
            return numItemsAnimated;
        }

        public void Method(Chest chest, Farmer who)
        {
            // Restart the quick stack animation stopwatch so that chests can be visually opened during animation.
            ConvenientInventory.QuickStackAnimationStopwatch.Restart();

            // TODO: Junimo Hut item stack is being tossed toward top-left of current location, near (0,0). Investigate and fix.

            int numChestItemStacks = new Random().Next(8) + 1;
            for (int i = 0; i < numChestItemStacks; i++)
            {
                Vector2 chestPosition = (chest.TileLocation + new Vector2(0, -1.5f)) * Game1.tileSize;
                Vector2 farmerOffset = who.FacingDirection switch
                {
                    0 => new Vector2(0f, -1.5f) * Game1.tileSize,   // Up
                    1 => new Vector2(0.5f, -1f) * Game1.tileSize,   // Right
                    3 => new Vector2(-0.5f, -1f) * Game1.tileSize,  // Left
                    _ => new Vector2(0f, -0.5f) * Game1.tileSize,   // Down
                };
                Vector2 farmerPosition = who.Position + farmerOffset;

                float distance = Vector2.Distance(farmerPosition, chestPosition);
                Vector2 motionVec = (chestPosition - farmerPosition) * 0.98f; // 0.98 multiplier gives a better result in-game; without it, items slightly overshoot the chest.

                float CONFIG_animationSpeed = 1.0f; // TODO: Implement in ModConfig: range = [0.5 to 3, 0.1 interval]; 0.5x speed (half speed) up to 3x speed (triple speed).
                float time = (float)(10 * Math.Pow(distance, 0.5)) + 400 - 0.5f * Math.Min(0, motionVec.Y);
                time /= CONFIG_animationSpeed;

                int randExtraHeight = 60;// new Random().Next(160);
                float extraHeight = 128 + randExtraHeight - Math.Min(0, motionVec.Y);
                float gravity = 2 * extraHeight / time;
                motionVec.Y -= extraHeight;
                float extraX = motionVec.X;
                float xAccel = -2 * motionVec.X / time;
                motionVec.X += extraX;

                float baseLayerDepth = (float)((chest.TileLocation.Y + 1) * 64) / 10000f + chest.TileLocation.X / 50000f; // Refactored from Object.draw()
                float addlayerDepth = 1E-06f * numItemsAnimated; // Avoids z-fighting by drawing each sprite on a separate layer depth.

                int delayPerItem = 0;

                float CONFIG_animationHoverSpeed = 1.0f; // TODO: Implement in ModConfig: range = [0.5 to 3, 0.1 interval]; 0.5x speed (half speed) up to 3x speed (triple speed).
                int hoverTimePerItem = 150;
                hoverTimePerItem = (int)(hoverTimePerItem / CONFIG_animationHoverSpeed);

                ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem("(O)" + new Random().Next(100)); // DEBUG: Get random item sprite.
                                                                                                           //ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem(playerItem.QualifiedItemId);
                var itemTossSprite = new TemporaryAnimatedSprite(itemData.GetTextureName(), itemData.GetSourceRect(), farmerPosition, false, alphaFade: 0f, Color.White)
                {
                    delayBeforeAnimationStart = i * delayPerItem,
                    scale = 4f,
                    layerDepth = 1f - addlayerDepth,
                    totalNumberOfLoops = 0,
                    interval = time,
                    motion = motionVec / time,
                    acceleration = new Vector2(xAccel, gravity) / time,
                    timeBasedMotion = true,
                };
                var itemHoverSprite = new TemporaryAnimatedSprite(itemData.GetTextureName(), itemData.GetSourceRect(), chestPosition, false, alphaFade: 0f, Color.White)
                {
                    delayBeforeAnimationStart = i * delayPerItem + (int)time,
                    scale = 4f,
                    layerDepth = baseLayerDepth - addlayerDepth,
                    interval = i * hoverTimePerItem,
                };
                var itemFadeSprite = new TemporaryAnimatedSprite(itemData.GetTextureName(), itemData.GetSourceRect(), chestPosition, false, alphaFade: 0f, Color.White)
                {
                    delayBeforeAnimationStart = i * delayPerItem + (int)time + i * hoverTimePerItem,
                    scale = 4f,
                    layerDepth = baseLayerDepth + addlayerDepth,
                    alphaFade = 0.04f * CONFIG_animationHoverSpeed,
                    interval = 500 * CONFIG_animationHoverSpeed,
                    motion = new Vector2(0.6f, 4.5f) * CONFIG_animationHoverSpeed,
                    acceleration = new Vector2(0f, -0.08f) * CONFIG_animationHoverSpeed,
                    scaleChange = -0.07f * CONFIG_animationHoverSpeed,
                };

                ConvenientInventory.AddQuickStackAnimationItemSprite(itemTossSprite);
                ConvenientInventory.AddQuickStackAnimationItemSprite(itemHoverSprite);
                ConvenientInventory.AddQuickStackAnimationItemSprite(itemFadeSprite);
                numItemsAnimated++;

                // Here we use 500ms as an estimate for how long fade animation takes to complete.
                int totalAnimationTimeMs = (int)(i * delayPerItem + (int)time + i * hoverTimePerItem + 500 / CONFIG_animationHoverSpeed);

                // Add total animation time to chest's mod data.
                // This is used to override the draw animation to display the chest as "opened", even though it really isn't.
                if (!chest.modData.TryAdd(ConvenientInventory.quickStackAnimationChestOpenMsModDataKey, totalAnimationTimeMs.ToString()))
                {
                    // Could not add mod data; Overwrite existing mod data entry, if any.
                    if (chest.modData.ContainsKey(ConvenientInventory.quickStackAnimationChestOpenMsModDataKey))
                    {
                        chest.modData[ConvenientInventory.quickStackAnimationChestOpenMsModDataKey] = totalAnimationTimeMs.ToString();
                    }
                }
            }
        }
    }
}
