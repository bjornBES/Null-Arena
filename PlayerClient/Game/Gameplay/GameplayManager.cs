/*
 * File: GameplayManager.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerClient.Game.Content;
using PlayerClient.Game.Gameplay.InputSubsystem;
using PlayerClient.Game.Gameplay.MapSystem;
using PlayerClient.Game.Gameplay.NetworkSystem;
using PlayerClient.Game.Gameplay.Rendering;
using Shared.Game.BVH;
using Shared.Network;
using Shared.Network.Package.matchs;
using EntityManager = PlayerClient.Game.Gameplay.EntitySystem.EntityManager;

namespace PlayerClient.Game.Gameplay
{
    public class GameplayManager
    {
        internal GraphicsDevice GraphicsDevice;
        private GraphicsDeviceManager graphics;
        internal SpriteBatch SpriteBatch;

        Game1 Game;

        // game systems
        RenderingSystem renderingSystem;

        List<ManagerClass> _managers = new List<ManagerClass>();
        // game system managers
        GameplayContext gameplayContext;
        public EntityManager EntityManager;
        public NetworkManager NetworkManager;
        public MapManager MapManager;

        internal InputSystem inputSystem;


        public GameplayManager(Game1 game)
        {
            Game = game;

            graphics = new GraphicsDeviceManager(game);

            EntityManager = new EntityManager();
            NetworkManager = new NetworkManager();
            MapManager = new MapManager();

            _managers.Add(EntityManager);
            _managers.Add(NetworkManager);
            _managers.Add(MapManager);

        }

        public void Initialize()
        {
            WindowManager.Game = Game.Window;
            _ = new EngineContentManager(Game.Content);

            // Managers
            GraphicsDevice = Game.GraphicsDevice;
            _ = new TextureManager(Game.Content, GraphicsDevice);
            _ = new MeshManager(Game.Content, GraphicsDevice);

            inputSystem = new InputSystem();

            renderingSystem = new RenderingSystem(Game);
            gameplayContext = new GameplayContext();
            gameplayContext.RenderCommands = new List<RenderCommand>();
            gameplayContext.Camera = renderingSystem.Camera;
            gameplayContext.NetworkManager = NetworkManager;
            gameplayContext.MapManager = MapManager;

            NetworkManager.Initialize(gameplayContext);
            MapManager.Initialize(gameplayContext);
            EntityManager.Initialize(gameplayContext);

            PacketDespasher.OnMatchStartPackages += StartMatch;
        }

        public void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            gameplayContext.SpriteBatch = SpriteBatch;

            EntityManager.LoadContent(gameplayContext, EngineContentManager.Instance);
            MapManager.LoadContent(gameplayContext, EngineContentManager.Instance);
        }

        public uint OnConnectToServer(ServerInfo server)
        {
            return NetworkManager.ConnectToServer(server);
        }
        public void StartMatch(GetMatchPackages mapPackages)
        {
            EntityManager.StartMatch(gameplayContext);
            NetworkManager.StartMatch(gameplayContext);
            MapManager.StartMatch(gameplayContext);
        }

        public void Update(GameTime gameTime)
        {
            if (Game.IsActive)
            {
                EntityManager.Update(gameplayContext, gameTime);
                MapManager.Update(gameplayContext, gameTime);

                renderingSystem.Update(gameTime);

                EntityManager.PostPhysicsUpdate(gameplayContext, gameTime);
                MapManager.PostPhysicsUpdate(gameplayContext, gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            gameplayContext.RenderCommands.Clear();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            EntityManager.Draw(gameplayContext, gameTime);
            MapManager.Draw(gameplayContext, gameTime);

            List<RenderCommand> commands = gameplayContext.RenderCommands;
            List<RenderCommand> sorted = [.. commands.OrderByDescending(b => b.Depth)];

            renderingSystem.Render(sorted);
            SpriteBatch.End();
        }

        public void Unload()
        {
            MapManager.Unload(gameplayContext);
            EntityManager.Unload(gameplayContext);
            NetworkManager.Unload(gameplayContext);
        }
    }

}
