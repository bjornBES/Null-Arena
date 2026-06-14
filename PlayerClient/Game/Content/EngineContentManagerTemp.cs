/*
 * File: EngineContentManagerTemp.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework.Content;


namespace PlayerClient.Game.Content
{
    public interface IContentSystem
    {
        Type ContentType { get; }
    }
    public interface IContentSystem<T> : IContentSystem where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assetName"></param>
        void Load(string key, string assetName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get(string key);

        /// <summary>
        /// Checks if an asset is loaded.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// Unloads a cached asset defined by the <see cref="key"/> from memory.
        /// </summary>
        /// <param name="key">the cached asset's key</param>
        void Unload(string key);

        /// <summary>
        /// Unloads all cached assets from memory.
        /// </summary>
        void UnloadAll();
    }
    public abstract class EngineContentManagerTemp<T> : IContentSystem<T> where T : class
    {
        public Type ContentType => typeof(T);

        protected readonly ContentManager _content;
        protected readonly Dictionary<int, T> _registry;
        protected readonly Dictionary<int, string> _keyCache;
        protected int _cacheIndex;

        public EngineContentManagerTemp(ContentManager content)
        {
            ContentSystemRegistry.Register(this);
            _content = content;
            _keyCache = new Dictionary<int, string>();
            _registry = new Dictionary<int, T>();
            _cacheIndex = 0;
        }

        protected int GetIntKey(string key)
        {
            int index = -1;
            if (!_keyCache.ContainsValue(key))
            {
                index = _cacheIndex;
                _keyCache[index] = key;
                _cacheIndex++;
            }
            else
            {
                foreach (string value in _keyCache.Values)
                {
                    if (value == key)
                    {
                        index = _keyCache.Values.ToList().IndexOf(value);
                        break;
                    }
                }
            }
            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="assetName"></param>
        public abstract void Load(string key, string assetName);
        protected abstract void Load(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract T Get(string key);

        /// <summary>
        /// Checks if an asset is loaded.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool Exists(string key);
        protected bool InternalExists(string key) => _keyCache.Values.Contains(key);

        /// <summary>
        /// Unloads a cached asset defined by the <see cref="key"/> from memory.
        /// </summary>
        /// <param name="key">the cached asset's key</param>
        public abstract void Unload(string key);

        /// <summary>
        /// Unloads all cached assets from memory.
        /// </summary>
        public abstract void UnloadAll();
    }
}
