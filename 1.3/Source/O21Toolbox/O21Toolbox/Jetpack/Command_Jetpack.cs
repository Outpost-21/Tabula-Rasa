using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace O21Toolbox.Jetpack
{

	public class Command_Jetpack : Command
	{
		public static TargetingParameters targParms = Command_Jetpack.ForJetpackDestination();
		public static string jetpackIconPath = "Toolbox/Special/JetPackIcon";
		public static bool jetpackRoofPunch = O21ToolboxMod.settings.roofPunch;
		public Action<IntVec3> action;

		public Pawn pilot;

		public float jetpackMaxJump;

		public float jetpackMinJump;

		internal static TargetingParameters ForJetpackDestination()
        {
            TargetingParameters targetingParameters = new TargetingParameters();
            targetingParameters.canTargetLocations = true;
            targetingParameters.canTargetSelf = false;
            targetingParameters.canTargetSelf = false;
            targetingParameters.canTargetPawns = false;
            targetingParameters.canTargetFires = false;
            targetingParameters.canTargetBuildings = false;
            targetingParameters.canTargetItems = false;
            targetingParameters.validator = ((TargetInfo x) => DropCellFinder.IsGoodDropSpot(x.Cell, x.Map, true, Command_Jetpack.jetpackRoofPunch, true));
            return targetingParameters;
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
            Texture2D JPIcon = ContentFinder<Texture2D>.Get(Command_Jetpack.jetpackIconPath, true);
            Find.Targeter.BeginTargeting(Command_Jetpack.targParms, delegate (LocalTargetInfo target)
            {
                this.action(target.Cell);
            }, this.pilot, null, JPIcon);
		}

		public override void GizmoUpdateOnMouseover()
		{
			if (Find.CurrentMap != null)
			{
				if (jetpackMaxJump > 0f)
				{
					GenDraw.DrawRadiusRing(pilot.Position, jetpackMaxJump);
				}
				if (this.jetpackMinJump > 0f)
				{
					GenDraw.DrawRadiusRing(pilot.Position, jetpackMinJump);
				}
			}
		}

		public override Color IconDrawColor
		{
			get
			{
				return base.IconDrawColor;
			}
		}

		public override bool InheritInteractionsFrom(Gizmo other)
		{
			return false;
		}
	}
}
