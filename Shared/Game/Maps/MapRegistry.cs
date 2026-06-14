/*
 * File: MapRegistry.cs
 * File Created: 25 May 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */

namespace Shared.Game.Maps
{
    public static class MapRegistry
    {
        #nullable disable
        static string baseAssetsPath = Path.Combine(Environment.ProcessPath, "..", "Assets", "Maps") + Path.DirectorySeparatorChar;
        #nullable enable
        static Dictionary<string, string> _mapRegistry = new Dictionary<string, string>();

        public static void Init()
        {
            string[] files = Directory.GetFiles(baseAssetsPath, "*.lmf");

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                _mapRegistry.Add(fileName, file);
            }
        }

        public static void InitAndParse()
        {
            string[] files = Directory.GetFiles(baseAssetsPath, "*.lmf");

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                _mapRegistry.Add(fileName, file);
                LMFParser.ParseLMF(file);
            }
        }
    }
}
