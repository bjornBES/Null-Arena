/*
 * File: EngineContentManager.cs
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
    public class EngineContentManager
    {
        public static EngineContentManager Instance { get; internal set; }
        internal ContentManager _contentManager;
        internal EngineContentManager(ContentManager contentManager)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                return;
            }
            _contentManager = contentManager;
        }

        internal void Update()
        {
        }

        internal void Destroy()
        {

        }

        // Public methods
        public IContentSystem<T> GetManager<T>(ContentType contentType) where T : class
        {
            IContentSystem<T> contentManager = null;
            try
            {
                contentManager = ContentSystemRegistry.Get<T>();
            }
            catch (Exception)
            {
                switch (contentType)
                {
                    case ContentType.Texture:
                        contentManager = (IContentSystem<T>)ContentSystemRegistry.Get<Texture2D>();
                        break;
                    case ContentType.Mesh:
                        contentManager = (IContentSystem<T>)ContentSystemRegistry.Get<MeshBuffer>();
                        break;
                }
            }
            if (contentManager == null)
            {
                throw new InvalidOperationException($"A ContentManager for a {contentType} was not found");
            }
            return contentManager;
        }
        public async Task AddContentAsync<T>(string key, string assetName, ContentType contentType) where T : class
        {
            IContentSystem<T> contentManager = GetManager<T>(contentType);
            await contentManager.LoadAsync(key, assetName);
        }
        public void AddContent<T>(string key, string assetName, ContentType contentType) where T : class
        {
            IContentSystem<T> contentManager = GetManager<T>(contentType);
            contentManager.LoadMainThread(key, assetName);
        }
        public async Task<T> GetContentAsync<T>(string key, ContentType contentType) where T : class
        {
            T result;
            IContentSystem<T> contentManager = GetManager<T>(contentType);
            result = contentManager.Get(key);
            return result;
        }
        public T GetContent<T>(string key, ContentType contentType) where T : class
        {
            T result;
            IContentSystem<T> contentManager = GetManager<T>(contentType);
            result = contentManager.Get(key);
            return result;
        }
    }
}
