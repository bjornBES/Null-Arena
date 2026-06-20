/*
 * File: Game1.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 20 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Diagnostics;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlayerClient.Game.Content;
using PlayerClient.Game.Gameplay;
using PlayerClient.Game.Gameplay.InputSubsystem;
using PlayerClient.Game.Gameplay.NetworkSystem;
using PlayerClient.Game.PreGameplay;
using RemoteConsole;
using Shared.Core.Network;
using Shared.Core.EasyArgs;
using Shared.Core.Game;
using Shared.Core.Game.InputSystem;
using Shared.Core.Game.Matches;
using Shared.Core.Game.Player;
using Shared.Core.Network.Package.Matches;

namespace PlayerClient
{
    public class ClientArguments
    {
        [Arg("-s", "--server")]
        public string GameServerIp { get; set; }

        [Arg("-p", "--port")]
        public int GameServerPort { get; set; }

        [Arg("-m", "--master-server")]
        public string MasterServerIp { get; set; }

        [Arg("", "--self-host")]
        public bool IsSelfHosted { get; set; }
    }
    public class Game1 : MonoGameType
    {
        public GameplayManager GameplayManager;
        public ScreenManager ScreenManager;
        public DebugConsole debugConsole;
        public static ClientArguments ClientArguments { get; private set; }

        public bool runningGameplay = true;

        public Game1(string[] args)
        {
            ClientArguments = EasyArgs.Parse<ClientArguments>(args);

            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;

            PlayerIdentity player = PlayerIdentity.Generate("BjornBEs");

            GameplayManager = new GameplayManager(this);
            ScreenManager = new ScreenManager();

            this.Activated += GameFocus;
            this.Deactivated += GameUnfocus;
        }

        private void GameUnfocus(object sender, EventArgs e)
        {
            IsMouseVisible = true;
            runningGameplay = false;
        }

        private void GameFocus(object sender, EventArgs e)
        {
            IsMouseVisible = false;
            runningGameplay = true;
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
            debugConsole = new DebugConsole(Content.Load<SpriteFont>("Fonts/Arial"), GraphicsDevice, GameplayManager.SpriteBatch);
            debugConsole.OnCommand += (string cmd) =>
            {
                Debug.WriteLine(cmd);
                string[] segments = cmd.Split(' ');
                string command = segments.First();
                switch (command)
                {
                    case "match":
                        EngineContentManager.Instance.GetManager<MeshBuffer>(ContentType.Mesh).UnloadAll();
                        EngineContentManager.Instance.GetManager<Texture2D>(ContentType.Texture).UnloadAll();
                        GameplayManager.Unload();
                        PacketDespatcher.DispatchHandler(new GetMatchPackages()
                        {
                            MatchId = new MatchId(0),
                            MapId = "mparena01".ToCharArray(),
                            MapIdLength = "mparena01".Length,
                            Sequence = 0,
                        });
                        ScreenManager.GoTo(GameState.Gameplay);
                        ScreenManager.IsActive = false;
                        break;
                    default:
                        break;
                }
            };

            if (ClientArguments.IsSelfHosted)
            {
                IPEndPoint local = NetworkFunctions.GetLocalIP();
                GameplayManager.OnConnectToServer(new ServerInfo("Offline", local.ToString(), "EU_DK", GameServerType.Offline));
                ScreenManager.GoTo(GameState.MainMenu2);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (runningGameplay)
            {
                MouseState mouseState = new MouseState();
                if (ClientArguments.IsSelfHosted)
                {
                    mouseState = Mouse.GetState();
                    if (Input.IsActionPressed(InputActionFlags.Debug.ToString()))
                    {
                        debugConsole.Toggle();
                    }
                    debugConsole.Update();
                }
                GameplayManager.inputSystem.Update();

                if (ScreenManager.IsActive || debugConsole.Visible)
                {
                    IsMouseVisible = true;
                    Mouse.SetPosition(mouseState.X, mouseState.Y);
                }

                ScreenManager.Update(gameTime);

                if (!ScreenManager.IsActive)
                {
                    IsMouseVisible = false;
                    GameplayManager.Update(gameTime);
                }
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!ScreenManager.IsActive)
            {
                GameplayManager.Draw(gameTime);
            }
            ScreenManager.Draw(gameTime);
            if (ClientArguments.IsSelfHosted)
            {
                GameplayManager.SpriteBatch.Begin();
                debugConsole.Draw();
                GameplayManager.SpriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public List<ServerInfo> GetServers()
        {
            return [];
        }

        public void PlayGamemode(GameplayMode mode)
        {
            if (ClientArguments.IsSelfHosted)
            {
                ScreenManager.GoTo(GameState.SelfHost);
            }
            else
            {
                ScreenManager.GoTo(GameState.Lobby);
            }
        }
    }
}
