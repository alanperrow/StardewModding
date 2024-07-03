using System;
using System.Reflection;
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

        // IDEA: Timeline style with designated "keyframes"
        //
        //       |--:--:--:--:--X------...------X--:--:--:--:--|
        //       ^  \________/  ^  \_________/  ^  \________/  ^
        //       |  "keyframes" |  chest stays  |  "keyframes" |
        //       |              |     open      |              |
        //  Start chest     End chest       Start chest    End chest
        //  open anim.      open anim.      close anim.    close anim.
        //
        // - Need to define:
        //      - number of intervals for animation in one direction 
        //      - interval between animation "keyframes"
        //      - start time for open animation
        //          - end time not specified; we can calculate the end time for open animation @ the final interval.
        //      - time duration for how long chest stays open while items are visually being stacked into it
        //      - use this^ duration to calculate start time for close animation
        //          - end time not specified; we can calculate the end time for close animation @ the final interval.
        private static int GetCurrentAnimationLidFrame(Chest chest, ChestAnimationData anim)
        {
            int elapsedMs = (int)(anim.CurrentTime - anim.StartTime).TotalMilliseconds;

            int numFrames = chest.getLastLidFrame() - chest.startingLidFrame.Value;
            int[] keyframesMs = new int[2 + 2 * numFrames]; // Start frame -> numFrames -> wait (until all items are stacked into chest) -> numFrames (reversed) -> end frame.
        }
    }
}
