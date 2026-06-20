using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Font;
using MLEM.Misc;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using PlayerClient.Game.PreGameplay.Panals;

namespace PlayerClient.Game.PreGameplay
{
    public class ScreenManager
    {
        public bool IsActive;
        public UiSystem UiSystem;
        public Game1 Game1;
        public Dictionary<GameState, UiScreen> Screens = new Dictionary<GameState, UiScreen>();
        public Panel RootPanel;

        public ScreenManager()
        {
            Screens.Add(GameState.MainMenu1, new MainMenu1Screen(Anchor.Center, Vector2.One));
            Screens.Add(GameState.Connect, new ConnectServerMenuPanel(Anchor.Center, Vector2.One));
            Screens.Add(GameState.MainMenu2, new MainMenu2Screen(Anchor.Center, Vector2.One));
            Screens.Add(GameState.Play, new PlayMenuPanel(Anchor.Center, Vector2.One));
            Screens.Add(GameState.SelfHost, new SelfHostMenuPanel(Anchor.Center, Vector2.One));
            Screens.Add(GameState.Gameplay, new GameplayPanel(Anchor.Center, Vector2.One));
            IsActive = true;
        }

        public void Initialize(Game1 game)
        {
            Game1 = game;
            RootPanel = new Panel(Anchor.Center, Vector2.One);
            // Set up platform (required for text input on desktop)
            MlemPlatform.Current = new MlemPlatform.DesktopGl<TextInputEventArgs>(
                (w, c) => w.TextInput += c
            );

            // Initialize the UI system
            UiStyle style = new UntexturedStyle(game.GameplayManager.SpriteBatch)
            {
                Font = new GenericSpriteFont(Game1.Content.Load<SpriteFont>("Fonts/Arial")),
                PanelTexture = new MLEM.Textures.NinePatch(new MLEM.Textures.TextureRegion(Game1.Content.Load<Texture2D>("Texture/ui_gameplay")), 0),
                PanelColor = Color.Transparent,
                ButtonColor = Color.Transparent,
            };
            UiSystem = new UiSystem(game, style)
            {
                Style = style,
                GlobalScale = 1.25f,
                AutoScaleWithScreen = true,
                DrawAlpha = 255
            };
            UiSystem.Add("root", RootPanel);

            foreach (KeyValuePair<GameState, UiScreen> item in Screens)
            {
                item.Value.Build();
                item.Value.NavigationRequested += GoTo;
                item.Value.ConnectedToServer += Game1.GameplayManager.OnConnectToServer;
                item.Value.GetServers += Game1.GetServers;
                item.Value.PlayGamemode += Game1.PlayGamemode;
            }

            GoTo(GameState.MainMenu1);
        }

        public void Update(GameTime delta)
        {
            UiSystem.Update(delta);
        }

        public void Draw(GameTime delta)
        {
            UiSystem.Draw(delta, Game1.GameplayManager.SpriteBatch);
        }

        public void GoTo(GameState state)
        {
            if (state == GameState.Gameplay)
            {
                IsActive = false;
            }
            else
            {
                IsActive = true;
            }
            // Transition
            RootPanel.RemoveChildren((ele) => { ((UiScreen)ele).OnExit(); return true; });
            RootPanel.AddChild(Screens[state]);
            Screens[state].OnEnter();
        }
    }
}
