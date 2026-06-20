/*
 * File: GameplayContext.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 25 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework.Graphics;
using PlayerClient.Game.Gameplay.MapSystem;
using PlayerClient.Game.Gameplay.NetworkSystem;
using PlayerClient.Game.Gameplay.Rendering;

namespace PlayerClient.Game.Gameplay
{
    public class GameplayContext
    {
        public SpriteBatch SpriteBatch;
        public Camera Camera;
        public List<RenderCommand> RenderCommands;
        public NetworkManager NetworkManager;
        public MapManager MapManager;
    }

}
