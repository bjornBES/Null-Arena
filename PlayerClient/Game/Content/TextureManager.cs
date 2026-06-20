/*
 * File: TextureManager.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PlayerClient.Game.Content
{
    public sealed class TextureManager : EngineContentManagerTemp<Texture2D>
    {
        private Dictionary<string, byte[]> _pendingBytes = new Dictionary<string, byte[]>();

        public TextureManager(ContentManager content, GraphicsDevice graphicsDevice) : base(content, graphicsDevice)
        {
        }

        /// <summary>
        /// Loads a texture from the Content pipeline and caches it.
        /// </summary>
        public override async Task LoadAsync(string key, string assetName)
        {
            string path = Path.Combine("./", _content.RootDirectory, assetName);
            if (File.Exists(path))
            {
                _pendingBytes[key] = await File.ReadAllBytesAsync(path); // pure I/O, safe on background thread
            }
        }

        /// <summary>
        /// Loads a texture from the Content pipeline and caches it.
        /// </summary>
        public override void LoadMainThread(string key, string assetName)
        {
            int indexKey = GetIntKey(key);
            if (_registry.ContainsKey(indexKey))
            {
                // throw new InvalidOperationException($"Texture key {indexKey} ('{key}') is already loaded.");
                return;
            }

            Texture2D texture;
            if (_pendingBytes.TryGetValue(key, out byte[] bytes))
            {
                using MemoryStream stream = new MemoryStream(bytes);
                texture = Texture2D.FromStream(graphicsDevice, stream);
                _pendingBytes.Remove(key);
            }
            else
            {
                texture = _content.Load<Texture2D>(assetName);
            }
            _registry[indexKey] = texture;
        }

        protected override void LoadMainThread(string key)
        {
            int indexKey = GetIntKey(key);
            if (!_keyCache.TryGetValue(indexKey, out string assetName))
            {
                throw new KeyNotFoundException($"Texture key {indexKey} ('{key}') has never been loaded.");
            }

            LoadMainThread(key, assetName);
        }

        /// <summary>
        /// Retrieves a cached texture.
        /// </summary>
        public override Texture2D Get(string key)
        {
            int indexKey = GetIntKey(key);
            if (!_registry.TryGetValue(indexKey, out Texture2D texture))
            {
                LoadMainThread(key);
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
