using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnAbilities
{
    public class Comp_PawnStructure : ThingComp
    {
        public CompProperties_PawnStructure Props => (CompProperties_PawnStructure)props;

        public Pawn storedPawn;

        public bool awakenToHostiles = true;

        public override string TransformLabel(string label)
        {
            return Props.labelString + " " + storedPawn.def.label + "(" + storedPawn.Name + ")";
        }

        public void Awaken()
        {

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo c in base.CompGetGizmosExtra())
            {
                yield return c;
			}
			if (parent.Faction == Faction.OfPlayer)
			{
				yield return new Command_Toggle
				{
					toggleAction = delegate ()
					{
						awakenToHostiles = !awakenToHostiles;
					},
					defaultDesc = "Command_AwakenPawn_Desc".Translate(),
					icon = ContentFinder<Texture2D>.Get(Props.awakenToHostilesIcon, true),
					defaultLabel = "Command_AwakenPawn_Label".Translate()
				};
			}
			yield break;
		}
    }
}
