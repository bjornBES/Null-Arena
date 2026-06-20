using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MLEM.Ui;

namespace PlayerClient.Game.PreGameplay.Panals
{
    public class GameplayPanel : UiScreen
    {
        public GameplayPanel(Anchor anchor, Vector2 size, bool setHeightBasedOnChildren = false, bool scrollOverflow = false, bool autoHideScrollbar = true) : base(anchor, size, setHeightBasedOnChildren, scrollOverflow, autoHideScrollbar)
        {
            Texture = null;
            DrawColor = new MLEM.Ui.Style.StyleProp<Color>(Color.Transparent);
        }

        public override void Build()
        {
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }
    }
}
