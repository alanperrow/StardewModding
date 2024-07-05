using System;
using System.Reflection;
using StardewValley;
using StardewValley.Objects;

namespace ConvenientInventory.QuickStack
{
    public static class QuickStackChestAnimation
    {
        private record ChestAnimationData(
            DateTimeOffset StartTime,
            DateTimeOffset EndTime,
            DateTimeOffset CurrentTime,
            int ItemAnimationTotalMs);

        private const int FrameIntervalMs = 83; // 83ms @ 60fps ~= 5 frames.

        // Actually don't need this; can be calculated: numIntervals = (LastFrame - StartFrame)
        //public static string NumIntervalsModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/QuickStackAnimation/NumIntervals";

        private static string StartTimeModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/QuickStackAnimation/StartTime";

        private static string ItemAnimationTotalMsModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/QuickStackAnimation/ItemAnimationTotalMs";

        // We'll just make this simpler and set the interval to a constant value (see above: FrameIntervalMs).
        //private static string FrameIntervalMsModDataKey { get; } = $"{ModEntry.Instance.ModManifest.UniqueID}/QuickStackAnimation/FrameIntervalMs";

        private static readonly FieldInfo chestCurrentLidFrameField = typeof(Chest)
            .GetField("currentLidFrame", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        /// <summary>
        /// Iterates through all chests in each game location and removes any quick stack chest animation mod data.
        /// </summary>
        // TODO: Call this method before save game occurs so we don't save mod data unnecessarily.
        public static void CleanupModData()
        {
            Utility.ForEachLocation(loc =>
            {
                foreach (var obj in loc.Objects.Values)
                {
                    if (obj is not Chest chest)
                    {
                        continue;
                    }

                    // If this works, great.
                    // If not, will have to adjust logic below to a TryGetValue() first to ensure the key exists before removing it for each chest.
                    chest.modData.Remove("What happens if i remove a key that DNE?");

                    chest.modData.Remove(StartTimeModDataKey);
                    chest.modData.Remove(ItemAnimationTotalMsModDataKey);
                }

                return true;
            });
        }

        /// <summary>
        /// Sets the provided chest's lid frame depending on its quick stack chest animation data, if any.
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

            // TODO: In ModEntry, on game start & on game end, do a cleanup of these modData values for all chests.
            //       We don't need these modData values to persist in the game save, we are just using them for the convenient multiplayer-sync.
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

        // Timeline style with designated "keyframes"
        //
        //       |--:--:--:--:--X------...------X--:--:--:--:--|
        //       ^  \________/  ^  \_________/  ^  \________/  ^
        //       |  "keyframes" |  chest stays  |  "keyframes" |
        //       |              |     open      |              |
        //  Start chest     End chest       Start chest    End chest
        //  open anim.      open anim.      close anim.    close anim.
        private static int GetCurrentAnimationLidFrame(Chest chest, ChestAnimationData anim)
        {
            int elapsedMs = (int)(anim.CurrentTime - anim.StartTime).TotalMilliseconds;

            int startingLidFrame = chest.startingLidFrame.Value;
            int numFrames = chest.getLastLidFrame() - startingLidFrame;
            int[] keyframesMs = new int[2 * numFrames + 1]; // numFrames (open) -> wait (until all items are stacked into chest) -> numFrames (close; reversed) -> closed.

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
            int j = 0;
            for (; i < 2 * numFrames; i++, j++)
            {
                keyframesMs[i] = anim.ItemAnimationTotalMs + (j * FrameIntervalMs);
            }

            // End of animation; chest is closed.
            keyframesMs[i] = anim.ItemAnimationTotalMs + (j * FrameIntervalMs);

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
                return startingLidFrame + 1 + numFrames - currentKeyframeIndex;
            }
            else
            {
                return startingLidFrame;
            }
        }
    }
}
