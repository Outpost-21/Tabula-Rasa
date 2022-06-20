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
    [StaticConstructorOnStartup]
    public static class TabulaRasaStartup
    {
        static TabulaRasaStartup()
        {
            FillLinkablesAutomatically();
        }

        public static void FillLinkablesAutomatically()
        {
            List<ThingDef> linkableNonFacilities = DefDatabase<ThingDef>.AllDefs.Where(def => def.HasComp(typeof(CompProperties_AffectedByFacilities)) && def.HasModExtension<DefModExt_AutomatedLinkables>()).ToList();

            List<ThingDef> linkableFacilities = DefDatabase<ThingDef>.AllDefs.Where(def => def.HasComp(typeof(CompProperties_Facility)) && def.HasModExtension<DefModExt_AutomatedLinkables>()).ToList();

            for (int i = 0; i < linkableNonFacilities.Count(); i++)
            {
                ThingDef currDef = linkableNonFacilities[i];
                DefModExt_AutomatedLinkables defExt = currDef.GetModExtension<DefModExt_AutomatedLinkables>();
                if (!defExt.linkableTags.NullOrEmpty())
                {
                    for (int j = 0; j < linkableFacilities.Count(); j++)
                    {
                        ThingDef currFac = linkableFacilities[j];
                        DefModExt_AutomatedLinkables facExt = currFac.GetModExtension<DefModExt_AutomatedLinkables>();
                        if (!facExt.linkableTags.NullOrEmpty() && !facExt.linkableTags.Intersect(defExt.linkableTags).EnumerableNullOrEmpty() && !currDef.GetCompProperties<CompProperties_AffectedByFacilities>().linkableFacilities.Contains(currFac))
                        {
                            currDef.GetCompProperties<CompProperties_AffectedByFacilities>().linkableFacilities.Add(currFac);
                        }
                    }
                }
            }
        }
    }
}
