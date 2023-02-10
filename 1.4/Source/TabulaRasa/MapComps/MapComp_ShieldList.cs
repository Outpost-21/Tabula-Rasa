using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class MapComp_ShieldList : MapComponent
    {
        public List<ThingWithComps> shieldGenList = new List<ThingWithComps>();

        public MapComp_ShieldList(Map map) : base(map)
        {

        }

        public IEnumerable<ThingWithComps> ActiveShieldGens
        {
            get
            {
                return from shieldGen in shieldGenList
                       where (bool)shieldGen.TryGetComp<Comp_Shield>()?.Active
                       select shieldGen;
            }
        }
    }
}
