/*
 * File: TextureManager.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Assimp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PlayerClient.Game.Content
{
    public record MeshBuffer(VertexBuffer Vertices, IndexBuffer Indices, Vector3[] CollisionTriangles);
    public sealed class MeshManager : EngineContentManagerTemp<MeshBuffer>
    {
        private readonly AssimpContext importer = new AssimpContext();
        private readonly Dictionary<string, Scene> _sceneCache = new();
        private readonly Dictionary<string, (VertexPositionNormalTexture[] vertices, uint[] indices, Vector3[] collTriangles)> _pending = new();

        public MeshManager(ContentManager content, GraphicsDevice graphicsDevice) : base(content, graphicsDevice)
        {
        }

        public override async Task LoadAsync(string key, string assetName)
        {
            string path = Path.Combine("Assets", "Objs", assetName);
            if (!_sceneCache.TryGetValue(path, out Scene scene))
            {
                scene = importer.ImportFile(path, PostProcessSteps.Triangulate);
                _sceneCache[path] = scene;
            }


            Mesh mesh;
            if (scene.Meshes.Count == 1)
            {
                mesh = scene.Meshes[0];
            }
            else
            {
                mesh = scene.Meshes.Find((m) => m.Name == key);
                if (mesh == null)
                {
                    return;
                }
            }

            VertexPositionNormalTexture[] vertices;
            if (mesh.TextureCoordinateChannels.All((v) => v.Count == 0))
            {
                vertices = mesh.Vertices.Select((v, i) => new VertexPositionNormalTexture(
                    new Vector3(v.X, v.Y, v.Z),
                    new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z),
                    Vector2.Zero
                )).ToArray();
            }
            else
            {
                vertices = mesh.Vertices.Select((v, i) => new VertexPositionNormalTexture(
                    new Vector3(v.X, v.Y, v.Z),
                    new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z),
                    new Vector2(mesh.TextureCoordinateChannels[0][i].X,
                                mesh.TextureCoordinateChannels[0][i].Y)
                )).ToArray();
            }

            uint[] indices = mesh.Faces.SelectMany(f => f.Indices).Select(i => (uint)i).ToArray();

            Vector3[] collisionTriangles = mesh.Faces
                .SelectMany(f => f.Indices.Select(i =>
                {
                    Vector3D v = mesh.Vertices[i];
                    return new Vector3(v.X, v.Y, v.Z);
                }))
                .ToArray();

            _pending[mesh.Name] = (vertices, indices, collisionTriangles);
        }

        public override void LoadMainThread(string key, string assetName)
        {
            if (!_pending.ContainsKey(key))
            {
                return;
            }
            (VertexPositionNormalTexture[] vertices, uint[] indices, Vector3[] collTriangles) = _pending[key];

            int indexKey = GetIntKey(key);

            VertexBuffer vb = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vb.SetData(vertices);

            IndexBuffer ib = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            ib.SetData(indices);

            _registry[indexKey] = new MeshBuffer(vb, ib, collTriangles);
            _pending.Remove(key);
        }

        protected override void LoadMainThread(string key)
        {
            int indexKey = GetIntKey(key);
            if (!_keyCache.TryGetValue(indexKey, out string asset))
            {
                throw new KeyNotFoundException($"Mesh key {indexKey} ('{key}') has never been loaded.");
            }

            LoadMainThread(key, asset);
        }

        /// <summary>
        /// Retrieves a cached mesh.
        /// </summary>
        public override MeshBuffer Get(string key)
        {
            int indexKey = GetIntKey(key);
            if (!_registry.TryGetValue(indexKey, out MeshBuffer mesh))
            {
                LoadMainThread(key);
            }

            return mesh;
        }

        public override bool Exists(string key) => InternalExists(key);

        public override void Unload(string key)
        {
            int keyIndex = GetIntKey(key);
            if (_registry.TryGetValue(keyIndex, out MeshBuffer mesh))
            {
                mesh.Deconstruct(out _, out _, out _);
                _registry.Remove(keyIndex);
            }
        }

        public override void UnloadAll()
        {
            foreach (MeshBuffer mesh in _registry.Values)
            {
                mesh.Deconstruct(out _, out _, out _);
            }

            _registry.Clear();
        }
    }

}
