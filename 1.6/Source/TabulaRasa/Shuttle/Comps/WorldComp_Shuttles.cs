using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class WorldComp_Shuttles : WorldComponent
    {
        public Dictionary<Map, HashSet<Thing>> registeredShuttles = new Dictionary<Map, HashSet<Thing>>();

        public WorldComp_Shuttles(World world) : base(world)
        {
        }

        public void RegisterShuttle(Map map, Thing thing)
        {
            if (map == null || thing == null) { return; }
            if (!registeredShuttles.ContainsKey(map))
            {
                registeredShuttles.Add(map, new HashSet<Thing>() { thing });
                return;
            }
            if (!registeredShuttles[map].Contains(thing))
            {
                registeredShuttles[map].Add(thing);
            }
        }

        public void UnregisterShuttle(Map map, Thing thing)
        {
            if (!registeredShuttles.ContainsKey(map)) { return; }
            if (map == null) 
            { 
                registeredShuttles.Remove(map); 
                return; 
            }
            if (!registeredShuttles[map].NullOrEmpty() && registeredShuttles[map].Contains(thing))
            {
                registeredShuttles[map].Remove(thing);
            }
            if (registeredShuttles[map].NullOrEmpty()) 
            { 
                registeredShuttles.Remove(map); 
            }
        }
    }
}
