/*
 * File: RenderingSystem.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlayerClient.Game.Gameplay.Rendering
{
    public class RenderingSystem
    {
        public GraphicsDevice GraphicsDevice;
        public SpriteBatch SpriteBatch;

        public Camera Camera;

        Effect _billboardEffect;

        Texture2D texture;

        public RenderingSystem(MonoGameType game)
        {
            GraphicsDevice = game.GraphicsDevice;

            Camera = new Camera(GraphicsDevice.Viewport.AspectRatio);
            Camera.Position = new Vector3(0, 0, 5f);

            _billboardEffect = game.Content.Load<Effect>("Shaders/Billboard");
            texture = game.Content.Load<Texture2D>("Texture/sprite_player");
        }

        public void Update(GameTime gameTime)
        {
            Camera.Update();
        }

        public void Render(List<RenderCommand> commands)
        {
            foreach (RenderCommand command in commands)
            {
                DrawBillboard(command.Texture, command.Position, command.Width, command.Height);
            }
        }

        public VertexPositionTexture[] BuildBillboardQuad(Vector3 worldPos, float width, float height)
        {
            Vector3 worldUp = Vector3.Up; // (0,1,0)

            // Camera might be looking straight up/down — guard against degenerate cross
            Vector3 right = Vector3.Cross(worldUp, Camera.Forward);
            if (right.LengthSquared() < 0.0001f)
                right = Vector3.Right; // fallback
            right = Vector3.Normalize(right);

            Vector3 up = worldUp;

            float hw = width * 0.5f;
            float hh = height * 0.5f;

            Vector3 tl = worldPos + (-hw * right) + (hh * up);
            Vector3 tr = worldPos + (hw * right) + (hh * up);
            Vector3 bl = worldPos + (-hw * right) + (-hh * up);
            Vector3 br = worldPos + (hw * right) + (-hh * up);

            // Two triangles (CCW winding)
            return new[]
            {
                new VertexPositionTexture(tl, new Vector2(0, 0)),
                new VertexPositionTexture(tr, new Vector2(1, 0)),
                new VertexPositionTexture(bl, new Vector2(0, 1)),

                new VertexPositionTexture(tr, new Vector2(1, 0)),
                new VertexPositionTexture(br, new Vector2(1, 1)),
                new VertexPositionTexture(bl, new Vector2(0, 1)),
            };
        }
        public void DrawBillboard(Texture2D texture, Vector3 worldPos, float width, float height)
        {
            _billboardEffect.Parameters["World"].SetValue(Matrix.Identity);
            _billboardEffect.Parameters["View"].SetValue(Camera.viewMatrix);
            _billboardEffect.Parameters["Projection"].SetValue(Camera.projectionMatrix);
            _billboardEffect.Parameters["SpriteTexture"].SetValue(texture);
            _billboardEffect.Parameters["Alpha"].SetValue(1);

            VertexPositionTexture[] verts = BuildBillboardQuad(worldPos, width, height);

            // Alpha blending for sprite transparency
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (EffectPass pass in _billboardEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    verts, 0, 2); // 2 triangles
            }
        }

    }
}
