using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace O21Toolbox.Jetpack
{
    public class Apparel_Jetpack : Apparel
    {
        public int jetpackSlowBurnTicks;
        public int jetpackCooldownTicks;

        public Comp_Jetpack comp;

        public bool JetpackOnCooldown => jetpackCooldownTicks > 0;

        public bool IsInMelee => ((Wearer != null) ? Wearer.CurJob : null) != null && (Wearer.CurJob.def == JobDefOf.AttackMelee || Wearer.CurJob.def == JobDefOf.AttackStatic);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.jetpackSlowBurnTicks, "jetpackSlowBurnTicks", 0);
            Scribe_Values.Look<int>(ref this.jetpackCooldownTicks, "jetpackCooldownTicks", 0);
        }

        public override void Tick()
        {
            base.Tick();
            if (comp == null)
            {
                comp = GetComp<Comp_Jetpack>();
            }

            if (JetpackOnCooldown)
            {
                jetpackCooldownTicks--;
            }
        }

        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            foreach (Gizmo gizmo in base.GetWornGizmos())
            {
                yield return gizmo;
            }
            if(Wearer != null && Wearer.IsColonistPlayerControlled && Wearer.Map != null && !Wearer.Downed && Wearer.Spawned && !IsInMelee)
            {
                if(Find.Selector.SingleSelectedThing == base.Wearer)
                {
                    yield return new Command_Jetpack
                    {
                        defaultLabel = JetpackOnCooldown ? "Cooldown " + Mathf.RoundToInt(jetpackCooldownTicks.TicksToSeconds()).ToString() : "Jump",
                        defaultDesc = "Use Jetpack to traverse terrain",
                        icon = def.uiIcon,
                        pilot = Wearer,
                        jetpackMinJump = comp.Props.jumpRange.min,
                        jetpackMaxJump = comp.Props.jumpRange.max,
                        action = delegate (IntVec3 cell)
                        {
                            SoundDefOf.Click.PlayOneShotOnCamera(null);
                            UseJetpack(Wearer, this, cell);
                        }
                    };
                    yield return new Command_Action
                    {
                        defaultLabel = "Fuel: " + comp.remainingCharges + "/" + comp.Props.maxCharges,
                        defaultDesc = "Fuel stored in jetpack. Click to refuel manually.",
                        icon = comp.Props.fuelDef.uiIcon,
                        action = delegate ()
                        {
                            SoundDefOf.Click.PlayOneShotOnCamera(null);
                            RefuelJetpack(Wearer, this);
                        }
                    };
                }
            }
            yield break;
        }

        public void RefuelJetpack(Pawn pilot, Apparel_Jetpack jetpack)
        {
            if (comp.remainingCharges < comp.Props.maxCharges)
            {
                string reason;
                if (!JetpackComposMentis(pilot, jetpack, out reason))
                {
                    Messages.Message("Cannot Refuel: " + reason, MessageTypeDefOf.NeutralEvent, false);
                    SoundDefOf.ClickReject.PlayOneShotOnCamera(null);
                    return;
                }
                JobDef jetpackRefuel = DefDatabase<JobDef>.GetNamed("O21_JetpackRefuel");
                Thing target = FindBestJetpackFuel(pilot);
                if(target != null)
                {
                    Job job = new Job(jetpackRefuel, target);
                    pilot.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    return;
                }
                else
                {
                    Messages.Message("No viable jetpack fuel found.", MessageTypeDefOf.NeutralEvent, false);
                    SoundDefOf.ClickReject.PlayOneShotOnCamera(null);
                    return;
                }
            }
            else
            {
                Messages.Message("Jetpack is already fully fueled.", MessageTypeDefOf.NeutralEvent, false);
                SoundDefOf.ClickReject.PlayOneShotOnCamera(null);
                return;
            }
        }

        public Thing FindBestJetpackFuel(Pawn pilot)
        {
            if(pilot != null && pilot.Map != null)
            {
                List<Thing> listFuel = pilot.Map.listerThings.ThingsOfDef(comp.Props.fuelDef);
                int fuelNeeded = comp.Props.maxCharges - comp.remainingCharges;
                if(fuelNeeded > comp.Props.fuelDef.stackLimit)
                {
                    fuelNeeded = comp.Props.fuelDef.stackLimit;
                }
                if(!listFuel.NullOrEmpty())
                {
                    Thing bestTarget = null;
                    float bestPoints = 0f;
                    for (int i = 0; i < listFuel.Count; i++)
                    {
                        Thing targetCheck = listFuel[i];
                        if (!targetCheck.IsForbidden(pilot) && targetCheck != null && (targetCheck.Faction == null || targetCheck.Faction.IsPlayer) && pilot.CanReserveAndReach(targetCheck, PathEndMode.ClosestTouch, Danger.Some))
                        {
                            float targetPoints;
                            if (targetCheck.stackCount >= fuelNeeded)
                            {
                                targetPoints = (float)targetCheck.stackCount / pilot.Position.DistanceTo(targetCheck.Position);
                            }
                            else
                            {
                                targetPoints = (float)targetCheck.stackCount / (pilot.Position.DistanceTo(targetCheck.Position) * 2f);
                            }

                            if(targetPoints > bestPoints)
                            {
                                bestTarget = targetCheck;
                                bestPoints = targetPoints;
                            }
                        }
                    }
                    if (bestTarget != null)
                    {
                        return bestTarget;
                    }
                }
            }
            return null;
        }

        public void UseJetpack(Pawn pilot, Thing jetpack, IntVec3 target)
        {
            string reason;
            if(!JetpackComposMentis(pilot, jetpack, out reason))
            {
                Messages.Message("Can't use. Reason: " + reason, MessageTypeDefOf.NeutralEvent, false);
                SoundDefOf.ClickReject.PlayOneShotOnCamera(null);
                return;
            }
            if (!FlightChecksOK(pilot, jetpack, out reason))
            {
                Messages.Message("Pre-flight Checks Failed. Reason: " + reason, MessageTypeDefOf.NeutralEvent, false);
                SoundDefOf.ClickReject.PlayOneShotOnCamera(null);
                return;
            }
            if (!FlightCellCheck(pilot, target, out reason))
            {
                Messages.Message("Target Invalid. Reason: " + reason, MessageTypeDefOf.NeutralEvent, false);
                SoundDefOf.ClickReject.PlayOneShotOnCamera(null);
                return;
            }
            DoJump(pilot, target);
        }

        public void DoJump(Pawn pilot, IntVec3 targetCell)
        {
            float angle;
            RotatePilot(pilot, targetCell, out angle);
            ThingDef skyfallerDef;
            if (comp.Props.skyfallerDef != null)
            {
                skyfallerDef = comp.Props.skyfallerDef;
            } 
            else 
            {
                skyfallerDef = DefDatabase<ThingDef>.GetNamed("O21_Skyfaller_Jetpack");
            }
            float speed = skyfallerDef.skyfaller.speed;
            if (speed <= 0f)
            {
                speed = 1f;
            }
            float distance = pilot.Position.DistanceTo(targetCell);
            int timeToLand = (int)(distance / speed);
            if (angle >= 360f)
            {
                angle -= 360f;
            }
            comp.UsedOnce();
            Skyfaller skyfaller = SkyfallerMaker.SpawnSkyfaller(skyfallerDef, targetCell, pilot.Map);
            skyfaller.ticksToImpact = timeToLand;
            skyfaller.angle = angle;
            pilot.DeSpawn();
            skyfaller.innerContainer.TryAdd(pilot, false);

        }

        public void RotatePilot(Pawn pilot, IntVec3 cell, out float angle)
        {
            angle = pilot.Position.ToVector3().AngleToFlat(cell.ToVector3());
            float offsetAngle = angle - 90f;
            if(offsetAngle > 360f)
            {
                offsetAngle -= 360f;
            }
            Rot4 facing = Rot4.FromAngleFlat(offsetAngle);
            pilot.Rotation = facing;
            angle = offsetAngle;
        }

        public bool FlightCellCheck(Pawn pilot, IntVec3 cell, out string reason)
        {
            if (!cell.InBounds(pilot.Map))
            {
                reason = "Target location is not within map bounds";
                return false;
            }
            if (!pilot.CanReserve(cell))
            {
                reason = "Target location is already reserved";
                return false;
            }
            if(cell.Roofed(pilot.Map) && !O21ToolboxMod.settings.roofPunch)
            {
                reason = "Target location is roofed";
                return false;
            }
            if (!cell.Walkable(pilot.Map))
            {
                reason = "Target location is not walkable";
                return false;
            }
            if(cell.GetDangerFor(pilot, pilot.Map) == Danger.Deadly)
            {
                reason = "Target location is deadly to " + pilot.Name;
                return false;
            }
            float distance = pilot.Position.DistanceTo(cell);
            if(distance < comp.Props.jumpRange.min)
            {
                reason = "Target location is too close to pilot";
                return false;
            }
            if(distance > comp.Props.jumpRange.max)
            {
                reason = "Target location is outside jump range";
                return false;
            }
            reason = "";
            return true;
        }

        public bool FlightChecksOK(Pawn pilot, Thing jetpack, out string reason)
        {
            if(pilot != null && pilot.Map != null)
            {
                if (comp != null && !comp.CanBeUsed)
                {
                    reason = "Jetpack does not have enough fuel";
                    return false;
                }
                if (pilot.Position.Roofed(pilot.Map) && !O21ToolboxMod.settings.roofPunch)
                {
                    reason = "Pawns position is roofed";
                    return false;
                }
                reason = "";
                return true;

            }
            reason = "Pawn or Map is null";
            return false;
        }

        public bool JetpackComposMentis(Pawn pilot, Thing jetpack, out string reason)
        {
            if(pilot != null)
            {
                if (pilot.IsBurning())
                {
                    reason = "Jetpack cannot be operated while user is on fire!";
                    return false;
                }
                if (pilot.Dead)
                {
                    reason = "Dead people can't really do anything with a jetpack.";
                    return false;
                }
                if (pilot.InMentalState)
                {
                    reason = pilot.Name + " is not feeling the vibes right now.";
                    return false;
                }
                if(pilot.Downed || pilot.stances.stunner.Stunned)
                {
                    reason = pilot.Name + " is currently incapacitated.";
                    return false;
                }
                if(!pilot.Awake() || !pilot.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                {
                    reason = pilot.Name + " cannot operate a jetpack in their current physical state.";
                    return false;
                }
                reason = "";
                return true;
            }
            reason = "Error: Reason Not Found";
            return false;
        }
    }
}
