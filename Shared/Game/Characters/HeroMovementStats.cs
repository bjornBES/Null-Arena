namespace Shared.Game.Characters
{
    public sealed class HeroMovementStats
    {
        public float Acceleration { get; init; }
        public float MaxSpeed { get; init; }
        public float JumpForce { get; init; }
        public float Gravity { get; init; }
        public float PlayerRadius { get; init; }
        public float Friction { get; init; }
    }
}
