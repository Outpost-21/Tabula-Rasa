using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Automation
{
    public class MiningFilter : IExposable
    {
        [Unsaved(false)]
        public HashSet<ThingDef> allowedDefs = new HashSet<ThingDef>();

        public MiningFilter()
        {
            allowedDefs = MiningUtility.CachedMineableThings.ToHashSet();
        }

        public virtual void CopyAllowancesFrom(MiningFilter other)
        {
            this.allowedDefs.Clear();
            foreach(ThingDef thingDef in MiningUtility.CachedMineableThings)
            {
                this.SetAllow(thingDef, other.Allows(thingDef));
            }
        }

        public void SetAllow(ThingDef thingDef, bool allow)
        {
            if(allow == this.Allows(thingDef))
            {
                return;
            }
            if (allow)
            {
                this.allowedDefs.Add(thingDef);
            }
            else
            {
                this.allowedDefs.Remove(thingDef);
            }
        }

        public void SetAllowAll()
        {
            allowedDefs.Clear();
            allowedDefs = MiningUtility.CachedMineableThings.ToHashSet();
        }

        public void SetDisallowAll()
        {
            allowedDefs.Clear();
        }

        public bool Allows(ThingDef def)
        {
            return this.allowedDefs.Contains(def);
        }

        public virtual void ExposeData()
        {
            Scribe_Collections.Look<ThingDef>(ref allowedDefs, "allowedDefs");
        }
    }
}
