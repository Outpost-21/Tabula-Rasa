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
    public class Building_NeedProvider : Building_Casket
    {
        public DefModExt_NeedProvider modExt;

        public DefModExt_NeedProvider ModExt
        {
            get 
            { 
                if(modExt == null)
                {
                    modExt = def.GetModExtension<DefModExt_NeedProvider>();
                }
                return modExt; 
            }
        }

        public override bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (base.TryAcceptThing(thing, allowSpecialEffects))
            {
                if (allowSpecialEffects)
                {
                    // This would be where a sound plays, maybe have an option?
                }
                return true;
            }
            return false;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (selPawn.IsQuestLodger() && !modExt.guestsAllowed)
            {
                yield return new FloatMenuOption("CannotUseReason".Translate("TabulaRasa.NeedProvider_GuestsNotAllowed"), null);
            }
            foreach (FloatMenuOption fmo in base.GetFloatMenuOptions(selPawn))
            {
                yield return fmo;
            }
            if (innerContainer.Count != 0)
            {
                yield break;
            }
            if(!selPawn.CanReach(this, Verse.AI.PathEndMode.InteractionCell, Danger.Deadly))
            {
                yield return new FloatMenuOption("CannotUseNoPath".Translate(), null);
                yield break;
            }
            JobDef jobDef = TabulaRasaDefOf.TabulaRasa_EnterNeedProvider;
            string label = "TabulaRasa.NeedProvider_Enter".Translate();
            Action action = delegate
            {
                if (ModsConfig.BiotechActive)
                {
                    if (!(selPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicBond) is Hediff_PsychicBond hediff_PsychicBond) || !ThoughtWorker_PsychicBondProximity.NearPsychicBondedPerson(selPawn, hediff_PsychicBond))
                    {
                        selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(jobDef, this), JobTag.Misc);
                    }
                    else
                    {
                        Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("PsychicBondDistanceWillBeActive_Cryptosleep".Translate(selPawn.Named("PAWN"), ((Pawn)hediff_PsychicBond.target).Named("BOND")), delegate
                        {
                            selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(jobDef, this), JobTag.Misc);
                        }, destructive: true));
                    }
                }
                else
                {
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(jobDef, this), JobTag.Misc);
                }
            };
            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action), selPawn, this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (base.Faction == Faction.OfPlayer && innerContainer.Count > 0 && def.building.isPlayerEjectable)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.action = EjectContents;
                command_Action.defaultLabel = "CommandPodEject".Translate();
                command_Action.defaultDesc = "CommandPodEjectDesc".Translate();
                if (innerContainer.Count == 0)
                {
                    command_Action.Disable("CommandPodEjectFailEmpty".Translate());
                }
                command_Action.hotKey = KeyBindingDefOf.Misc8;
                command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject");
                yield return command_Action;
            }
        }

        public override void EjectContents()
        {
            ThingDef filth_Slime = ThingDefOf.Filth_Slime;
            foreach (Thing item in (IEnumerable<Thing>)innerContainer)
            {
                if (item is Pawn pawn)
                {
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                    pawn.filth.GainFilth(filth_Slime);
                    if (pawn.RaceProps.IsFlesh)
                    {
                        pawn.health.AddHediff(HediffDefOf.CryptosleepSickness);
                    }
                }
            }
            if (!base.Destroyed)
            {
                SoundDefOf.CryptosleepCasket_Eject.PlayOneShot(SoundInfo.InMap(new TargetInfo(base.Position, base.Map)));
            }
            base.EjectContents();
        }

        public static Building_CryptosleepCasket FindCryptosleepCasketFor(Pawn p, Pawn traveler, bool ignoreOtherReservations = false)
        {
            foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => def.IsCryptosleepCasket))
            {
                Building_CryptosleepCasket building_CryptosleepCasket = (Building_CryptosleepCasket)GenClosest.ClosestThingReachable(p.PositionHeld, p.MapHeld, ThingRequest.ForDef(item), PathEndMode.InteractionCell, TraverseParms.For(traveler), 9999f, (Thing x) => !((Building_CryptosleepCasket)x).HasAnyContents && traveler.CanReserve(x, 1, -1, null, ignoreOtherReservations));
                if (building_CryptosleepCasket != null)
                {
                    return building_CryptosleepCasket;
                }
            }
            return null;
        }
    }
}
