/*
 * File: AbilityDefinition.cs
 * File Created: 12 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 12 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Text.Json.Serialization;

namespace Shared.Game.Characters
{
    public sealed class AbilityDefinition
    {
        public string AbilityId { get; init; }

        // Is the ability on the global cooldown timer too
        public bool OnGlobalCD { get; init; }

        public DamageType AbilityDamageType { get; init; }

        public string AbilityEffectId { get; init; }

        public float Cooldown { get; init; }

        public float Damage { get; init; }
        public float DurationTime { get; init; }
        public float TickRate { get; init; }
        // DamagePerTick = Damage / TickRate

        public int MaxCharges { get; init; }

        public AbilityType Type { get; init; }

        public string AbilityIconId { get; init; }

        public string Keybinding { get; init; }

        public string CastAnimationId { get; init; }
        public string HitAnimationId { get; init; }
        public string IdleOverrideId { get; init; }

        [JsonIgnore]
        public uint Checksum { get; private set; }
    }

    public enum AbilityType
    {
        Primary,
        Secondary,
        Ultimate,
        Passive,
    }

    public enum DamageType
    {
        Instant,  // blunt, slash, bullet — resolved immediately
        Dot,      // poison, fire, bleed — resolved over time
        Burst,    // AoE — if you need it later
    }

    public class AbilityRuntimeState
    {
        public AbilityDefinition Base;

        public float CooldownRemaining;
        public int ChargesRemaining;
        public bool IsActive;          // for sustained/channeled abilities
    }
}
