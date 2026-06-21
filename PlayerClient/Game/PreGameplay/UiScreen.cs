/*
 * File: UiScreen.cs
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
using Shared.Ncode;

namespace PlayerClient.Game.PreGameplay
{
    public abstract class UiScreen : Panel
    {
        public Action<GameState> NavigationRequested;
        public Func<ServerInfo, uint> ConnectedToServer;
        public Func<List<ServerInfo>> GetServers;
        public Action<GameplayMode> PlayGamemode;
        protected UiScreen(Anchor anchor, Vector2 size, bool setHeightBasedOnChildren = false, bool scrollOverflow = false, bool autoHideScrollbar = true) : base(anchor, size, setHeightBasedOnChildren, scrollOverflow, autoHideScrollbar)
        {
        }

        protected UiScreen(Anchor anchor, Vector2 size, Vector2 positionOffset, bool setHeightBasedOnChildren = false, bool scrollOverflow = false, bool autoHideScrollbar = true) : base(anchor, size, positionOffset, setHeightBasedOnChildren, scrollOverflow, autoHideScrollbar)
        {
        }

        public abstract void Build();
        public abstract void OnEnter();
        public abstract void OnExit();
    }
}
