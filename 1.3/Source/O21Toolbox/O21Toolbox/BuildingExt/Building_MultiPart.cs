using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class Building_MultiPart : Building
    {
        public DefModExt_MultiPart modExt;

        public List<Building> linkedBuildings = new List<Building>();

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref linkedBuildings, "linkedBuildings");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            modExt = def.GetModExtension<DefModExt_MultiPart>();
        }

        public override void Tick()
        {
            base.Tick();

            CheckParts();
            SpawnBlueprints();
        }

        public void CheckParts()
        {
            foreach(MultiPartItem item in modExt.parts)
            {
                IntVec3 itemPos = (Position.ToVector3() + item.position).ToIntVec3();

                Building building = (Building)itemPos.GetThingList(Map).Find(t => t.def == item.def);
                if (building != null)
                {
                    linkedBuildings.Add(building);
                }
            }

            if(linkedBuildings.Count() == modExt.parts.Count())
            {
                CompleteBuilding();
            }
        }

        public void SpawnBlueprints()
        {
            while(linkedBuildings.Count() < modExt.parts.Count())
            {
                foreach(MultiPartItem item in modExt.parts)
                {
                    IntVec3 itemPos = (Position.ToVector3() + item.position).ToIntVec3();

                    if(!linkedBuildings.Any(b => b.Position == itemPos) && GenConstruct.CanPlaceBlueprintAt(item.def, itemPos, Rotation, Map, false, null, null, Stuff).Accepted)
                    {
                        GenConstruct.PlaceBlueprintForBuild_NewTemp(item.def, itemPos, Map, Rotation, Faction.OfPlayer, Stuff);
                    }
                }
            }
        }

        public void CompleteBuilding()
        {
            new NotImplementedException();
        }
    }
}
