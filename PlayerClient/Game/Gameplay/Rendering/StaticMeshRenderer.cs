using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerClient.Game.Gameplay.MapSystem;

namespace PlayerClient.Game.Gameplay.Rendering
{
    public class StaticMeshRenderer : RenderingModule
    {
        Effect staticMeshEffect;

        public StaticMeshRenderer(Game1 game, Camera camera) : base(game, camera)
        {
            staticMeshEffect = game.Content.Load<Effect>("Shaders/StaticMesh");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Render(RenderCommand command)
        {
            MeshRenderCommand cmd = command as MeshRenderCommand;

            Matrix model = Matrix.CreateScale(cmd.Scale)
                         * Matrix.CreateFromQuaternion(cmd.Rotation)
                         * Matrix.CreateTranslation(cmd.Position);

            staticMeshEffect.Parameters["World"].SetValue(model);
            staticMeshEffect.Parameters["View"].SetValue(Camera.viewMatrix);
            staticMeshEffect.Parameters["Projection"].SetValue(Camera.projectionMatrix);
            staticMeshEffect.Parameters["SpriteTexture"].SetValue(command.Texture);


            LoadedMap map = GameplayManager.MapManager.GetLoadedMap();
            staticMeshEffect.Parameters["AmbientColor"].SetValue(map.AmbientLight.Color.ToVector3());
            staticMeshEffect.Parameters["AmbientIntensity"].SetValue(map.AmbientLight.Intensity);

            PointLight[] lights = map.Lights;
            staticMeshEffect.Parameters["LightPositions"].SetValue(lights.Select(l => l.Position).ToArray());
            staticMeshEffect.Parameters["LightRadii"].SetValue(lights.Select(l => l.Radius).ToArray());
            staticMeshEffect.Parameters["LightColors"].SetValue(lights.Select(l => l.Color.ToVector3()).ToArray());
            staticMeshEffect.Parameters["LightIntensities"].SetValue(lights.Select(l => l.Intensity).ToArray());
            staticMeshEffect.Parameters["LightCount"].SetValue(lights.Length);

            GraphicsDevice.SetVertexBuffer(cmd.Mesh.Vertices);
            GraphicsDevice.Indices = cmd.Mesh.Indices;

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            foreach (EffectPass pass in staticMeshEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0, 0,
                    cmd.Mesh.Indices.IndexCount / 3);
            }
        }
    }
}
