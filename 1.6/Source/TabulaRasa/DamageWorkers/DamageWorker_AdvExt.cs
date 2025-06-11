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
    public class DamageWorker_AdvExt : DamageWorker_AddInjury
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            DamageResult result =  base.Apply(dinfo, thing);

            DefModExt_DamageAdv ext = dinfo.Def.GetModExtension<DefModExt_DamageAdv>();
            if(ext != null)
            {
                if(ext.hediff != null && thing is Pawn pawn)
                {
                    ApplyHediffToPawn(pawn, ext, dinfo, result);
                }
            }

            return result;
        }

        public void ApplyHediffToPawn(Pawn pawn, DefModExt_DamageAdv ext, DamageInfo dinfo, DamageResult damage)
        {
            FleshTypeDef flesh = ext.fleshTypeToAffect;
            if(flesh == null || flesh == pawn.RaceProps.FleshType)
            {
                Hediff existingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(ext.hediff);
                if (existingHediff != null)
                {
                    if(ext.increaseSevPerShot > 0f)
                    {
                        existingHediff.Severity += ext.increaseSevPerShot;
                    }
                }
                else
                {
                    if (ext.wholeBody)
                    {
                        Hediff newHediff = HediffMaker.MakeHediff(ext.hediff, pawn);
                        newHediff.Severity = ext.hediffSev;
                        pawn.health.AddHediff(newHediff, null, dinfo);
                    }
                    else if(ext.bodyPart != null)
                    {
                        foreach(BodyPartRecord bodyPart in pawn.RaceProps.body.GetPartsWithDef(ext.bodyPart))
                        {
                            Hediff newHediff = HediffMaker.MakeHediff(ext.hediff, pawn, bodyPart);
                            newHediff.Severity = ext.hediffSev;
                            pawn.health.AddHediff(newHediff, bodyPart, dinfo);
                        }
                    }
                    else
                    {
                        foreach(BodyPartRecord bodyPart in damage.parts)
                        {
                            Hediff newHediff = HediffMaker.MakeHediff(ext.hediff, pawn, bodyPart);
                            newHediff.Severity = ext.hediffSev;
                            pawn.health.AddHediff(newHediff, bodyPart, dinfo);
                        }
                    }
                }
            }
        }
    }
}
