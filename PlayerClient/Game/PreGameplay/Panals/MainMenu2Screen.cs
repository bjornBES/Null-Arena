using Microsoft.Xna.Framework;
using MLEM.Ui;
using MLEM.Ui.Elements;

namespace PlayerClient.Game.PreGameplay.Panals
{
    internal class MainMenu2Screen : UiScreen
    {
        public MainMenu2Screen(Anchor anchor, Vector2 size, bool setHeightBasedOnChildren = false, bool scrollOverflow = false, bool autoHideScrollbar = true) : base(anchor, size, setHeightBasedOnChildren, scrollOverflow, autoHideScrollbar)
        {
        }

        public override void Build()
        {
            Button playButton = new Button(Anchor.TopLeft, new Vector2(150, 40), "Play");
            playButton.OnPressed += _ => NavigationRequested(GameState.Play);
            AddChild(playButton);
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }
    }
}
