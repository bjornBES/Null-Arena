/*
 * File: ManagerClass.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 25 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using PlayerClient.Game.Content;

namespace PlayerClient.Game.Gameplay
{
    public abstract class ManagerClass
    {
        public ManagerClass()
        {
        }

        public abstract void Initialize(GameplayContext context);
        public abstract void LoadContent(GameplayContext context, EngineContentManager contentManager);

        public abstract void StartMatch(GameplayContext context);
        public abstract void Update(GameplayContext context, GameTime gameTime);
        public abstract void PostPhysicsUpdate(GameplayContext context, GameTime gameTime);
        public abstract void Draw(GameplayContext context, GameTime gameTime);
        public abstract void Unload(GameplayContext context);

    }

}
