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
using Shared.Network;
using EntityManager = PlayerClient.Game.Gameplay.EntitySystem.EntityManager;

namespace PlayerClient.Game.Gameplay
{
    public class GameplayManager
    {
        private GraphicsDevice graphicsDevice;
        private GraphicsDeviceManager graphics;
        internal SpriteBatch spriteBatch;

        MonoGameType Game;

        // game systems
        RenderingSystem renderingSystem;

        List<ManagerClass> _managers = new List<ManagerClass>();
        // game system managers
        GameplayContext gameplayContext;
        EntityManager entityManager;
        NetworkManager networkManager;
        MapManager mapManager;

        InputSystem inputSystem;

        public GameplayManager(MonoGameType game)
        {
            Game = game;
            graphics = new GraphicsDeviceManager(game);

            entityManager = new EntityManager();
            networkManager = new NetworkManager();
            mapManager = new MapManager();

            _managers.Add(entityManager);
            _managers.Add(networkManager);
            _managers.Add(mapManager);

        }

        void doActionForeach(Action<ManagerClass> action)
        {
            foreach (ManagerClass manager in _managers)
            {
                action.Invoke(manager);
            }
        }

        public void Initialize()
        {
            WindowManager.Game = Game.Window;
            _ = new EngineContentManager(Game.Content);

            // Managers
            _ = new TextureManager(Game.Content);

            inputSystem = new InputSystem();

            graphicsDevice = Game.GraphicsDevice;
            renderingSystem = new RenderingSystem(Game);
            gameplayContext = new GameplayContext();
            gameplayContext.RenderCommands = new List<RenderCommand>();
            gameplayContext.Camera = renderingSystem.Camera;
            gameplayContext.NetworkManager = networkManager;

            networkManager.Initialize(gameplayContext);
            mapManager.Initialize(gameplayContext);
            entityManager.Initialize(gameplayContext);
        }

        public void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphicsDevice);

            entityManager.LoadContent(gameplayContext, EngineContentManager.Instance);
            mapManager.LoadContent(gameplayContext, EngineContentManager.Instance);
        }

        public uint OnConnectToServer(ServerInfo server)
        {
            return networkManager.ConnectToServer(server);
        }
        public void StartMatch()
        {
            networkManager.StartMatch(gameplayContext);
            mapManager.StartMatch(gameplayContext);
        }

        public void Update(GameTime gameTime)
        {
            if (Game.IsActive)
            {
                inputSystem.Update();

                entityManager.Update(gameplayContext, gameTime);
                mapManager.Update(gameplayContext, gameTime);

                renderingSystem.Update(gameTime);

                entityManager.PostPhysicsUpdate(gameplayContext, gameTime);
                mapManager.PostPhysicsUpdate(gameplayContext, gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            gameplayContext.RenderCommands.Clear();
            graphicsDevice.Clear(Color.CornflowerBlue);

            entityManager.Draw(gameplayContext, gameTime);
            mapManager.Draw(gameplayContext, gameTime);

            List<RenderCommand> commands = gameplayContext.RenderCommands;
            List<RenderCommand> sorted = [.. commands.OrderByDescending(b => b.Depth)];

            renderingSystem.Render(sorted);
        }
    }

}
