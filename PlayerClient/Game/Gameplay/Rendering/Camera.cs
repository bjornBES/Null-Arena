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
        private float _yaw = 0f;

        public Vector3 Forward => new Vector3(rotMatrix.M31, rotMatrix.M32, rotMatrix.M33);
        public Vector3 Up => new Vector3(rotMatrix.M21, rotMatrix.M22, rotMatrix.M23);
        public Vector3 Right => new Vector3(rotMatrix.M11, rotMatrix.M12, rotMatrix.M13);

        public float FieldOfView { get; set; } = MathHelper.ToRadians(60f);

        public float NearPlane = 0.1f;
        public float FarPlane = 1000f;

        public Camera(float aspectRatio)
        {
            Position = Vector3.Zero;

            _yaw = MathHelper.Pi;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, NearPlane, FarPlane);

            RebuildRotation();

            Update();
        }

        public void Update()
        {
            // Build forward vector directly from yaw and pitch
            Vector3 forward = new Vector3(
                MathF.Sin(_yaw) * MathF.Cos(_pitch),
                MathF.Sin(_pitch),
                MathF.Cos(_yaw) * MathF.Cos(_pitch)
            );

            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
            Vector3 up = Vector3.Cross(right, forward);

            viewMatrix = Matrix.CreateLookAt(Position, Position + forward, up);
        }
        public void Move(Vector3 delta)
        {
            Position += delta;
        }
        public void MoveForward(float amount)
        {
            Vector3 forward = new Vector3(
                MathF.Sin(_yaw) * MathF.Cos(_pitch),
                MathF.Sin(_pitch),
                MathF.Cos(_yaw) * MathF.Cos(_pitch)
            );
            Position += forward * amount;
        }
        public void MoveRight(float amount)
        {
            Vector3 forward = new Vector3(
                MathF.Sin(_yaw) * MathF.Cos(_pitch),
                MathF.Sin(_pitch),
                MathF.Cos(_yaw) * MathF.Cos(_pitch)
            );
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
            Position += right * amount;
        }

        public void Rotate(Quaternion delta)
        {
            Rotation *= delta;
        }
        public void AddYaw(float radians)
        {
            _yaw += radians;
            RebuildRotation();
        }

        public void AddPitch(float radians)
        {
            float limit = MathHelper.ToRadians(45f);
            _pitch = Math.Clamp(_pitch + radians, -limit, limit);
            RebuildRotation();
        }

        private void RebuildRotation()
        {
            Quaternion qYaw = Quaternion.CreateFromAxisAngle(Vector3.Up, _yaw);
            Vector3 localRight = Vector3.Transform(Vector3.Right, qYaw);
            Quaternion qPitch = Quaternion.CreateFromAxisAngle(localRight, _pitch);
            Quaternion result = Quaternion.Normalize(qYaw * qPitch);

            // Force consistent hemisphere to prevent W flipping
            if (result.W < 0)
            {
                result = new Quaternion(-result.X, -result.Y, -result.Z, -result.W);
            }

            Rotation = result;
        }

        public void RebuildRotation(float yaw, float pitch)
        {
            _yaw = yaw;
            _pitch = pitch;
            Quaternion qYaw = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);
            Vector3 localRight = Vector3.Transform(Vector3.Right, qYaw);
            Quaternion qPitch = Quaternion.CreateFromAxisAngle(localRight, pitch);
            Quaternion result = Quaternion.Normalize(qYaw * qPitch);

            // Force consistent hemisphere to prevent W flipping
            if (result.W < 0)
            {
                result = new Quaternion(-result.X, -result.Y, -result.Z, -result.W);
            }

            Rotation = result;
        }
    }
}
