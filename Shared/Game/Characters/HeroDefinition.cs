using System.Text.Json.Serialization;

namespace Shared.Game.Characters
{
    public sealed class HeroDefinition
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string Id { get; init; }
        public float MoveSpeed { get; init; }
        public float MaxHealth { get; init; }
        public string ModelId { get; init; }
        public HeroTextureDefinition[] HeroSkins { get; init; }
        public string[] AbilityIds { get; init; }
        public string IdleAnimId { get; init; }
        public string WalkingAnimId { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [JsonIgnore]
        public uint Checksum { get; private set; }
    }
    public sealed class HeroTextureDefinition
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OverwriteModelId { get; init; }
        public string SkinId { get; init; }
        public string TextureId { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }

    public struct HeroRuntimeState
    {
        public HeroDefinition Base;

        public float Health;
        public float MoveSpeed;

        public AbilityRuntimeState[] Abilities;
        public List<DotRuntimeState> ActiveDots;

        public HeroRuntimeState(HeroDefinition definition)
        {
            Base = definition;
            Health = definition.MaxHealth;
            MoveSpeed = definition.MoveSpeed;

            Abilities = new AbilityRuntimeState[definition.AbilityIds.Length];
            ActiveDots = new List<DotRuntimeState>();
        }
    }
}
