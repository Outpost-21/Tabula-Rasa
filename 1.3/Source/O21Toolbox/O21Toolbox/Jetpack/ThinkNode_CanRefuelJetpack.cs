using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.Jetpack
{
    public class ThinkNode_CanRefuelJetpack : ThinkNode_Conditional
	{
		public override bool Satisfied(Pawn pawn)
		{
			return O21ToolboxMod.settings.jetpackAutoRefuel && pawn.IsColonistPlayerControlled && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving) && !pawn.Downed && !pawn.IsBurning() && !pawn.InMentalState && !pawn.Drafted && pawn.Awake() && !HealthAIUtility.ShouldSeekMedicalRest(pawn);
		}
	}
}
