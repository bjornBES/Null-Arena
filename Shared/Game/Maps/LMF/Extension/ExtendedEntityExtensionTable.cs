using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Game.Maps.LMF.Extension
{
    public class ExtendedEntity
    {
        public uint Id;
        public int Length;
        public byte[] Data;
    }
    public class ExtendedEntityExtensionTable : ExtensionTable
    {
        public List<ExtendedEntity> EntityDataTable = new List<ExtendedEntity>();
        public Dictionary<uint, ExtendedEntity> EntityDataDictionary = new Dictionary<uint, ExtendedEntity>();
    }
}
