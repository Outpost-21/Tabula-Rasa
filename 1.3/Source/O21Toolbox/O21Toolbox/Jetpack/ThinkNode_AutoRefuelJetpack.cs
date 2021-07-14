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
    public class ThinkNode_AutoRefuelJetpack : ThinkNode_JobGiver
	{
		public override Job TryGiveJob(Pawn pawn)
		{
			Job result;
			if (!O21ToolboxMod.settings.jetpackAutoRefuel || !pawn.IsColonistPlayerControlled)
			{
				result = null;
			}
			else
			{
				if (pawn.InMentalState)
				{
					result = null;
				}
				else
				{
					if (pawn != null && pawn.Map != null)
					{
						result = null;
					}
					else
					{
						JobDef jobdef = DefDatabase<JobDef>.GetNamed("O21_JetpackRefuel", true);
						bool flag3 = ((pawn != null) ? pawn.CurJobDef : null) == jobdef;
						if (pawn != null && pawn.CurJobDef == jobdef)
						{
							result = null;
						}
						else
						{
							Apparel JP = JPUtility.GetWornJP(pawn);
							bool flag4 = JP != null;
							if (flag4)
							{
								int FuelMax = (JP as JetPackApparel).JPFuelMax;
								int Fuel = (JP as JetPackApparel).JPFuelAmount;
								ThingDef FuelItem = (JP as JetPackApparel).JPFuelItem;
								bool flag5 = FuelMax > 0 && Fuel < FuelMax && Fuel * 100 / FuelMax <= Settings.RefuelPCT;
								if (flag5)
								{
									Thing targ;
									this.FindBestRefuel(pawn, FuelItem, FuelMax, Fuel, out targ);
									bool flag6 = targ != null;
									if (flag6)
									{
										return new Job(jobdef, targ);
									}
								}
							}
							result = null;
						}
					}
				}
			}
			return result;
		}



		public Thing FindBestJetpackFuel(Pawn pilot, Apparel_Jetpack jetpack)
		{
			if (pilot != null && pilot.Map != null)
			{
				Log.Message("Stage 1");
				List<Thing> listFuel = pilot.Map.listerThings.ThingsOfDef(jetpack.comp.Props.fuelDef);
				int fuelNeeded = jetpack.comp.Props.maxCharges - jetpack.comp.remainingCharges;
				if (fuelNeeded > jetpack.comp.Props.fuelDef.stackLimit)
				{
					Log.Message("Stage 2");
					fuelNeeded = jetpack.comp.Props.fuelDef.stackLimit;
				}
				if (!listFuel.NullOrEmpty())
				{
					Log.Message("Stage 3");
					Thing bestTarget = null;
					float bestPoints = 0f;
					for (int i = 0; i < listFuel.Count; i++)
					{
						Log.Message("Stage 4");
						Thing targetCheck = listFuel[i];
						if (!targetCheck.IsForbidden(pilot) && targetCheck != null && (targetCheck.Faction == null || targetCheck.Faction.IsPlayer) && pilot.CanReserveAndReach(targetCheck, PathEndMode.ClosestTouch, Danger.Some))
						{
							Log.Message("Stage 5");
							float targetPoints;
							if (targetCheck.stackCount >= fuelNeeded)
							{
								targetPoints = (float)targetCheck.stackCount / pilot.Position.DistanceTo(targetCheck.Position);
								Log.Message("Stage 6");
							}
							else
							{
								Log.Message("Stage 7");
								targetPoints = (float)targetCheck.stackCount / (pilot.Position.DistanceTo(targetCheck.Position) * 2f);
							}

							if (targetPoints > bestPoints)
							{
								Log.Message("Stage 8");
								bestTarget = targetCheck;
								bestPoints = targetPoints;
							}
						}
					}
					Log.Message("Stage 9");
					if (bestTarget != null)
					{
						Log.Message("Stage 10");
						return bestTarget;
					}
				}
			}
			Log.Message("Stage 11");
			return null;
		}
	}
}
