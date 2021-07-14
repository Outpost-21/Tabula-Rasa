using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Hivemind
{
    public class Comp_HivemindCore : ThingComp
    {
        public CompProperties_HivemindCore Props => (CompProperties_HivemindCore)this.props;

        public List<Pawn> connectedPawns = new List<Pawn>();

        public CompPowerTrader powerComp;

        public bool isActive = false;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Collections.Look(ref connectedPawns, "connectedPawns");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            GetPowerComp();
            ApplyHiveHediff();
        }

        public override void CompTick()
        {
            base.CompTick();

            if (!IsActive())
            { 
                DisconnectPawns(); 
            }
        }

        public void Notify_ConnectionChanged()
        {
            if(parent is Pawn)
            {
                UpdateHediff((Pawn)parent);
            }
        }

        public void UpdateHediff(Pawn pawn)
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hivemindCoreHediff);
            float severity = Props.severityAtCount.Evaluate(connectedPawns.Count());
            hediff.Severity = severity;
        }

        public void ApplyHiveHediff()
        {
            if(parent is Pawn && Props.hivemindCoreHediff != null)
            {
                Pawn pawn = (Pawn)parent;
                if (!pawn.health.hediffSet.HasHediff(Props.hivemindCoreHediff))
                {
                    BodyPartRecord partRecord = null;
                    Hediff hediffNew = HediffMaker.MakeHediff(Props.hivemindCoreHediff, pawn, null);
                    hediffNew.Severity = 0.0f;
                    pawn.health.AddHediff(hediffNew, partRecord, null, null);
                }
            }
        }

        public void DisconnectPawns()
        {
            for (int i = 0; i < connectedPawns.Count; i++)
            {
                connectedPawns[i].TryGetComp<Comp_HivemindPawn>().Disconnect(this);
            }
        }

        public void GetPowerComp()
        {
            if(parent is Building)
            {
                powerComp = parent.TryGetComp<CompPowerTrader>();
            }
        }

        public bool IsActive()
        {
            if(parent is Building && HasPower())
            {
                return true;
            }
            return false;
        }

        public bool HasPower()
        {
            if(parent is Pawn)
            {
                Pawn pawn = parent as Pawn;
                if (!pawn.Dead)
                {
                    return true;
                }
            }
            else if(parent is Building)
            {
                if(powerComp == null)
                {
                    return true;
                }
                else if (powerComp.PowerOn)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
