/*
 * File: Camera.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;

namespace PlayerClient.Game.Gameplay.Rendering
{
    public class Camera
    {
        public Vector3 Position;

        public Matrix projectionMatrix;
        public Matrix viewMatrix;
        public Matrix worldMatrix;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        Matrix rotMatrix => Matrix.CreateFromQuaternion(Rotation);
        private float _pitch = 0f;

        public Vector3 Forward => new Vector3(rotMatrix.M13, rotMatrix.M23, rotMatrix.M33);
        public Vector3 Up => new Vector3(rotMatrix.M12, rotMatrix.M22, rotMatrix.M32);
        public Vector3 Right => new Vector3(rotMatrix.M11, rotMatrix.M21, rotMatrix.M31);

        public float FieldOfView { get; set; } = MathHelper.ToRadians(60f);

        public float NearPlane = 1f;
        public float FarPlane = 1000f;

        public Camera(float aspectRatio)
        {
            Position = Vector3.Zero;

            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, NearPlane, FarPlane);

            Vector3 Target = Position + Forward;
            viewMatrix = Matrix.CreateLookAt(Position, Target, Up);
        }

        public void Update()
        {
            /*
             if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Position.X -= 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Position.X += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Position.Y -= 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Position.Y += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                Position.Z += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                Position.Z -= 1f;
            }
             */

            Vector3 Target = Position + Forward;
            viewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);
        }
        public void Move(Vector3 delta)
        {
            Position += delta;
        }
        public void MoveForward(float amount)
        {
            Position += Forward * amount;
        }
        public void MoveRight(float amount)
        {
            Position += Right * amount;
        }

        public void Rotate(Quaternion delta)
        {
            Rotation *= delta;
        }
        public void AddYaw(float radians)
        {
            Quaternion delta = Quaternion.CreateFromAxisAngle(Vector3.Up, radians);
            Rotation = Quaternion.Normalize(delta * Rotation); // world-space yaw
        }

        public void AddPitch(float radians)
        {
            float limit = MathHelper.ToRadians(45f);
            _pitch = Math.Clamp(_pitch + radians, -limit, limit);

            // Rebuild pitch component from scratch to avoid drift
            Quaternion yawOnly = Quaternion.CreateFromAxisAngle(Vector3.Up, ExtractYaw(Rotation));
            Quaternion pitchQ = Quaternion.CreateFromAxisAngle(Vector3.Right, _pitch);
            Rotation = Quaternion.Normalize(yawOnly * pitchQ);
        }

        private static float ExtractYaw(Quaternion q)
        {
            return MathF.Atan2(
                2f * (q.W * q.Y + q.X * q.Z),
                1f - 2f * (q.Y * q.Y + q.X * q.X)
            );
        }
    }
}
