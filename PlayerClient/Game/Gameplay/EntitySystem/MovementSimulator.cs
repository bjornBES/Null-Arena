/*
 * File: MovementSimulator.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 21 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Engine.Game.BVH;
using Engine.Game.Characters;
using Engine.Game.Simulator;
using Shared.Ncode.Packages;

namespace PlayerClient.Game.Gameplay.EntitySystem
{
    public class MovementSimulator : ISimulator
    {
        public static PlayerState Simulate(PlayerState current, Packet input, BVHTree mapBVH, HeroMovementStats stats, float deltaTime)
        {
            throw new NotImplementedException();
        }

        public PlayerStateGeneric Simulate(PlayerStateGeneric current, Packet input, BVHTree mapBVH, HeroMovementStats stats, float deltaTime)
        {
            return MovementSimulator.Simulate((PlayerState)current, input, mapBVH, stats, deltaTime);
        }
    }
}
