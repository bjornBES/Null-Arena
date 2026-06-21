/*
 * File: PlayMenuPanel.cs
 * File Created: 20 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 21 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Engine.Game.Game;
using Microsoft.Xna.Framework;
using MLEM.Ui;
using MLEM.Ui.Elements;

namespace PlayerClient.Game.PreGameplay.Panals
{
    public class PlayMenuPanel : UiScreen
    {
        public ErrorPanel ErrorPanel;

        public PlayMenuPanel(Anchor anchor, Vector2 size, bool setHeightBasedOnChildren = false, bool scrollOverflow = false, bool autoHideScrollbar = true) : base(anchor, size, setHeightBasedOnChildren, scrollOverflow, autoHideScrollbar)
        {
        }

        public static string Title => "SubMenu";

        public override void Build()
        {
            Button Quickplay = new Button(Anchor.TopLeft, new Vector2(150, 40), "Quick play");
            Quickplay.OnPressed += _ => PlayGamemode(GameplayMode.QuickPlay);
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
