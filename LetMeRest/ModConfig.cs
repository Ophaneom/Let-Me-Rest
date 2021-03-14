namespace LetMeRest
{
    class ModConfig
    {
        public float Multiplier { get; set; } = 1;
        public bool SittingVerification { get; set; } = true;
        public bool RidingVerification { get; set; } = true;
        public bool StandingVerification { get; set; } = true;
        public bool EnableSecrets { get; set; } = true;
    }
}
