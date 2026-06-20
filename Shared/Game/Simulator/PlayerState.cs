/*
 * File: Entity.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;

namespace Shared.Game.Simulator
{
    public class PlayerState
    {
        public Vector3 Position;
        public Vector3 Velocity;

        // direction player is moving/facing for movement purposes
        public float BodyYaw;

        // direction player is looking/aiming
        public float AimYaw;
        public float AimPitch;

        public bool IsGrounded;

        // 1.0 = normal, 0.3 = slow fall, 0 = no gravity
        public float GravityScale;

        public PlayerState()
        {
            Position = new Vector3(0);
            Velocity = new Vector3(0);
        }

        public PlayerState(PlayerState playerState)
        {
            Position = playerState.Position;
            Velocity = playerState.Velocity;

            BodyYaw = playerState.BodyYaw;

            AimYaw = playerState.AimYaw;
            AimPitch = playerState.AimPitch;

            IsGrounded = playerState.IsGrounded;

            GravityScale = playerState.GravityScale;
        }
    }
}
