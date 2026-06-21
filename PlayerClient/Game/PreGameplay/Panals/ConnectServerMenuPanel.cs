using Microsoft.Xna.Framework;
using MLEM.Ui;
using MLEM.Ui.Elements;
using Shared.Ncode;

namespace PlayerClient.Game.PreGameplay.Panals
{
    public class ConnectServerMenuPanel : UiScreen
    {
        public event Action<ServerInfo> OnServerSelected;
        public ServerListPanel ServerListPanel;

        public ErrorPanel ErrorPanel;

        public ConnectServerMenuPanel(Anchor anchor, Vector2 size, bool setHeightBasedOnChildren = false, bool scrollOverflow = false, bool autoHideScrollbar = true) : base(anchor, size, setHeightBasedOnChildren, scrollOverflow, autoHideScrollbar)
        {
        }

        public static string Title => "ConnectMenu";

        public override void Build()
        {
            RemoveChildren();

            AddChild(new Paragraph(Anchor.TopCenter, 100, "Hello world"));

            Button playButton = new Button(Anchor.BottomLeft, new Vector2(150, 40), "Back");
            playButton.OnPressed += _ => NavigationRequested(GameState.MainMenu1);
            AddChild(playButton);

            ServerListPanel = new ServerListPanel();
            ServerListPanel.OnServerSelected += async (fullAddress) =>
            {
                string address = fullAddress.ServerAddress;
                int port = fullAddress.ServerPort;
                uint result = ConnectedToServer(fullAddress);
                if (result == NetworkConstants.SUCCESS)
                {
                    ErrorPanel = new ErrorPanel("Successfully connected",
                        $"Successfully connected to the server",
                        ["ok"], [() => NavigationRequested(GameState.MainMenu2)]);
                    OnServerSelected?.Invoke(fullAddress);
                    AddChild(ErrorPanel);
                }
                else
                {
                    ErrorPanel = new ErrorPanel("Connection Failed",
                        $"Could not reach the server. Check your network and try again.\nError code: {result}",
                        ["ok"], [() => NavigationRequested(GameState.MainMenu1)]);
                    AddChild(ErrorPanel);
                }
            };
            AddChild(ServerListPanel);
        }

        public override void OnEnter()
        {
            ServerListPanel.PopulateServers(GetServers());
        }

        public override void OnExit()
        {
            ServerListPanel.RemoveAll();
        }
    }
}
