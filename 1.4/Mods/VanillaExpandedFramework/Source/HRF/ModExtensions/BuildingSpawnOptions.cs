using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HRF
{
    public class MaterialReplace
    {
        public TerrainDef floorDef;
	    public ThingDef replaceWithThingDef;
	    public ThingDef replaceWithStuffDef;
    }
    public class BuildingSpawnOptions : DefModExtension
    {
        public List<MaterialReplace> materialReplaces;
    }
}
