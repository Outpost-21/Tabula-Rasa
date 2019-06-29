using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;

namespace O21Toolbox.TurretsPlus
{
    public class Building_Emplacement : Building_TurretGun, IThingHolder
    {
        [DefOf]
        public static class BunkerDefOf
        {
            public static JobDef EnterEmplacement;
        }

        protected ThingOwner<Pawn> innerContainer;

        public int maxCount = 1;

        private bool holdFire;

        public Building_Emplacement()
        {
            this.innerContainer = new ThingOwner<Pawn>(this, false, LookMode.Deep);
            this.top = new TurretTop(this);
        }

        public bool HasAnyContents
        {
            get
            {
                return this.innerContainer.Count > 0;
            }
        }

        public Thing ContainedThing
        {
            get
            {
                return (this.innerContainer.Count != 0) ? this.innerContainer[0] : null;
            }
        }

        public bool CanOpen
        {
            get
            {
                return this.HasAnyContents;
            }
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        public ThingOwner<Pawn> GetInner()
        {
            return this.innerContainer;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public override void TickRare()
        {
            base.TickRare();
            this.innerContainer.ThingOwnerTickRare(true);
        }

        public override void Tick()
        {
            base.Tick();
            if (this.CanExtractShell && this.MannedByColonist)
            {
                CompChangeableProjectile compChangeableProjectile = this.gun.TryGetComp<CompChangeableProjectile>();
                if (!compChangeableProjectile.allowedShellsSettings.AllowedToAccept(compChangeableProjectile.LoadedShell))
                {
                    this.ExtractShell();
                }
            }
            if (this.forcedTarget.IsValid && !this.CanSetForcedTarget)
            {
                this.ResetForcedTarget();
            }
            if (!this.CanToggleHoldFire)
            {
                this.holdFire = false;
            }
            if (this.forcedTarget.ThingDestroyed)
            {
                this.ResetForcedTarget();
            }
            bool flag = (this.powerComp == null || this.powerComp.PowerOn) && (this.MannedByColonist);
            if (flag && base.Spawned)
            {
                this.GunCompEq.verbTracker.VerbsTick();
                if (!this.stunner.Stunned && this.AttackVerb.state != VerbState.Bursting)
                {
                    if (this.WarmingUp)
                    {
                        this.burstWarmupTicksLeft--;
                        if (this.burstWarmupTicksLeft == 0)
                        {
                            this.BeginBurst();
                        }
                    }
                    else
                    {
                        if (this.burstCooldownTicksLeft > 0)
                        {
                            this.burstCooldownTicksLeft--;
                        }
                        if (this.burstCooldownTicksLeft <= 0 && this.IsHashIntervalTick(10))
                        {
                            this.TryStartShootSomething(true);
                        }
                    }
                    this.top.TurretTopTick();
                }
            }
            else
            {
                this.ResetCurrentTarget();
            }
        }

        private bool WarmingUp
        {
            get
            {
                return this.burstWarmupTicksLeft > 0;
            }
        }

        private void ResetCurrentTarget()
        {
            this.currentTargetInt = LocalTargetInfo.Invalid;
            this.burstWarmupTicksLeft = 0;
        }

        private bool CanToggleHoldFire
        {
            get
            {
                return this.PlayerControlled;
            }
        }

        private void ResetForcedTarget()
        {
            this.forcedTarget = LocalTargetInfo.Invalid;
            this.burstWarmupTicksLeft = 0;
            if (this.burstCooldownTicksLeft <= 0)
            {
                this.TryStartShootSomething(false);
            }
        }

        private bool CanSetForcedTarget
        {
            get
            {
                return this.mannableComp != null && this.PlayerControlled;
            }
        }

        private bool CanExtractShell
        {
            get
            {
                if (!this.PlayerControlled)
                {
                    return false;
                }
                CompChangeableProjectile compChangeableProjectile = this.gun.TryGetComp<CompChangeableProjectile>();
                return compChangeableProjectile != null && compChangeableProjectile.Loaded;
            }
        }

        private void UpdateGunVerbs()
        {
            List<Verb> allVerbs = this.gun.TryGetComp<CompEquippable>().AllVerbs;
            for (int i = 0; i < allVerbs.Count; i++)
            {
                Verb verb = allVerbs[i];
                verb.caster = this;
                verb.castCompleteCallback = new Action(this.BurstComplete);
            }
        }

        private void ExtractShell()
        {
            GenPlace.TryPlaceThing(this.gun.TryGetComp<CompChangeableProjectile>().RemoveShell(), base.Position, base.Map, ThingPlaceMode.Near, null, null);
        }

        public virtual void Open()
        {
            bool flag = !this.HasAnyContents;
            if (!flag)
            {
                this.EjectContents();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<ThingOwner<Pawn>>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
        }

        private bool MannedByColonist
        {
            get
            {
                return this.mannableComp != null && this.innerContainer != null && this.ContainedThing != null && this.ContainedThing.Faction == Faction.OfPlayer;
            }
        }

        private bool PlayerControlled
        {
            get
            {
                return base.Faction == Faction.OfPlayer || this.MannedByColonist;
            }
        }

        public virtual bool Accepts(Thing thing)
        {
            return this.innerContainer.CanAcceptAnyOf(thing, true);
        }

        public virtual bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            bool flag = !this.Accepts(thing);
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = thing.holdingOwner != null;
                bool flag3;
                if (flag2)
                {
                    thing.holdingOwner.TryTransferToContainer(thing, this.innerContainer, thing.stackCount, true);
                    flag3 = true;
                }
                else
                {
                    flag3 = this.innerContainer.TryAdd(thing, true);
                }
                bool flag4 = flag3;
                result = flag4;
            }
            return result;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            bool flag = this.innerContainer.Count > 0 && (mode == DestroyMode.Deconstruct || mode == DestroyMode.KillFinalize);
            if (flag)
            {
                this.EjectContents();
            }
            this.innerContainer.ClearAndDestroyContents(DestroyMode.Vanish);
            base.Destroy(mode);
        }

        public virtual void EjectContents()
        {
            this.innerContainer.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near, null, null);
        }

        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            string str = this.innerContainer.Count + "/" + this.maxCount;
            bool flag = !text.NullOrEmpty();
            if (flag)
            {
                text += "\n";
            }
            return string.Concat(new string[]
            {
                text,
                "CasketContains".Translate(),
                ": ",
                str.CapitalizeFirst(),
                (this.innerContainer.Count == this.maxCount) ? "(Full)" : ""
            });
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            if (!ReservationUtility.CanReserve(myPawn, this, 1))
            {
                FloatMenuOption item = new FloatMenuOption(Translator.Translate("CannotUseReserved"), (Action)null, MenuOptionPriority.Default, (Action)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
            {
                item
            };
            }
            if (!ReachabilityUtility.CanReach(myPawn, this, PathEndMode.InteractionCell, Danger.Some, false))
            {
                FloatMenuOption item2 = new FloatMenuOption(Translator.Translate("CannotUseNoPath"), (Action)null, MenuOptionPriority.Default, (Action)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
                {
                    item2
                };
            }
            if (!this.HasAnyContents)
            {
                FloatMenuOption item3 = new FloatMenuOption(Translator.Translate("EnterEmplacement"), (Action)delegate
                {
                    Job val2 = new Job(BunkerDefOf.EnterEmplacement, this);
                    ReservationUtility.Reserve(myPawn, this, val2);
                    myPawn.jobs.TryTakeOrderedJob(val2);
                }, MenuOptionPriority.Default, (Action)null, null, 0f, (Func<Rect, bool>)null, null);
                return new List<FloatMenuOption>
                {
                    item3
                };
            }
            return null;
        }



        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo c in base.GetGizmos())
            {
                yield return c;
            }
            bool flag = base.Faction == Faction.OfPlayer && this.innerContainer.Count > 0;
            if (flag)
            {
                Command_Action eject = new Command_Action
                {
                    action = new Action(this.EjectContents),
                    defaultLabel = "CommandPodEject".Translate(),
                    defaultDesc = "CommandPodEjectDesc".Translate()
                };
                bool flag2 = this.innerContainer.Count == 0;
                if (flag2)
                {
                    eject.Disable("CommandPodEjectFailEmpty".Translate());
                }
                eject.hotKey = KeyBindingDefOf.Misc1;
                eject.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject", true);
                yield return eject;
                eject = null;
            }
            yield break;
        }
    }
}
