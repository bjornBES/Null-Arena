using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerClient.Game.Content;
using PlayerClient.Game.Gameplay.NetworkSystem;
using PlayerClient.Game.Gameplay.Rendering;
using Shared.Game.BVH;
using Shared.Game.Maps;
using Shared.Network.Package.matchs;

namespace PlayerClient.Game.Gameplay.MapSystem
{
    public class PointLight
    {
        public Vector3 Position;
        public float Radius;
        public Color Color;
        public float Intensity;
    };
    public class LoadedMap
    {
        public PlacedMesh[] StaticMeshes;
        public PointLight AmbientLight;
        public PointLight[] Lights;
        public BVHTree BVH;
        public Map Map;
    }
    public class MapManager : ManagerClass
    {
        LoadedMap loadedMap;
        public override void Initialize(GameplayContext context)
        {
            loadedMap = new LoadedMap();

            PacketDespatcher.OnMatchStartPackages += GotMatchMap;
        }
        public override void LoadContent(GameplayContext context, EngineContentManager contentManager)
        {
        }
        public override void StartMatch(GameplayContext context)
        {
        }
        public override void Update(GameplayContext context, GameTime gameTime)
        {
        }
        public override void PostPhysicsUpdate(GameplayContext context, GameTime gameTime)
        {
        }
        public override void Draw(GameplayContext context, GameTime gameTime)
        {
            if (loadedMap.StaticMeshes == null || loadedMap.StaticMeshes.Length == 0)
            {
                return;
            }

            foreach (PlacedMesh p in loadedMap.StaticMeshes)
            {
                context.RenderCommands.Add(new MeshRenderCommand(p.Mesh, p.Texture, p.Position, p.Rotation, p.Scale, context));
            }

            // MeshBuffer debugMesh = EngineContentManager.Instance.GetContent<MeshBuffer>("dragon", ContentType.Mesh);
            // context.RenderCommands.Add(new MeshRenderCommand(debugMesh, "tex_stanford_dragon", Vector3.Zero, Quaternion.Identity, new Vector3(5, 5, 5), context));
        }
        public override void Unload(GameplayContext context)
        {
            loadedMap = new LoadedMap();
        }

        public LoadedMap GetLoadedMap() => loadedMap;

