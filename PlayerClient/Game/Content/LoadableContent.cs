/*
 * File: LoadableContent.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework.Graphics;

namespace PlayerClient.Game.Content
{
    public enum ContentType
    {
        Texture,
        Mesh,
    }
    public class LoadableContent
    {
        public string Key { get; set; }
        public string AssetName { get; set; }
        public ContentType ContentType { get; set; }

        public LoadableContent(string key, string assetName, ContentType contentType)
        {
            Key = key;
            AssetName = assetName;
            ContentType = contentType;
        }

        public void Load()
        {
            try
            {
                switch (ContentType)
                {
                    case ContentType.Texture:
                        Load<Texture2D>();
                        break;
                }
            }
            catch (InvalidOperationException)
            {
                Console.Error.WriteLine($"{AssetName} didn't exist as {ContentType}");
            }
        }
        public void Load<T>() where T : class
        {
            try
            {
                IContentSystem<Texture2D> content = null;
                switch (ContentType)
                {
                    case ContentType.Texture:
                        content = ContentSystemRegistry.Get<Texture2D>();
                        break;
                }
                content.LoadMainThread(Key, AssetName);
                Console.WriteLine($"Loaded a {ContentType} {AssetName} as {Key}");
            }
            catch (InvalidOperationException)
            {
                Console.Error.WriteLine($"{AssetName} didn't exist as {ContentType}");
            }
        }
    }
}
