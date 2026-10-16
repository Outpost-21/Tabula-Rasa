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
    public class Comp_Recall : ThingComp
    {
        public CompProperties_Teleporter Props => (CompProperties_Teleporter)props;

        public CompApparelReloadable compReloadable;

        public Thing target;

        public Pawn GetPawn => ApparelUtil.WearerOf(this);

        public Comp_Recall()
        {
            compReloadable = parent.TryGetComp<CompApparelReloadable>();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref target, "target");
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
            {
                yield return gizmo;
            }
            string reason = "";
            if (compReloadable == null || compReloadable.CanBeUsed(out reason))
            {
                yield return new Command_FloatAction
                {
                    defaultLabel = target != null ? "TabulaRasa.RecallDest".Translate(target.LabelCap) : "TabulaRasa.RecallLabel".Translate(),
                    defaultDesc = "TabulaRasa.RecallDesc".Translate(),
                    activateSound = SoundDefOf.Click,
                    icon = ContentFinder<Texture2D>.Get("UI/Buttons/Drop", true),
                    action = delegate
                    {
                        if (target == null)
                        {
                            Messages.Message("No destination selected. Right click the gizmo to select one.", MessageTypeDefOf.CautionInput);
                        }
                        else if (Props.receiverMustBeActive && !target.TryGetComp<Comp_Teleporter>().IsActive)
                        {
                            Messages.Message("Selected destination is currently inactive, cannot recall to.", MessageTypeDefOf.CautionInput);
                        }
                        else
                        {
                            GetPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(TabulaRasaDefOf.TabulaRasa_UseRecall, GetPawn.Position, parent), JobTag.Misc);
                        }
                        //Find.WindowStack.Add(new FloatMenu(DestinationFloatMenuOptions(true).ToList()));
                    },
                    floatMenuFunc = DestinationFloatMenuOptions
                };
            }
            yield break;
        }

        private List<Thing> GetAllViableTeleporters(bool needPad = false)
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

        private IEnumerable<FloatMenuOption> DestinationFloatMenuOptions()
        {
            string reason = "";
            if (compReloadable == null || compReloadable.CanBeUsed(out reason))
            {
                if (GetAllViableTeleporters().NullOrEmpty())
                {
                    FloatMenuOption option = new FloatMenuOption("No destinations to choose from.", null);
                    yield return option;
                }
                else
                {
                    foreach (Thing receiver in GetAllViableTeleporters())
                    {
                        Comp_Teleporter receiverComp = receiver.TryGetComp<Comp_Teleporter>();

                        FloatMenuOption option = new FloatMenuOption(null, null);

                        if (Props.receiverMustBeActive && !receiverComp.IsActive)
                        {
                            option.Label = "Destination Inactive: " + receiver.Label;
                        }
                        else
                        {
                            option.Label = "Set Destination: " + receiver.Label;
                            option.action = delegate ()
                            {
                                target = receiver;
                                // GetPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(TabulaRasaDefOf.TabulaRasa_UseRecall, GetPawn.Position, parent), JobTag.Misc);
                            };
                        }

                        yield return option;
                    }
                }
            }
            else
            {
                FloatMenuOption option = new FloatMenuOption("No uses remaining.", null);
                yield return option;
            }
            yield break;
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
    }
}
