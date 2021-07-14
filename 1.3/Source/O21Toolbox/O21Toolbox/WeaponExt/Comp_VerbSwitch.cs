using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.WeaponExt
{
    public class Comp_VerbSwitch : ThingComp
    {
        public CompProperties_VerbSwitch Props => props as CompProperties_VerbSwitch;

        public int fireMode = 0;

        public CompEquippable Equippable => parent.TryGetComp<CompEquippable>();
        protected virtual bool IsWorn => (GetUser != null);

        protected virtual Pawn GetUser
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_EquipmentTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else
                {
                    return null;
                }
            }
        }

        public VerbProperties Active
        {
            get
            {
                if(parent != null && parent is ThingWithComps)
                {
                    return parent.def.Verbs[fireMode];
                }
                else
                {
                    return null;
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref fireMode, "fireMode", 0);
        }
        public Comp_VerbSwitch()
        {
            if (!(props is CompProperties_VerbSwitch))
            {
                props = new CompProperties_VerbSwitch();
            }
        }

        public override void CompTick()
        {
            base.CompTick();
        }

        public void SwitchFireMode(int x)
        {
            fireMode = x;
            if (Props.useCooldown)
            {
                this.GetUser.stances.SetStance(new Stance_Cooldown(this.Active.AdjustedCooldownTicks(this.Equippable.PrimaryVerb, this.GetUser), this.Equippable.PrimaryVerb.CurrentTarget, this.Equippable.PrimaryVerb));
            }
        }

        public FloatMenu VerbSelectionList()
        {
            List<FloatMenuOption> floatMenu = new List<FloatMenuOption>();
            foreach(VerbProperties verb in parent.def.Verbs)
            {
                int verbIndex = parent.def.Verbs.IndexOf(verb);
                if(fireMode != verbIndex)
                {
                    FloatMenuOption option = new FloatMenuOption(verb.label, delegate () { this.SwitchFireMode(verbIndex); });
                    if(Props.requiredResearchSpecific.Exists(pair => pair.index == verbIndex))
                    {
                        ResearchProjectDef research = Props.requiredResearchSpecific.Find(pair => pair.index == verbIndex).research;
                        if (!research.IsFinished)
                        {
                            option.Label = verb.label + " (Requires Research: " + research.label + ")";
                            option.Disabled = true;
                        }
                    }
                    floatMenu.Add(option);
                }
            }

            return new FloatMenu(floatMenu);
        }

        public IEnumerable<Gizmo> VerbSwitchGizmos()
        {
            ThingWithComps owner = IsWorn ? GetUser : parent;
            if (Find.Selector.SingleSelectedThing == GetUser && GetUser.Drafted && GetUser.Faction == Faction.OfPlayer)
            {
                Texture2D verbIcon;
                if (Active.defaultProjectile.HasModExtension<DefModExt_VerbSwitchIcon>())
                {
                    verbIcon = ContentFinder<Texture2D>.Get(Active.defaultProjectile.GetModExtension<DefModExt_VerbSwitchIcon>().gizmoIcon, true);
                }
                else
                {
                    verbIcon = Active.defaultProjectile.uiIcon;
                }

                Command_Action action = new Command_Action()
                {
                    icon = verbIcon,
                    defaultLabel = "Mode: " + Active.label,
                    defaultDesc = "Switch weapon mode.",
                    activateSound = SoundDefOf.Click,
                    action = delegate ()
                    {
                        Find.WindowStack.Add(VerbSelectionList());
                    }
                };
                if (Props.requiredResearch != null && !Props.requiredResearch.IsFinished)
                {
                    action.Disable(Active.label + " (Requires Research: " + Props.requiredResearch.label + ")");
                }
                else if (GetUser.stances.curStance.StanceBusy)
                {
                    action.Disable("Cannot switch while busy.");
                }
                yield return action;
            }

            yield break;
        }
    }
}
