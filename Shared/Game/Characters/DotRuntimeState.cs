namespace Shared.Game.Characters
{
    public struct DotRuntimeState
    {
        public AbilityDefinition Base;

        public float DamagePerTick;
        public float TickRate;
        public float NextTickAt;
        public float ExpiresAt;
    }
}
