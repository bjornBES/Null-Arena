/*
 * File: RenderingCommand.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerClient.Game.Content;

namespace PlayerClient.Game.Gameplay.Rendering
{
    public enum RenderType
    {
        Billboard,
        Mesh
    }
    public abstract class RenderCommand
    {
        public abstract RenderType Type { get; }
        public Texture2D Texture;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        GameplayContext Context;
        public float Depth => Vector3.Dot(Position - Context.Camera.Position, Context.Camera.Forward);

        public RenderCommand(Texture2D texture, Vector3 position, Quaternion rotation, Vector3 scale, GameplayContext context)
        {
            Texture = texture;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Context = context;
        }
        public RenderCommand(string textureKey, Vector3 position, Quaternion rotation, Vector3 scale, GameplayContext context)
        {
            Texture2D texture = EngineContentManager.Instance.GetContent<Texture2D>(textureKey, ContentType.Texture);
            Texture = texture;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Context = context;
        }
    }
}
