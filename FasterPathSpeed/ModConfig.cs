namespace FasterPathSpeed
{
    public class ModConfig
    {
        public float DefaultPathSpeedBuff { get; set; } = 1f;   // Original: 0.1f

        public bool IsPathSpeedBuffOnlyOnTheFarm { get; set; } = false;

        public bool IsPathAffectHorseSpeed { get; set; } = true;

        public float HorsePathSpeedBuffModifier { get; set; } = 1f;

        public bool IsEnablePathReplace { get; set; } = true;

        public bool IsUseCustomPathSpeedBuffValues { get; set; } = false;

        public CustomPathSpeedBuffValues CustomPathSpeedBuffValues { get; set; } = new();
    }
}
