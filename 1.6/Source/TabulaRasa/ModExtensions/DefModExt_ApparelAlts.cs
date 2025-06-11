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
    public class DefModExt_ApparelAlts : DefModExtension
    {
        public List<ApparelAlts> apparelAlts = new List<ApparelAlts>();
        public Dictionary<string, ApparelAlts> apparelAltData;

        public ApparelAlts TryGetAltApparelData(string headTypeDef)
        {
            if(apparelAltData == null)
            {
                apparelAltData = new Dictionary<string, ApparelAlts>();
                foreach(ApparelAlts alt in apparelAlts)
                {
                    for (int j = 0; j < alt.headTypeDef.Count; j++)
                    {
                        string curHeadTypeDef = alt.headTypeDef[j];
                        if (string.IsNullOrWhiteSpace(curHeadTypeDef)) { LogUtil.Error($"Missing <headTypeDef> tag in apparelAlts list item."); continue; }
                        if (apparelAltData.ContainsKey(curHeadTypeDef)) { LogUtil.Error($"Duplicate apparel data for {curHeadTypeDef}"); }
                        HeadTypeDef htd = DefDatabase<HeadTypeDef>.GetNamedSilentFail(curHeadTypeDef);
                        if (htd == null) { LogUtil.Warning($"Could not find def for headTypeDef named '{curHeadTypeDef}'."); }
                        apparelAltData.Add(curHeadTypeDef, alt);
                    }
                }
            }
            if(headTypeDef == null) { return null; }
            return apparelAltData.TryGetValue(headTypeDef);
        }
    }
}
