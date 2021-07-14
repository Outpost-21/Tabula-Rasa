using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BuildingExt
{
    public class Comp_OrganicBuilding : ThingComp
    {
        public CompProperties_OrganicBuilding Props => (CompProperties_OrganicBuilding)this.props;

        public int healTick = 0;
        public int witherTick = 0;

        CompPowerTrader CompPower;

        public bool hasMaintainer;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            CompPower = this.parent.TryGetComp<CompPowerTrader>();

            healTick = this.Props.ticksBetweenHeal;
            witherTick = this.Props.ticksBetweenWither;
        }

        public override void CompTick()
        {
            base.CompTick();

            CheckMaintainer();
            AttemptHealing();
            AttemptWither();

            healTick--;
        }

        public override void CompTickRare()
        {
            base.CompTickRare();


            CheckMaintainer();
            AttemptHealing();
            AttemptWither();

            healTick--;
        }

        public void CheckMaintainer()
        {
            hasMaintainer = true;
        }

        public void AttemptHealing()
        {
            if (this.parent.HitPoints < this.parent.MaxHitPoints && this.Props.canHeal && healTick <= 0)
            {
                if (this.Props.needsPower && (CompPower == null || !CompPower.PowerOn))
                {
                    return;
                }
                if (this.Props.needsMaintainerToHeal && !hasMaintainer)
                {
                    return;
                }

                this.parent.HitPoints ++;
                healTick = this.Props.ticksBetweenHeal;
            }
        }

        public void AttemptWither()
        {
            if (witherTick <= 0)
            {
                if (this.Props.needsPower && (CompPower == null || !CompPower.PowerOn))
                {
                    return;
                }
                if (this.Props.needsMaintainerToHeal && !hasMaintainer)
                {
                    return;
                }

                this.parent.HitPoints--;
                healTick = this.Props.ticksBetweenHeal;
            }
        }
    }
}
