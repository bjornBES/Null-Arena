using Microsoft.Xna.Framework;
using PlayerClient.Game.Gameplay;
using PlayerClient.Game.PreGameplay;
using Shared.EasyArgs;
using Shared.Game;
using Shared.Game.Player;
using Shared.Network;

namespace PlayerClient
{
    public class ClientArguments
    {
        [Arg("-s", "--server")]
        public string GameServerIp { get; set; }

        [Arg("-p", "--port")]
        public int GameServerPort { get; set; }
    }
    public class Game1 : MonoGameType
    {
        public GameplayManager GameplayManager;
        public ScreenManager ScreenManager;
        public static ClientArguments ClientArguments { get; private set; }

        public Game1(string[] args)
        {
            ClientArguments = EasyArgs.Parse<ClientArguments>(args);
            PlayerIdentity player = PlayerIdentity.Generate("BjornBEs");
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            GameplayManager = new GameplayManager(this);
            ScreenManager = new ScreenManager();
        }

        protected override void Initialize()
        {
            GameplayManager.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GameplayManager.LoadContent();

            ScreenManager.Initialize(this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (ScreenManager.IsActive)
            {
                IsMouseVisible = true;
                ScreenManager.Update(gameTime);
            }
            else
            {
                IsMouseVisible = false;
                GameplayManager.Update(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (ScreenManager.IsActive)
            {
                ScreenManager.Draw(gameTime);
            }
            else
            {
                GameplayManager.Draw(gameTime);
            }
            base.Draw(gameTime);
        }

        public List<ServerInfo> GetServers()
        {
            return [];
        }

        public void PlayGamemode(GameplayMode mode)
        {

            ScreenManager.GoTo(GameState.Lobby);
        }
    }
}
