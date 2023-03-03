using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking.Types;
using Verse;

namespace HRF
{
    public class ResourceOnAction
    {
        public HediffDef hediffApplied;
        public float adjustSeverity;
        public float adjustResource;
        public bool onSelf;
        public float applyChance = 1f;
        public void TryApplyOn(Pawn pawn)
        {
            if (Rand.Chance(applyChance))
            {
                if (hediffApplied is HediffResourceDef def)
                {
                    Utils.AdjustResourceAmount(pawn, def, adjustResource, true, null, null);
                }
                else
                {
                    HealthUtility.AdjustSeverity(pawn, hediffApplied, adjustSeverity);
                }
            }
        }
    }
    public class ResourceOnActionExtension : DefModExtension
    {
        public List<ResourceOnAction> resourcesOnAction;
    }
}
