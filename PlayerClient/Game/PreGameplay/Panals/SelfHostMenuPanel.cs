using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Ui;
using MLEM.Ui.Elements;
using PlayerClient.Game.Content;
using PlayerClient.Game.Gameplay;
using PlayerClient.Game.Gameplay.NetworkSystem;
using Shared.Game;
using Shared.Game.Matchs;
using Shared.Network.Package.matchs;

namespace PlayerClient.Game.PreGameplay.Panals
{
    public class SelfHostMenuPanel : UiScreen
    {
        public ErrorPanel ErrorPanel;

        public SelfHostMenuPanel(Anchor anchor, Vector2 size, bool setHeightBasedOnChildren = false, bool scrollOverflow = false, bool autoHideScrollbar = true) : base(anchor, size, setHeightBasedOnChildren, scrollOverflow, autoHideScrollbar)
        {
        }

        public static string Title => "SubMenu";

        public override void Build()
        {

            Button Quickplay = new Button(Anchor.TopLeft, new Vector2(150, 40), "Map");
            Quickplay.OnPressed += _ =>
            {
                EngineContentManager.Instance.GetManager<MeshBuffer>(ContentType.Mesh).UnloadAll();
                EngineContentManager.Instance.GetManager<Texture2D>(ContentType.Texture).UnloadAll();
                PacketDespasher.DispaschHandler(new GetMatchPackages()
                {
                    MatchId = new MatchId(0),
                    MapId = "mparena01".ToCharArray(),
                    MapIdLength = "mparena01".Length,
                    Sequence = 0,
                });
                NavigationRequested(GameState.Gameplay);
            };
            AddChild(Quickplay);
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }
    }
}
