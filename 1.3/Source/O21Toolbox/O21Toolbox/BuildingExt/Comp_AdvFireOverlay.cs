using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox.BuildingExt
{
	[StaticConstructorOnStartup]
	public class Comp_AdvFireOverlay : CompFireOverlay
	{
		public new CompProperties_AdvFireOverlay Props => (CompProperties_AdvFireOverlay)props;

		public CompPowerTrader compPower;

		public CompFlickable compFlickable;

		public List<Rot4> showList = new List<Rot4>();

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			showList = Props.showRotations;
			compFlickable = parent.GetComp<CompFlickable>();
			compPower = parent.GetComp<CompPowerTrader>();
		}

		public void DrawCall()
		{
			Vector3 drawPos = parent.DrawPos;
			drawPos.y += 0.046875f;
			CompFireOverlay.FireGraphic.Draw(drawPos, Rot4.North, parent, 0f);
		}

		public override void PostDraw()
		{
			foreach (Rot4 rot in showList)
			{
				if (rot == parent.Rotation)
				{
					if (compPower != null)
					{
						if (compPower.PowerOn)
						{
							DrawCall();
						}
					}
					else
					{
						if (refuelableComp != null)
						{
							if (refuelableComp.HasFuel && compFlickable.SwitchIsOn)
							{
								DrawCall();
							}
						}
						else
						{
							if (compFlickable.SwitchIsOn)
							{
								DrawCall();
							}
						}
					}
					break;
				}
			}
		}
	}
}
