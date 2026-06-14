using Microsoft.Xna.Framework;
using MLEM.Ui;
using MLEM.Ui.Elements;

namespace PlayerClient.Game.PreGameplay.Panals
{
    internal class MainMenu1Screen : UiScreen
    {
        public MainMenu1Screen(Anchor anchor, Vector2 size, bool setHeightBasedOnChildren = false, bool scrollOverflow = false, bool autoHideScrollbar = true) : base(anchor, size, setHeightBasedOnChildren, scrollOverflow, autoHideScrollbar)
        {
        }

        public override void Build()
        {
            Button connectToServerButton = new Button(Anchor.TopLeft, new Vector2(150, 40), "Connect To Server");
            connectToServerButton.OnPressed += _ => NavigationRequested(GameState.Connect);
            AddChild(connectToServerButton);
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }
    }
}
