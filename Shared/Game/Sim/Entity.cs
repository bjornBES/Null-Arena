/*
 * File: Entity.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;

namespace Shared.Game.Sim
{
    public class Entity
    {
        public Vector3 Position;
        public string textureKey;

        public Entity(Vector3 position, string key)
        {
            Position = position;
            textureKey = key;
        }
    }
}
