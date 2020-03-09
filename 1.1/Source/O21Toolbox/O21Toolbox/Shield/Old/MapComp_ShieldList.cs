using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Shield
{
    public class MapComp_ShieldList : MapComponent
    {
        public List<Building_Shield> shieldGenList = new List<Building_Shield>();

        public MapComp_ShieldList(Map map) : base(map)
        {

        }

        public IEnumerable<Building_Shield> ActiveShieldGens
        {
            get
            {
                return from shieldGen in shieldGenList
                       where shieldGen.active && shieldGen.CurShieldStress > 1.0f
                       select shieldGen;
            }
        }
    }
}
