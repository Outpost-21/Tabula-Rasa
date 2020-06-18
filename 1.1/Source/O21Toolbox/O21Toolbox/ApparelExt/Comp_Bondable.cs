using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
{
    public class Comp_Bondable : ThingComp
    {
        public CompProperties_Bondable Props => (CompProperties_Bondable)props;

        private Pawn bondedPawn;

        public bool IsBonded => BondedPawn != null;

        public Pawn BondedPawn
        {
            get
            {
                return bondedPawn;
            }
            set
            {
                bondedPawn = value;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_References.Look<Pawn>(ref bondedPawn, "bondedPawn");
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder(base.CompInspectStringExtra());

            if (IsBonded)
            {
                stringBuilder.AppendLine("Bonded to: " + BondedPawn.Name.ToString());
            }

            return stringBuilder.ToString().Trim();
        }
    }
}
