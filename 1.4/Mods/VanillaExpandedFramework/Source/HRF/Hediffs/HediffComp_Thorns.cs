using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace HRF
{
    public class HediffComp_Thorns : HediffComp
    {
        public HediffCompProperties_Thorns Props => props as HediffCompProperties_Thorns;

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            if (Props.ignoreDamageDefs.Contains(dinfo.Def)) return;
            if (dinfo.Weapon != null)
            {
                if (dinfo.Weapon.IsRangedWeapon && !Props.affectRange) return;
                if (dinfo.Weapon.IsMeleeWeapon && !Props.affectMelee) return;
            }

            if (dinfo.WeaponLinkedHediff != null &&
                dinfo.WeaponLinkedHediff.CompProps<HediffCompProperties_VerbGiver>() is HediffCompProperties_VerbGiver
                    verbGiver)
            {
                var isMelee = verbGiver.verbs.Any(v => v.IsMeleeAttack);
                var isRanged = verbGiver.verbs.TrueForAll(v => !v.IsMeleeAttack);
                if (!Props.affectRange && isRanged) return;
                if (!Props.affectMelee && isMelee) return;
            }

            if (Pawn.Position.AdjacentTo8WayOrInside(dinfo.Instigator.Position) && !Props.affectMelee) return;

            var amount1 = Props.thornsDamageFlat;
            var amount2 = Props.thornsDamagePercent * dinfo.Amount;
            var amount = Mathf.Max(amount1, amount2);
            if (amount == 0) return;
            var dinfo2 = new DamageInfo(Props.thornsDamageDef, amount, Props.thornsDamageDef.defaultArmorPenetration,
                dinfo.Angle, Pawn);
            dinfo.Instigator.TakeDamage(dinfo2);
        }
    }

    public class HediffCompProperties_Thorns : HediffCompProperties
    {
        public HediffCompProperties_Thorns()
        {
            compClass = typeof(HediffComp_Thorns);
        } // ReSharper disable InconsistentNaming
        public float thornsDamageFlat;
        public float thornsDamagePercent;
        public DamageDef thornsDamageDef;
        public bool affectMelee;
        public bool affectRange;
        public List<DamageDef> ignoreDamageDefs;
    }
}