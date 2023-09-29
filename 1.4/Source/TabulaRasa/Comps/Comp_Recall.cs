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

        public CompReloadable compReloadable;

        public Thing target;

        public Pawn GetPawn => ApparelUtil.WearerOf(this);

        public Comp_Recall()
        {
            compReloadable = parent.TryGetComp<CompReloadable>();
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
            if (compReloadable == null || compReloadable.CanBeUsed)
            {
                yield return new Command_Recall
                {
                    defaultLabel = "Recall",
                    defaultDesc = "Teleports the pawn equipped with this item to the selected destination.",
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
                    }
                };
            }
            yield break;
        }

        private List<Thing> GetAllViableTeleporters
        {
            get
            {
                List<Thing> results = new List<Thing>();

                foreach (Map map in Current.Game.Maps)
                {
                    foreach (Thing port in map.GetComponent<MapComp_Teleporter>().allMapTeleports)
                    {
                        Comp_Teleporter portComp = port.TryGetComp<Comp_Teleporter>();
                        if (portComp != null && !portComp.Props.networkTags.Where(t => Props.networkTags.Contains(t)).ToList().NullOrEmpty()
                            && portComp.Props.direction != TeleporterDirection.transmitter)

                        {
                            results.Add(port);
                        }
                    }
                }
                return results;
            }
        }

        public IEnumerable<FloatMenuOption> DestinationFloatMenuOptions(bool sending)
        {
            if (compReloadable == null || compReloadable.CanBeUsed)
            {
                if (GetAllViableTeleporters.NullOrEmpty())
                {
                    FloatMenuOption option = new FloatMenuOption("No destinations to choose from.", null);
                    yield return option;
                }
                else
                {
                    foreach (Thing receiver in GetAllViableTeleporters)
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
                                GetPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(TabulaRasaDefOf.TabulaRasa_UseRecall, GetPawn.Position, parent), JobTag.Misc);
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
