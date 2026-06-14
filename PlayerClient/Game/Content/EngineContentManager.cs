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
        public void AddContent<T>(string assetName, string key, ContentType contentType) where T : class
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
                }
            }
            if (contentManager == null)
            {
                throw new InvalidOperationException($"A ContentManager for a {contentType} was not found");
            }
            contentManager.Load(key, assetName);
        }
        public T GetContent<T>(string key, ContentType contentType) where T : class
        {
            T result;
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
                }
            }
            if (contentManager == null)
            {
                throw new InvalidOperationException($"A ContentManager for a {contentType} was not found");
            }
            result = contentManager.Get(key);
            return result;
        }
    }
}
