namespace Shared.Game.Characters
{
    public static class AbilityChecksumCheck
    {
        private static Dictionary<string, uint> _checksumTable = new Dictionary<string, uint>()
        {
            { "char2_primary_1", 635132808 }
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
            return HeroChecksumCheck.ComputeChecksum(json);
        }
    }
}
