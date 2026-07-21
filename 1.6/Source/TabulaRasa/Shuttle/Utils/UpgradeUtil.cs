using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TabulaRasa.Shuttle
{
    public static class UpgradeUtil
    {
        public static WorldComp_Shuttles GetUpgradeableShuttleWorldComp
        {
            get
            {
                WorldComp_Shuttles comp = Find.World.GetComponent(typeof(WorldComp_Shuttles)) as WorldComp_Shuttles;
                if (comp != null)
                {
                    return comp;
                }
                else
                {
                    Log.Error("Could not find WorldComp_UpgradeableShuttles.");
                }
                return null;
            }
        }
        public static List<Thing> GetAllShuttlesOnMap(this Map map)
        {
            WorldComp_Shuttles comp = GetUpgradeableShuttleWorldComp;
            if (comp.registeredShuttles.ContainsKey(map))
            {
                return comp.registeredShuttles[map].ToList();
            }
            return new List<Thing>();
        }
    }
}
