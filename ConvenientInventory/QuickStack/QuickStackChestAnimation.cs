using System;
using System.Linq;
using System.Reflection;
using StardewValley;
using StardewValley.Objects;

namespace ConvenientInventory.QuickStack
{
    /// <summary>
    /// Handles the animation of a chest to visually appear "open" when a <see cref="QuickStackAnimation"/> is depositing items into it.
    /// </summary>
    public static class QuickStackChestAnimation
    {
        private record ChestAnimationData(
            DateTimeOffset StartTime,
            DateTimeOffset EndTime,
            DateTimeOffset CurrentTime,
            int ItemAnimationTotalMs);

        private const int FrameIntervalMs = 83; // 83ms ~= 5 frames @ 60fps.

        private static string StartTimeModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/QuickStackAnimation/StartTime";

        private static string ItemAnimationTotalMsModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/QuickStackAnimation/ItemAnimationTotalMs";

        private static readonly FieldInfo chestCurrentLidFrameField = typeof(Chest)
            .GetField("currentLidFrame", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        /// <summary>
        /// Sets the mod data for the provided chest that will be used to animate its quick stack chest animation.
        /// </summary>
        /// <param name="chest">The chest to set mod data for.</param>
        /// <param name="itemAnimationTotalMs">Total time (in ms) of the quick stack item animation.</param>
        public static void SetModData(Chest chest, int itemAnimationTotalMs)
        {
            string startTimeStr = DateTimeOffset.Now.ToString("O");
            string itemAnimationTotalMsStr = itemAnimationTotalMs.ToString();

            chest.modData[StartTimeModDataKey] = startTimeStr;
            chest.modData[ItemAnimationTotalMsModDataKey] = itemAnimationTotalMsStr;
        }

        /// <summary>
        /// Iterates through all chests in each game location and removes any quick stack chest animation mod data.
        /// </summary>
        public static bool CleanupChestAnimationModDataByLocation(GameLocation gameLocation)
        {
            try
            {
                foreach (Chest chest in gameLocation.Objects.Values.OfType<Chest>())
                {
                    chest.modData.Remove(StartTimeModDataKey);
                    chest.modData.Remove(ItemAnimationTotalMsModDataKey);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the provided chest's lid frame depending on its quick stack chest animation data, if any.
        /// Visually, this makes the chest appear "open" without it actually being in use.
        /// </summary>
        /// <param name="chest"></param>
        public static void Animate(Chest chest)
        {
            ChestAnimationData anim = GetChestAnimationData(chest);
            if (anim == null)
            {
                // Animation not enabled, or no animation data found for this chest.
                return;
            }

            if (anim.CurrentTime < anim.StartTime || anim.EndTime < anim.CurrentTime)
            {
                // We are outside of the time interval for this chest animation.
                return;
            }

            int currentAnimationLidFrame = GetCurrentAnimationLidFrame(chest, anim);
            chestCurrentLidFrameField.SetValue(chest, currentAnimationLidFrame);
        }

        /// <summary>
        /// Gets data relevant for animating the provided chest.
        /// </summary>
        /// <param name="chest">The chest to get animation data for.</param>
        /// <returns>The data, if any. Otherwise, null.</returns>
        private static ChestAnimationData GetChestAnimationData(Chest chest)
        {
            if (!ModEntry.Config.IsEnableQuickStack
                || !ModEntry.Config.IsEnableQuickStackAnimation
                || !ModEntry.Config.IsEnableQuickStackChestAnimation)
            {
                return null;
            }

            if (!chest.modData.ContainsKey(StartTimeModDataKey)
                || !chest.modData.ContainsKey(ItemAnimationTotalMsModDataKey))
            {
                return null;
            }

            string startTimeStr = chest.modData[StartTimeModDataKey];
            string itemAnimationTotalMsStr = chest.modData[ItemAnimationTotalMsModDataKey];

            DateTimeOffset startTime = DateTimeOffset.Parse(startTimeStr);
            int itemAnimationTotalMs = int.Parse(itemAnimationTotalMsStr);

            DateTimeOffset endTime = startTime.AddMilliseconds(itemAnimationTotalMs);
            DateTimeOffset currentTime = DateTimeOffset.Now;

            return new ChestAnimationData(startTime, endTime, currentTime, itemAnimationTotalMs);
        }

        /// <summary>
        /// Gets the current frame of animation to use for this chest based on its animation data.
        /// <para/>Timeline style with designated "keyframes":
        /// <code>
        ///       |--:--:--:--:--X------...------X--:--:--:--:--|
        ///       ^  \________/  ^  \_________/  ^  \________/  ^
        ///       |  "keyframes" |  chest stays  |  "keyframes" |
        ///       |              |     open      |              |
        ///  Start chest     End chest       Start chest    End chest
        ///  open anim.      open anim.      close anim.    close anim.
        /// </code></summary>
        private static int GetCurrentAnimationLidFrame(Chest chest, ChestAnimationData anim)
        {
            int elapsedMs = (int)(anim.CurrentTime - anim.StartTime).TotalMilliseconds;

            // Length of keyframesMs = numFrames (open) -> wait (until all items are stacked into chest) -> numFrames (close; reversed) -> closed.
            int startingLidFrame = chest.startingLidFrame.Value;
            int numFrames = chest.getLastLidFrame() - startingLidFrame;
            int[] keyframesMs = new int[2 * numFrames + 1];

            // IDEA: Optimization: Should I store `keyframesMs` as a serialized string in modData, rather than recalculating it each frame?
            //                     That way, every frame would just do a lookup with `elapsedMs` to see where we land in terms of `keyframesMs`.
            //                     Not sure which is more expensive:
            //                      - storing `keyframesMs` in modData as string and parsing it as int[] every frame, or
            //                      - manually recalculate `keyframesMs` every frame (as we are doing here).

            // == Calculate keyframe timings. ==
            // Open animation.
            int i = 0;
            for (; i < numFrames; i++)
            {
                keyframesMs[i] = i * FrameIntervalMs;
            }

            // Close animation.
            int j = numFrames;
            for (; i < 2 * numFrames; i++, j--)
            {
                keyframesMs[i] = anim.ItemAnimationTotalMs - (j * FrameIntervalMs);
            }

            // End of animation; chest is closed.
            keyframesMs[i] = anim.ItemAnimationTotalMs;

            // == Determine which keyframe should be used at the current time. ==
            int currentKeyframeIndex = 0;
            while (currentKeyframeIndex < keyframesMs.Length - 1 && elapsedMs > keyframesMs[currentKeyframeIndex + 1])
            {
                currentKeyframeIndex++;
            }

            // == Map the found keyframe index to the correct chest lid frame. ==
            if (currentKeyframeIndex < numFrames)
            {
                // Open animation, 
                return startingLidFrame + 1 + currentKeyframeIndex;
            }
            else if (currentKeyframeIndex < 2 * numFrames)
            {
                return startingLidFrame + numFrames - (currentKeyframeIndex - numFrames);
            }
            else
            {
                return startingLidFrame;
            }
        }
    }
}
