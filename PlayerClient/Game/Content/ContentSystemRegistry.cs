/*
 * File: ContentSystemRegistry.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

namespace PlayerClient.Game.Content
{
    public static class ContentSystemRegistry
    {
        private static readonly Dictionary<Type, IContentSystem> _systems = new();

        public static void Register(IContentSystem system)
        {
            _systems.Add(system.ContentType, system);
        }

        public static IContentSystem<T> Get<T>() where T : class
        {
            return (IContentSystem<T>)_systems[typeof(T)];
        }
    }
}
