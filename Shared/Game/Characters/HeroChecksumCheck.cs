namespace Shared.Game.Characters
{
    public static class HeroChecksumCheck
    {
        private static Dictionary<string, uint> _checksumTable = new Dictionary<string, uint>()
        {
            { "player_char1", 1054411072 },
            { "player_char2", 3902038262 },
        };

        public static bool Check(string heroId, uint checksum)
        {
            if (!_checksumTable.ContainsKey(heroId))
            {
                return false;
            }
            return checksum == _checksumTable[heroId];
        }


        public static uint ComputeChecksum(string json)
        {
            uint hash = 2166136261u;
            foreach (char c in json)
            {
                hash = (hash ^ c) * 16777619u;
            }

            return hash;
        }
    }
}
