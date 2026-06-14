using Microsoft.Xna.Framework;
using MLEM.Ui;
using MLEM.Ui.Elements;
using Shared.Network;

namespace PlayerClient.Game.PreGameplay.Panals
{
    public class ServerListPanel : Panel
    {
        public event Action<ServerInfo> OnServerSelected; // passes address

        private readonly Panel _scrollableList;

        public ServerListPanel() : base(Anchor.Center, new Vector2(300, 400), Vector2.Zero)
        {
            Paragraph title = new Paragraph(Anchor.AutoLeft, 280, "Select Server");
            AddChild(title);

            // scrollable=true enables the scrollbar automatically
            _scrollableList = new Panel(Anchor.AutoLeft, new Vector2(1f, 0.8f), Vector2.Zero, scrollOverflow: true);
            AddChild(_scrollableList);
        }

        public void RemoveAll()
        {
            _scrollableList.RemoveChildren();
        }

        public void PopulateServers(List<ServerInfo> servers)
        {
            _scrollableList.RemoveChildren();

            foreach (ServerInfo serverInfo in servers)
            {
                Button btn = new Button(Anchor.AutoLeft, new Vector2(1f, 40), serverInfo.ServerName);
                ServerInfo capturedServer = serverInfo;
                btn.OnPressed += _ => OnServerSelected?.Invoke(capturedServer);
                _scrollableList.AddChild(btn);
            }

            if (!string.IsNullOrEmpty(Game1.ClientArguments.GameServerIp))
            {
                Button offline = new Button(Anchor.AutoLeft, new Vector2(1f, 40), "Dedicated");
                ServerInfo offlineCapturedServer = new ServerInfo("Dedicated", Game1.ClientArguments.GameServerIp + ":" + Game1.ClientArguments.GameServerPort, "");
                offline.OnPressed += _ => OnServerSelected?.Invoke(offlineCapturedServer);
                _scrollableList.AddChild(offline);
            }
            else
            {
                Button offline = new Button(Anchor.AutoLeft, new Vector2(1f, 40), "Offline");
                ServerInfo offlineCapturedServer = new ServerInfo("Offline", "127.0.0.1:0000", "");
                offline.OnPressed += _ => OnServerSelected?.Invoke(offlineCapturedServer);
                _scrollableList.AddChild(offline);
            }
        }
    }
}
