/*
 * File: MapRegistry.cs
 * File Created: 25 May 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Shared.Game.Maps.LMF;

namespace Shared.Game.Maps
{
    public static class MapRegistry
    {
        #nullable disable
        static string baseAssetsPath = Path.Combine("./", "Assets", "Maps") + Path.DirectorySeparatorChar;
#nullable enable
        public static string[] AvailableMaps;
        static Dictionary<string, Map> _mapRegistry = new Dictionary<string, Map>();

        public static void Init()
        {
            string[] files = Directory.GetFiles(baseAssetsPath, "*.lmf");
            List<string> maps = new List<string>();
            foreach (string file in files)
            {
                string mapId = Path.GetFileNameWithoutExtension(file);
                maps.Add(mapId);
            }
            AvailableMaps = maps.ToArray();
        }

        public static void InitAndParse()
        {
        }

        public static Map GetMap(string id)
        {
            string fileName = Path.Combine(baseAssetsPath, id + ".lmf");
            return LMFParser.ParseLMF(fileName);
        }
    }
}
