using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class ShuttleTurret : Building_Turret
    {
        public override LocalTargetInfo CurrentTarget => throw new NotImplementedException();

        public override Verb AttackVerb => throw new NotImplementedException();

        public override void OrderAttack(LocalTargetInfo targ)
        {
            throw new NotImplementedException();
        }
    }
}
