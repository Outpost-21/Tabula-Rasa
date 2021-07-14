using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnExt
{
    [StaticConstructorOnStartup]
    public static class ResurrectionUtility
    {
        static ResurrectionUtility()
        {
            foreach(ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                CompProperties_Resurrection comp = thingDef.GetCompProperties<CompProperties_Resurrection>();
                if(comp != null)
                {
                    ThingDef corpseDef = thingDef.race?.corpseDef;
                    if(corpseDef != null)
                    {
                        CompProperties_Resurrection corpseComp = new CompProperties_Resurrection();

                        corpseComp.chanceToResurrect = comp.chanceToResurrect;
                        corpseComp.requiredBodyPart = comp.requiredBodyPart;
                        corpseComp.ticksToResurrect = comp.ticksToResurrect;
                            
                        corpseDef.comps.Add(corpseComp);
                    }
                }
            }
        }
    }
}
