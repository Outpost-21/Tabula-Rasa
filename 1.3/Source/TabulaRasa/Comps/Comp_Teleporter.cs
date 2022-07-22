using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace TabulaRasa
{
    public class Comp_Teleporter : ThingComp
    {
        public CompProperties_Teleporter Props => (CompProperties_Teleporter)props;

        public CompPowerTrader powerComp;
        public CompFlickable flickComp;
        public CompRefuelable fuelComp;

        public MapComp_Teleporter mapComp;

        public Thing target;

        public bool IsActive => (!Props.needsPower || IsPowered) || (!Props.usesFuel || HasFuel);

        public bool IsPowered => ((powerComp != null && powerComp.PowerOn) || powerComp == null);

        public bool HasFuel => (fuelComp != null && fuelComp.Fuel > Props.fuelCost);

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            powerComp = parent.TryGetComp<CompPowerTrader>();
            flickComp = parent.TryGetComp<CompFlickable>();
            fuelComp = parent.TryGetComp<CompRefuelable>();

            mapComp = parent.Map.GetComponent<MapComp_Teleporter>();

            mapComp.RegisterTeleporter(parent);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            mapComp.UnregisterTeleporter(parent);
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (IsActive)
            {
                if (!selPawn.CanReach(parent, PathEndMode.InteractionCell, Danger.Deadly))
                {
                    FloatMenuOption option = new FloatMenuOption("CannotUseNoPath".Translate(), null);
                    yield return option;
                }
                else
                {
                    if (GetAllViableTeleporters(false).NullOrEmpty())
                    {
                        FloatMenuOption option = new FloatMenuOption("No destinations to choose from.", null);
                        yield return option;
                    }
                    else
                    {
                        foreach (Thing receiver in GetAllViableTeleporters(false))
                        {
                            Comp_Teleporter receiverComp = receiver.TryGetComp<Comp_Teleporter>();

                            FloatMenuOption option = new FloatMenuOption(null, null);

                            if (Props.receiverMustBeActive && !receiverComp.IsActive)
                            {
                                option.Label = "Destination Inactive: " + receiver.Label;
                            }
                            else
                            {
                                option.Label = "Teleport To: " + receiver.Label;
                                option.action = delegate ()
                                {
                                    target = receiver;
                                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(TabulaRasaDefOf.TabulaRasa_UseTeleporter, parent), JobTag.Misc);
                                };
                            }

                            yield return FloatMenuUtility.DecoratePrioritizedTask(option, selPawn, parent, "ReservedBy");
                        }
                    }
                }
            }
            else
            {
                FloatMenuOption option = new FloatMenuOption("Teleporter not active", null);
                yield return option;
            }
            foreach (FloatMenuOption option in base.CompFloatMenuOptions(selPawn))
            {
                yield return option;
            }
            yield break;
        }

        private bool CanBeUsedBy(Pawn p, out string failReason)
        {
            List<ThingComp> allComps = this.parent.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                CompUseEffect compUseEffect = allComps[i] as CompUseEffect;
                if (compUseEffect != null && !compUseEffect.CanBeUsedBy(p, out failReason))
                {
                    return false;
                }
            }
            failReason = null;
            return true;
        }

        private List<Thing> GetAllViableTeleporters(bool needPad)
        {
            List<Thing> results = new List<Thing>();
            if (Props.teleporterType == TeleporterType.world)
            {
                foreach (Map map in Current.Game.Maps)
                {
                    foreach (Thing port in map.GetComponent<MapComp_Teleporter>().allMapTeleports.Where(t => t != parent))
                    {
                        Comp_Teleporter portComp = port.TryGetComp<Comp_Teleporter>();
                        if (portComp != null && !portComp.Props.networkTags.Where(t => Props.networkTags.Contains(t)).ToList().NullOrEmpty()
                            && portComp.Props.direction != TeleporterDirection.transmitter
                            && (!needPad || Props.isPad || portComp.Props.isPad))

                        {
                            results.Add(port);
                        }
                    }
                }
            }
            else if (Props.teleporterType == TeleporterType.local)
            {
                foreach (Thing port in parent.Map.GetComponent<MapComp_Teleporter>().allMapTeleports)
                {
                    Comp_Teleporter portComp = port.TryGetComp<Comp_Teleporter>();
                    if (portComp != null && !portComp.Props.networkTags.Where(t => Props.networkTags.Contains(t)).ToList().NullOrEmpty()
                        && portComp.Props.direction != TeleporterDirection.transmitter
                            && (!needPad || Props.isPad || portComp.Props.isPad))
                    {
                        results.Add(port);
                    }
                }
            }
            return results;
        }

        public void TeleportEffect(Pawn actor)
        {
            TeleportEffect(actor as Thing);
        }

        public void TeleportEffect(Thing thing)
        {
            if (Props.sound != null)
            {
                SoundInfo info = SoundInfo.InMap(new TargetInfo(thing.Position.ToIntVec2.ToIntVec3, thing.Map, false), MaintenanceType.None);
                Props.sound.PlayOneShot(info);
            }
            FleckMaker.ThrowSmoke(thing.Position.ToVector3(), thing.Map, 1.5f);
            FleckMaker.ThrowMicroSparks(thing.Position.ToVector3(), thing.Map);
            FleckMaker.ThrowLightningGlow(thing.Position.ToVector3(), thing.Map, 1.5f);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (Props.canSendNonPawns)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Send",
                    defaultDesc = "Teleports everything on the teleporter to the selected destination.",
                    activateSound = SoundDefOf.Click,
                    icon = ContentFinder<Texture2D>.Get("UI/Buttons/Drop", true),
                    action = delegate
                    {
                        Find.WindowStack.Add(new FloatMenu(DestinationFloatMenuOptions(true).ToList()));
                    }
                };
                //yield return new Command_Action()
                //{
                //    defaultLabel = "Recieve",
                //    defaultDesc = "Teleports everything from the selected teleporter to this one.",
                //    activateSound = SoundDefOf.Click,
                //    icon = ContentFinder<Texture2D>.Get("UI/Buttons/Rename", true),
                //    action = delegate ()
                //    {
                //        Find.WindowStack.Add(new FloatMenu(DestinationFloatMenuOptions(false).ToList()));
                //    }
                //};
            }
            yield break;
        }

        public IEnumerable<FloatMenuOption> DestinationFloatMenuOptions(bool sending)
        {
            if (IsActive)
            {
                if (GetAllViableTeleporters(true).NullOrEmpty())
                {
                    FloatMenuOption option = new FloatMenuOption("No destinations to choose from.", null);
                    yield return option;
                }
                else
                {
                    foreach (Thing receiver in GetAllViableTeleporters(true))
                    {
                        Comp_Teleporter receiverComp = receiver.TryGetComp<Comp_Teleporter>();

                        FloatMenuOption option = new FloatMenuOption(null, null);

                        if (Props.receiverMustBeActive && !receiverComp.IsActive)
                        {
                            option.Label = "Destination Inactive: " + receiver.Label;
                        }
                        else if (sending)
                        {
                            option.Label = "Teleport To: " + receiver.Label;
                            option.action = delegate ()
                            {
                                TeleportToDestination(receiver);
                            };
                        }
                        else
                        {
                            option.Label = "Teleport From: " + receiver.Label;
                            option.action = delegate ()
                            {
                                //TeleportFromDestination(receiver);
                            };
                        }

                        yield return option;
                    }
                }
            }
            else
            {
                FloatMenuOption option = new FloatMenuOption("Teleporter not active", null);
                yield return option;
            }
            yield break;
        }

        public static CellRect TeleportRect(IntVec3 center, Rot4 rot, IntVec2 size)
        {
            GenAdj.AdjustForRotation(ref center, ref size, rot);
            return new CellRect(center.x - (size.x - 1) / 2, center.z - (size.z - 1) / 2, size.x, size.z);
        }

        public void TeleportToDestination(Thing destination)
        {
            List<IntVec3> cells = TeleportRect(parent.Position, parent.Rotation, Props.teleportArea).Cells.ToList();
            List<Thing> thingsToTeleport = new List<Thing>();
            for (int i = 0; i < cells.Count; i++)
            {
                List<Thing> cellThingList = cells[i].GetThingList(parent.Map).ToList();
                if (!cellThingList.NullOrEmpty())
                {
                    foreach (Thing thing in cellThingList)
                    {
                        if (thing != parent && thing != destination)
                        {
                            thingsToTeleport.Add(thing);
                        }
                    }
                }
            }

            foreach (Thing thing in thingsToTeleport)
            {
                TeleportThing(thing, destination);
            }
        }

        //public void TeleportFromDestination(Thing destination)
        //{
        //    List<IntVec3> cells = destination.OccupiedRect().Cells.ToList();
        //    List<Thing> thingsToTeleport = new List<Thing>();
        //    for (int i = 0; i < cells.Count; i++)
        //    {
        //        List<Thing> cellThingList = cells[i].GetThingList(destination.Map).ToList();
        //        if (!cellThingList.NullOrEmpty())
        //        {
        //            foreach (Thing thing in cellThingList)
        //            {
        //                Building building = thing as Building;
        //                if(building == null && thing != parent && thing != destination)
        //                {
        //                    thingsToTeleport.Add(thing);
        //                }
        //            }
        //        }
        //    }

        //    foreach (Thing thing in thingsToTeleport)
        //    {
        //        TeleportThing(thing, parent);
        //    }
        //}

        public void TeleportThing(Thing thing, Thing destination)
        {
            if (IsPowered && destination.TryGetComp<Comp_Teleporter>().IsPowered)
            {
                if (thing != parent && thing != destination)
                {
                    List<IntVec3> potentialCells = destination.OccupiedRect().Cells.Where(c => c.GetThingList(destination.Map).Where(t => t != destination && t.def.thingClass != typeof(Filth)).EnumerableNullOrEmpty()).ToList();
                    if (potentialCells.NullOrEmpty())
                    {
                        return;
                    }
                    else
                    {
                        TeleportEffect(thing);
                        thing.DeSpawn(DestroyMode.Vanish);
                        GenSpawn.Spawn(thing, potentialCells.RandomElement(), destination.Map);
                        destination.TryGetComp<Comp_Teleporter>().TeleportEffect(thing);
                    }
                }
            }
        }


    }
}
