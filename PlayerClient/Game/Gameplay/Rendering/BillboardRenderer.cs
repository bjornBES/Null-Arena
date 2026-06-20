using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlayerClient.Game.Gameplay.Rendering
{
    public class BillboardRenderer : RenderingModule
    {
        Effect billboardEffect;

        public BillboardRenderer(Game1 game, Camera camera) : base(game, camera)
        {
            billboardEffect = game.Content.Load<Effect>("Shaders/Billboard");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Render(RenderCommand command)
        {
            DrawBillboard(command.Texture, command.Position, command.Scale.X, command.Scale.Y);
        }

        public VertexPositionTexture[] BuildBillboardQuad(Vector3 worldPos, float width, float height)
        {
            Vector3 worldUp = Camera.Up; // (0,1,0)

            // Camera might be looking straight up/down — guard against degenerate cross
            Vector3 right = Vector3.Cross(worldUp, Camera.Forward);
            if (right.LengthSquared() < 0.0001f)
            {
                right = Vector3.Right; // fallback
            }

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
            billboardEffect.Parameters["World"].SetValue(Matrix.Identity);
            billboardEffect.Parameters["View"].SetValue(Camera.viewMatrix);
            billboardEffect.Parameters["Projection"].SetValue(Camera.projectionMatrix);
            billboardEffect.Parameters["SpriteTexture"].SetValue(texture);
            billboardEffect.Parameters["Alpha"].SetValue(1);

            VertexPositionTexture[] verts = BuildBillboardQuad(worldPos, width, height);

            // Alpha blending for sprite transparency
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (EffectPass pass in billboardEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    verts, 0, 2); // 2 triangles
            }
        }
    }
}
