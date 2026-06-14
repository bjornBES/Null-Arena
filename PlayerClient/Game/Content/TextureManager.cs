/*
 * File: TextureManager.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PlayerClient.Game.Content
{
    public sealed class TextureManager : EngineContentManagerTemp<Texture2D>
    {

        public TextureManager(ContentManager content) : base(content)
        {
        }

        /// <summary>
        /// Loads a texture from the Content pipeline and caches it.
        /// </summary>
        public override void Load(string key, string assetName)
        {
            int indexKey = GetIntKey(key);
            if (_registry.ContainsKey(indexKey))
            {
                throw new InvalidOperationException($"Texture key {indexKey} ('{key}') is already loaded.");
            }

            Texture2D texture = _content.Load<Texture2D>(assetName);
            _registry[indexKey] = texture;
        }

        protected override void Load(string key)
        {
            int indexKey = GetIntKey(key);
            if (!_keyCache.TryGetValue(indexKey, out string asset))
            {
                throw new KeyNotFoundException($"Texture key {indexKey} ('{key}') has never been loaded.");
            }

            _registry[indexKey] = _content.Load<Texture2D>(asset);
        }

        /// <summary>
        /// Retrieves a cached texture.
        /// </summary>
        public override Texture2D Get(string key)
        {
            int indexKey = GetIntKey(key);
            if (!_registry.TryGetValue(indexKey, out Texture2D texture))
            {
                Load(key);
            }

            return texture;
        }

        public override bool Exists(string key) => InternalExists(key);

        public override void Unload(string key)
        {
            int keyIndex = GetIntKey(key);
            if (_registry.TryGetValue(keyIndex, out Texture2D texture))
            {
                texture.Dispose();
                _registry.Remove(keyIndex);
            }
        }

        public override void UnloadAll()
        {
            foreach (Texture2D tex in _registry.Values)
            {
                tex.Dispose();
            }

            _registry.Clear();
        }
    }

}
