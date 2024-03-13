using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class WorldComp_FactionWars : WorldComponent
    {
        private Dictionary<AllegianceDef, float> warProgressDict;

        public Dictionary<AllegianceDef, float> WarProgressDict
        {
            get
            {
                if(warProgressDict == null)
                {
                    warProgressDict = new Dictionary<AllegianceDef, float>();
                }
                return warProgressDict;
            }
        }

        public WorldComp_FactionWars(World world) : base(world)
        {
            foreach(AllegianceDef def in DefDatabase<AllegianceDef>.AllDefs)
            {
                if (!WarProgressDict.ContainsKey(def))
                {
                    WarProgressDict.Add(def, 0f);
                }
            }
        }

        public float GetWarProgress(AllegianceDef def)
        {
            return WarProgressDict[def];
        }

        public void SetWarProgress(AllegianceDef def, float value)
        {
            WarProgressDict[def] = Mathf.Clamp(value, -1f, 1f);
        }

        public void AdjustWarProgress(AllegianceDef def, float value)
        {
            WarProgressDict[def] = Mathf.Clamp(WarProgressDict[def] + value, -1f, 1f);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref warProgressDict, "warProgressDict");
        }
    }
}
