/*
 * File: Entity.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 12 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using Shared.Game.Characters;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlayerClient.Game.Gameplay.EntitySystem
{
    public class Entity
    {
        public HeroRuntimeState RuntimeState;
        public HeroTextureDefinition BaseSkin => RuntimeState.Base.HeroSkins[0];
        public Vector3 Position;

        public Entity(Vector3 position, string heroID)
        {
            string basePath = Path.Combine("Assets", "Heros");
            string path = Path.Combine(basePath, heroID + ".json");
            string json = File.ReadAllText(path);
            uint sum = HeroChecksumCheck.ComputeChecksum(json);
            if (!HeroChecksumCheck.Check(heroID, sum))
            {
                Debug.WriteLine($"File {path} has been changed");
#if DEBUG
                Debug.WriteLine($"given sum is {sum}");
                throw new InvalidDataException($"Checksum mismatch for {path} got {sum}");
#endif
                throw new InvalidDataException($"Checksum mismatch for {path}");
            }

            JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = false
            };
            serializerOptions.Converters.Add(new JsonStringEnumConverter());

            HeroDefinition definition = JsonSerializer.Deserialize<HeroDefinition>(json, serializerOptions);

            List<AbilityDefinition> abilityDefinitions = new List<AbilityDefinition>();
            foreach (string abilityId in definition.AbilityIds)
            {
                string abilityPath = Path.Combine(basePath, "Abilities", abilityId + ".json");
                json = File.ReadAllText(abilityPath);
                sum = HeroChecksumCheck.ComputeChecksum(json);
                if (!AbilityChecksumCheck.Check(abilityId, sum))
                {
                    Debug.WriteLine($"File {path} has been changed");
#if DEBUG
                    Debug.WriteLine($"given sum is {sum}");
                    throw new InvalidDataException($"Checksum mismatch for {path} got {sum}");
#endif
                    throw new InvalidDataException($"Checksum mismatch for {path}");
                }
                abilityDefinitions.Add(JsonSerializer.Deserialize<AbilityDefinition>(json, serializerOptions));
            }
            RuntimeState = new HeroRuntimeState(definition);

            Position = position;
        }
    }
}
