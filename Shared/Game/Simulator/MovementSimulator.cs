using Microsoft.Xna.Framework;
using Shared.Game.BVH;
using Shared.Game.Characters;
using Shared.Game.InputSystem;
using Shared.Network.Package;

namespace Shared.Game.Simulator
{
    public static class MovementSimulator
    {
        public static PlayerState Simulate(PlayerState current, InputPackage input, BVHTree mapBVH, HeroMovementStats stats, float deltaTime)
        {
            PlayerState newState = new PlayerState(current);

            newState.AimYaw = MathHelper.ToRadians(input.AimX) / 2;
            newState.AimPitch = MathHelper.ToRadians(input.AimY) / 2;

            Vector3 objSpaceMovement = input.RawMovement;

            // Convert object-space input into world-space
            Vector3 forward = new Vector3(MathF.Sin(newState.BodyYaw), 0, MathF.Cos(newState.BodyYaw)); // or however your yaw convention maps to axes
            Vector3 right = new Vector3(forward.Z, 0, -forward.X); // perpendicular on XZ plane

            Vector3 movementVector = (forward * objSpaceMovement.Z) + (right * objSpaceMovement.X);
            Vector3 worldMoveDir = Vector3.Normalize(movementVector);
            if (movementVector.Length() == 0)
            {
                worldMoveDir = new Vector3(0);
            }
            else
            {

            }

            // Calculate new velocity
            Vector3 desiredVelocity = worldMoveDir * stats.MaxSpeed;

            // accelerate toward desired velocity rather than snapping instantly
            Vector3 newVelocity = Vector3.Lerp(current.Velocity, desiredVelocity, stats.Acceleration * deltaTime);

            // clamp horizontal speed only, let gravity/vertical velocity be separate
            Vector3 horizontalVel = new Vector3(newVelocity.X, 0, newVelocity.Z);
            if (horizontalVel.Length() > stats.MaxSpeed)
            {
                horizontalVel = Vector3.Normalize(horizontalVel) * stats.MaxSpeed;
            }

            float speed = horizontalVel.Length();
            if (speed > 0.0001f)
            {
                float drop = stats.Friction * deltaTime;
                float newSpeed = MathF.Max(0f, speed - drop);
                horizontalVel *= (newSpeed / speed);
            }
            newVelocity.X = horizontalVel.X;
            newVelocity.Z = horizontalVel.Z;

            // Simulate gravity
            newVelocity.Y -= stats.Gravity * current.GravityScale * deltaTime;
            Vector3 verticalVel = new Vector3(0, newVelocity.Y, 0);
            if (verticalVel.Length() > stats.MaxSpeed)
            {
                verticalVel = Vector3.Normalize(verticalVel) * stats.MaxSpeed;
            }
            newVelocity.Y = verticalVel.Y;

            if (input.Buttons.HasFlag(InputActionFlags.Jump) && current.IsGrounded)
            {
                newVelocity.Y = stats.JumpForce;
            }

            newState.Velocity = newVelocity;

            Vector3 desiredMove = newState.Velocity * deltaTime;
            Vector3 resolvedMove = ResolveCollision(current.Position, desiredMove, mapBVH, stats.PlayerRadius);

            newState.Position = current.Position + resolvedMove;
            newState.IsGrounded = CheckGrounded(newState.Position, mapBVH, stats.PlayerRadius);

            if (newState.IsGrounded)
            {
                newState.Velocity.Y = 0;
            }

            return newState;
        }

        public static bool CheckGrounded(Vector3 position, BVHTree mapBVH, float radius, float groundCheckDistance = 0.1f)
        {
            Vector3 moveDir = Vector3.Down;
            float moveDist = groundCheckDistance;

            return BVHTraversal.SphereSweepTest(mapBVH, position, radius, moveDir, moveDist, out float hitDist, out Vector3 hitNormal) && hitNormal.Y > 0.5f; // only count it as "ground" if surface faces mostly upward, not a wall
        }

        public static Vector3 ResolveCollision(Vector3 position, Vector3 desiredMove, BVHTree mapBVH, float radius)
        {
            Vector3 remainingMove = desiredMove;
            Vector3 totalMove = Vector3.Zero;
            const int maxIterations = 4;

            for (int i = 0; i < maxIterations; i++)
            {
                float moveDist = remainingMove.Length();
                if (moveDist < 1e-5f)
                {
                    break;
                }

                Vector3 moveDir = remainingMove / moveDist;
                Vector3 currentPos = position + totalMove;

                if (BVHTraversal.SphereSweepTest(mapBVH, currentPos, radius, moveDir, moveDist, out float hitDist, out Vector3 hitNormal))
                {
                    Vector3 safeMove = moveDir * MathF.Max(0, hitDist - 0.01f);
                    totalMove += safeMove;

                    float remaining = moveDist - hitDist;
                    Vector3 remainingDir = moveDir * remaining;
                    remainingMove = remainingDir - hitNormal * Vector3.Dot(remainingDir, hitNormal);
                }
                else
                {
                    totalMove += remainingMove;
                    remainingMove = Vector3.Zero;
                    break;
                }
            }

            return totalMove;
        }
    }
}
