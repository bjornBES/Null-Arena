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
using PlayerClient.Game.Gameplay.MapSystem;
using Shared.Game.BVH;

namespace PlayerClient.Game.Gameplay.Rendering
{
    public abstract class RenderingModule
    {
        public GameplayManager GameplayManager;
        public GraphicsDevice GraphicsDevice;
        public SpriteBatch SpriteBatch;
        public Camera Camera;
        public RenderingModule(Game1 game, Camera camera)
        {
            GraphicsDevice = game.GraphicsDevice;
            GameplayManager = game.GameplayManager;

            Camera = camera;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Render(RenderCommand command);
    }
    public class RenderingSystem
    {
        public GraphicsDevice GraphicsDevice;
        public SpriteBatch SpriteBatch;

        public Camera Camera;

        BillboardRenderer billboardRenderer;
        StaticMeshRenderer staticMeshRenderer;

        MapManager mapManager;
        BasicEffect basic;

        public RenderingSystem(Game1 game)
        {
            mapManager = game.GameplayManager.MapManager;
            GraphicsDevice = game.GraphicsDevice;
            basic = new BasicEffect(GraphicsDevice);

            Camera = new Camera(GraphicsDevice.Viewport.AspectRatio);
            Camera.Position = new Vector3(0, 0, 5f);
            billboardRenderer = new BillboardRenderer(game, Camera);
            staticMeshRenderer = new StaticMeshRenderer(game, Camera);
        }

        public void Update(GameTime gameTime)
        {
            Camera.Update();
        }

        public void Render(List<RenderCommand> commands)
        {
            foreach (RenderCommand command in commands)
            {
                switch (command.Type)
                {
                    case RenderType.Billboard:
                        billboardRenderer.Render(command);
                        break;
                    case RenderType.Mesh:
                        staticMeshRenderer.Render(command);
                        break;
                    default:
                        break;
                }
            }

            basic.View = Camera.viewMatrix;
            basic.Projection = Camera.projectionMatrix;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            DepthStencilState depthStencilState = GraphicsDevice.DepthStencilState;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            DrawBVHWireframe(mapManager.GetLoadedMap().BVH, basic, GraphicsDevice);
            GraphicsDevice.DepthStencilState = depthStencilState;
        }

        public void DrawBVHWireframe(BVHTree tree, BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            List<VertexPositionColor> lines = new List<VertexPositionColor>();

            foreach (BVHNode node in tree.Nodes)
            {
                if (!node.IsLeaf)
                {
                    continue; // only draw leaves, comment out to see whole hierarchy
                }

                AddBoxLines(lines, node.AABBMin, node.AABBMax, Color.Green);
            }

            if (lines.Count == 0)
            {
                return;
            }

            effect.Alpha = 1.0f;
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;
            effect.World = Matrix.Identity;
            effect.LightingEnabled = false;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                    graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, lines.ToArray(), 0, lines.Count / 2);
                /*
                 */
            }
        }

        private void AddBoxLines(List<VertexPositionColor> lines, Vector3 min, Vector3 max, Color color)
        {
            Vector3[] corners = new Vector3[8]
            {
                new Vector3(min.X, min.Y, min.Z),
                new Vector3(max.X, min.Y, min.Z),
                new Vector3(max.X, min.Y, max.Z),
                new Vector3(min.X, min.Y, max.Z),
                new Vector3(min.X, max.Y, min.Z),
                new Vector3(max.X, max.Y, min.Z),
                new Vector3(max.X, max.Y, max.Z),
                new Vector3(min.X, max.Y, max.Z),
            };

            int[,] edges = new int[12, 2]
            {
                {0,1},{1,2},{2,3},{3,0}, // bottom
                {4,5},{5,6},{6,7},{7,4}, // top
                {0,4},{1,5},{2,6},{3,7}, // verticals
            };

            for (int i = 0; i < 12; i++)
            {
                lines.Add(new VertexPositionColor(corners[edges[i, 0]], color));
                lines.Add(new VertexPositionColor(corners[edges[i, 1]], color));
            }
        }
    }
}
