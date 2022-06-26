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
    public class Comp_WirelessCharger : ThingComp
    {
        public CompProperties_WirelessCharger Props => (CompProperties_WirelessCharger)props;

        public override void CompTickLong()
        {
            base.CompTickLong();

            if ((bool)parent?.TryGetComp<CompPowerTrader>()?.PowerOn)
            {
                AttemptChargeInRadius();
            }
        }

        public void AttemptChargeInRadius()
        {
            for (int i = 0; i < parent.Map.mapPawns.AllPawnsSpawned.Count; i++)
            {
                Pawn pawn = parent.Map.mapPawns.AllPawnsSpawned[i];
                if (pawn != null && pawn.def.HasModExtension<DefModExt_FoodNeedAdjuster>())
                {
                    AttemptChargePawn(pawn);
                }
            }
        }

        public void AttemptChargePawn(Pawn pawn)
        {
            if (pawn.Position.DistanceTo(parent.Position) < Props.chargingRadius)
            {
                DefModExt_FoodNeedAdjuster modExt = pawn.def.GetModExtension<DefModExt_FoodNeedAdjuster>();
                if (modExt != null && modExt.wirelesslyCharged)
                {
                    pawn.needs.food.CurLevel = pawn.needs.food.MaxLevel;
                }
            }
        }
    }
}
