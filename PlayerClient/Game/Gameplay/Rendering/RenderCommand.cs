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
        Billboard
    }
    public class RenderCommand
    {
        public RenderType Type;
        public Texture2D Texture;
        public Vector3 Position;
        public float Width;
        public float Height;
        public float Depth;

        public RenderCommand(RenderType renderType, Texture2D texture, Vector3 position, float width, float height, GameplayContext context)
        {
            Type = renderType;
            Texture = texture;
            Position = position;
            Width = width;
            Height = height;
            Depth = Vector3.Dot(position - context.Camera.Position, context.Camera.Forward);
        }
        public RenderCommand(RenderType renderType, string textureKey, Vector3 position, float width, float height, GameplayContext context)
        {
            Texture2D texture = EngineContentManager.Instance.GetContent<Texture2D>(textureKey, ContentType.Texture);
            Type = renderType;
            Texture = texture;
            Position = position;
            Width = width;
            Height = height;
            Depth = Vector3.Dot(position - context.Camera.Position, context.Camera.Forward);
        }
    }
}