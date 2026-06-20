using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Shared.Game.BVH;

namespace PlayerClient.Game.Gameplay.BVH
{
    public class BVHGPUTraversal
    {
        public static BVHGPUTraversal Instance { get; internal set; }
        internal ContentManager contentManager;
        internal GraphicsDevice graphicsDevice;
        internal BVHGPUTraversal(Game1 game)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                return;
            }
            contentManager = game.Content;
            graphicsDevice = game.GraphicsDevice;
        }
    }
}