        async Task loadMapAssetsAsync<T>(string id, string asset, ContentType contentType) where T : class
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string logs = $"start loading {id} at {stopwatch.ElapsedMilliseconds}\n";
            await EngineContentManager.Instance.AddContentAsync<T>(id, asset, contentType);
            logs += $"done loading {id} at {stopwatch.ElapsedMilliseconds}\n";
            stopwatch.Stop();
            Debug.WriteLine(logs);
        }
        void loadMapAssetsMain<T>(string id, string asset, ContentType contentType) where T : class
        {
            EngineContentManager.Instance.AddContent<T>(id, asset, contentType);
        }
        T getMapAssetsMain<T>(string id, ContentType contentType) where T : class
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return EngineContentManager.Instance.GetContent<T>(id, contentType);
        }
        async Task<T> getMapAssetsAsync<T>(string id, ContentType contentType) where T : class
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return await EngineContentManager.Instance.GetContentAsync<T>(id, contentType);
        }
        public async Task<string> BuildMapCollision(List<BVHTriangle> allTriangles)
        {
            loadedMap.BVH = await Task.Run(() => BVHBuilder.Build(allTriangles.ToArray()));
            return "";
        }
        private async void GotMatchMap(GetMatchPackages mapPackage)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string logs = "";
            string mapId = new string(mapPackage.MapId, 0, mapPackage.MapIdLength);
            logs += $"Getting map at {stopwatch.ElapsedMilliseconds}\n";
            Map map = MapRegistry.GetMap(mapId);
            logs += $"Done getting map at {stopwatch.ElapsedMilliseconds}\n";
            string path = Path.Combine("Maps", mapId + ".obj");

            loadedMap.Map = map;

            logs += $"textures and meshes starting at {stopwatch.ElapsedMilliseconds}\n";

            // Kick off all unique mesh loads in parallel
            Dictionary<string, Task> meshTasks = map.Objects
                .Select(o => o.Mesh).Distinct()
                .ToDictionary(key => key, key => loadMapAssetsAsync<MeshBuffer>(key, path, ContentType.Mesh));

            // Kick off all unique mesh loads in parallel
            Dictionary<string, Task> textureTasks = map.Objects
                .Select(o => o.Texture).Distinct()
                .ToDictionary(key => key, key => loadMapAssetsAsync<Texture2D>(key, $"Texture/Map/{mapId}/{key}", ContentType.Texture));

            // Await all
            await Task.WhenAll(meshTasks.Values);
            logs += $"mesh async done at {stopwatch.ElapsedMilliseconds}\n";

            // Populate registry
            foreach ((string key, Task task) in meshTasks)
            {
                loadMapAssetsMain<MeshBuffer>(key, path, ContentType.Mesh);
            }
            logs += $"mesh main thread done at {stopwatch.ElapsedMilliseconds}\n";

            await Task.WhenAll(textureTasks.Values);
            logs += $"textures async done at {stopwatch.ElapsedMilliseconds}\n";

            foreach ((string key, Task task) in textureTasks)
            {
                loadMapAssetsMain<Texture2D>(key, $"Texture/Map/{mapId}/{key}", ContentType.Texture);
            }
            logs += $"textures main thread done at {stopwatch.ElapsedMilliseconds}\n";

            logs += $"textures and meshes done at {stopwatch.ElapsedMilliseconds}\n";

            logs += $"loading static meshes and triangles at {stopwatch.ElapsedMilliseconds}\n";
            loadedMap.StaticMeshes = map.Objects.Where(o => o.EntityType == EntityType.Mesh)
                .Select(o =>
                {
                    Task<MeshBuffer> meshTask = getMapAssetsAsync<MeshBuffer>(o.Mesh, ContentType.Mesh);
                    Texture2D texture = getMapAssetsMain<Texture2D>(o.Texture, ContentType.Texture);
                    Matrix modelMatrix = Matrix.CreateScale(o.Scale)
                             * Matrix.CreateFromQuaternion(o.Rotation)
                             * Matrix.CreateTranslation(o.Position);
                    MeshBuffer mesh = meshTask.GetAwaiter().GetResult();
                    Vector3[] worldVerts = mesh.CollisionTriangles
                                        .Select(v => Vector3.Transform(v, modelMatrix))
                                        .ToArray();

                    BVHTriangle[] tris = new BVHTriangle[worldVerts.Length / 3];
                    for (int i = 0; i < worldVerts.Length; i += 3)
                    {
                        tris[i / 3] = new BVHTriangle()
                        {
                            A = worldVerts[i],
                            B = worldVerts[i + 1],
                            C = worldVerts[i + 2],
                        };
                    }
                    return new PlacedMesh
                    {
                        Triangles = tris,
                        Mesh = mesh,
                        Texture = texture,
                        MapObject = o,
                        Position = o.Position,
                        Rotation = o.Rotation,
                        Scale = o.Scale,
                        ModelMatrix = modelMatrix
                    };
                }).ToArray();
            logs += $"loaded static meshes and triangles at {stopwatch.ElapsedMilliseconds}\n";

            List<BVHTriangle> mapTriangles = new List<BVHTriangle>();
            foreach (PlacedMesh mesh in loadedMap.StaticMeshes)
            {
                mapTriangles.AddRange(mesh.Triangles);
            }

            Task<string> collisionData = BuildMapCollision(mapTriangles);
            logs += $"starting collision data at {stopwatch.ElapsedMilliseconds}\n";

            loadedMap.Lights = map.Objects.Where(o => o.EntityType == EntityType.Light)
                .Select(o =>
                {
                    EntityLightData lightData = (EntityLightData)o.Meta;
                    return new PointLight
                    {
                        Position = o.Position,
                        Intensity = lightData.Intensity,
                        Radius = lightData.Radius,
                        Color = Color.White,
                    };
                }).ToArray();
            loadedMap.AmbientLight = new PointLight()
            {
                Color = map.AmbientLightColor,
                Intensity = map.AmbientLightIntensity,
            };
            logs += $"lights done at {stopwatch.ElapsedMilliseconds}\n";

            logs += await collisionData;
            logs += $"Map fully done at {stopwatch.ElapsedMilliseconds}\n";
            stopwatch.Stop();
            Debug.WriteLine(logs);

            Ray downRay = new Ray(new Vector3(0, 0, 0), Vector3.Down);
            bool hit = BVHTraversal.RayTest(loadedMap.BVH, downRay, out float distance);

            Debug.WriteLine($"Hit: {hit}, Distance: {distance}");
        }
    }
}
