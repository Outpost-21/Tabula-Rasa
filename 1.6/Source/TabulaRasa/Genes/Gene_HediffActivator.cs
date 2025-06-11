using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class Gene_HediffActivator : Gene
    {
        public int cooldownTicks;

        public override void Tick()
        {
            base.Tick();
            cooldownTicks--;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (!base.GetGizmos().EnumerableNullOrEmpty())
            {
                foreach (Gizmo gizzy in base.GetGizmos())
                {
                    yield return gizzy;
                }
            }
            DefModExt_GeneHediffActivator modExt = def.GetModExtension<DefModExt_GeneHediffActivator>();
            if(PawnIsCapable(modExt))
            {
                yield return new Command_Action
                {
                    defaultLabel = modExt.labelKey.Translate(),
                    defaultDesc = modExt.descKey.Translate(),
                    icon = ContentFinder<Texture2D>.Get(modExt.iconTex),
                    disabled = CheckHasHediff(modExt.hediff) && cooldownTicks <= 0,
                    disabledReason = "TabulaRasa.DisabledByCooldown".Translate(cooldownTicks.TicksToDays()),
                    action = delegate
                    {
                        ApplyHediff(modExt.hediff);
                        cooldownTicks = modExt.cooldown;
                    }
                };
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref cooldownTicks, "cooldownTicks");
        }

        public void ApplyHediff(HediffDef hediff)
        {
            if (!pawn.health.hediffSet.HasHediff(hediff))
            {
                pawn.health.AddHediff(hediff).Severity = 1f;
            }
        }

        public bool CheckHasHediff(HediffDef hediff)
        {
            return pawn.health.hediffSet.HasHediff(hediff);
        }

        public bool PawnIsCapable(DefModExt_GeneHediffActivator modExt)
        {
            // Skill Checks
            if (!modExt.reqSkillLevels.NullOrEmpty())
            {
                foreach(SkillLevelSetting set in modExt.reqSkillLevels)
                {
                    if (pawn.skills.GetSkill(set.skill).Level < set.level)
                    {
                        return false;
                    }
                }
            }
            // Body Checks
            if (!modExt.reqBodyTypes.NullOrEmpty())
            {
                if (!modExt.reqBodyTypes.Contains(pawn.story.bodyType))
                {
                    return false;
                }
            }
            // Hediff Checks
            if (!modExt.reqHediffs.NullOrEmpty())
            {
                foreach (HediffDef hediff in modExt.reqHediffs)
                {
                    if (!CheckHasHediff(hediff))
                    {
                        return false;
                    }
                }
            }
            // Trait Checks
            if (!modExt.reqTraits.NullOrEmpty())
            {
                foreach (TraitDef trait in modExt.reqTraits)
                {
                    if (!pawn.story.traits.HasTrait(trait))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
